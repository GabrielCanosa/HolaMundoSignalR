using HolaMundoSignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HolaMundoSignalR.Services
{
    public interface IDatabaseChangeNotificationService
    {
        void Config();
    }

    public class SqlDependencyService : IDatabaseChangeNotificationService
    {
        private readonly IConfiguration _configuration;
        private readonly IHubContext<ChatHub> _chatHub;

        public SqlDependencyService(IConfiguration configuration, IHubContext<ChatHub> chatHub)
        {
            _configuration = configuration;
            _chatHub = chatHub;
        }

        public void Config()
        {
            SuscribirseALosCambiosDeLaTablaPersonas();
        }

        private void SuscribirseALosCambiosDeLaTablaPersonas()
        {
            string connString = _configuration.GetConnectionString("DefaultConnection");
            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                // No funciona con SELECT * FROM ...
                using (var cmd = new SqlCommand(@"SELECT Nombre FROM Personas", conn))
                {
                    cmd.Notification = null;
                    SqlDependency dependency = new SqlDependency(cmd);
                    dependency.OnChange += Persona_Cambio;
                    SqlDependency.Start(connString);
                    cmd.ExecuteReader();
                }
            }
        }

        private void Persona_Cambio(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                string mensaje = ObtenerMensajeAMostrar(e);
                _chatHub.Clients.All.SendAsync("ReceiveMessage", "Admin", mensaje);
            }
            SuscribirseALosCambiosDeLaTablaPersonas();
        }

        private string ObtenerMensajeAMostrar(SqlNotificationEventArgs e)
        {
            switch (e.Info)
            {
                case SqlNotificationInfo.Insert:
                    return "El registro ha sido insertado";
                case SqlNotificationInfo.Update:
                    return "El registro ha sido actualizado";
                case SqlNotificationInfo.Delete:
                    return "El registro ha sido borrado";
                default:
                    return "ha ocurrido un cambio de la tabla";
            }
        }
    }
}
