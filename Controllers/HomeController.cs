using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using aspcode.Models;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace aspcode.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
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

        [HttpPost]
        public ActionResult GetIpAndMac()
        {
            string ipAddress = GetIPAddress();
            string macAddress = GetClientMAC(ipAddress);
            ViewBag.IPAddress = ipAddress; // Guardar la dirección IP en ViewBag
            ViewBag.MacAddress = macAddress; // Guardar la dirección MAC en ViewBag
            return View("Index");
        }

        private string GetIPAddress()
        {
            // Captura la dirección IP del cliente
            return HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        private static string GetClientMAC(string strClientIP)
        {
            string macDest = "";
            try
            {
                Int32 ldest = inet_addr(strClientIP);
                Int32 lhost = inet_addr("");
                Int64 macInfo = new Int64();
                Int32 len = 6;
                int res = SendARP(ldest, 0, ref macInfo, ref len);
                string macSrc = macInfo.ToString("X");

                while (macSrc.Length < 12)
                {
                    macSrc = macSrc.Insert(0, "0");
                }

                for (int i = 0; i < 11; i++)
                {
                    if (0 == (i % 2))
                    {
                        if (i == 10)
                        {
                            macDest = macDest.Insert(0, macSrc.Substring(i, 2));
                        }
                        else
                        {
                            macDest = "-" + macDest.Insert(0, macSrc.Substring(i, 2));
                        }
                    }
                }
            }
            catch (Exception err)
            {
                throw new Exception("Error: " + err.Message);
            }
            return macDest;
        }

        [DllImport("Iphlpapi.dll")]
        private static extern int SendARP(Int32 dest, Int32 host, ref Int64 mac, ref Int32 length);
        [DllImport("Ws2_32.dll")]
        private static extern Int32 inet_addr(string ip);
    }
}