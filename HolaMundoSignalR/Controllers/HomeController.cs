using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HolaMundoSignalR.Models;
using Microsoft.AspNetCore.SignalR;
using HolaMundoSignalR.Hubs;

namespace HolaMundoSignalR.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHubContext<ChatHub> _chatHub;

        public HomeController(IHubContext<ChatHub> chatHub)
        {
            _chatHub = chatHub;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            _chatHub.Clients.All.SendAsync("ReceiveMessage", "Admin", "Alguien a entrado a la pagina de About");

            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
