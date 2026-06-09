using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Booking_Management_System.Models
{
        public class Room
        {
            public int Id { get; set; }

            [Required]
            public string RoomNumber { get; set; }

            [Required]
            public string Type { get; set; }   // Single, Double, Suite

            [Required]
            [Precision(18, 2)]
            public decimal Price { get; set; }

            public bool IsAvailable { get; set; }
        }
    }


