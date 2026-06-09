using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_Booking_Management_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminsController:Controller
    {
        public IActionResult AdminDashboard()
        {
            return View();
        }
    }

}

