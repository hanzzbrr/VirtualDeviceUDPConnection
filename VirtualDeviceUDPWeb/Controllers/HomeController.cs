using Microsoft.AspNetCore.Mvc;
using VirtualDeviceUDPWeb.Models;

namespace VirtualDeviceUDPWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index() => View(DeviceRepository.SharedRepository.Devices);
    }
}
