using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel_Booking_Management_System.Models
{
        public class Booking
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "Room is required")]
            [ForeignKey("Room")]
            public int RoomId { get; set; }
            public Room? Room { get; set; }

            
            [Required(ErrorMessage = "Guest is required")]
            [ForeignKey("Guest")]
             public int GuestId { get; set; }
            public Guest? Guest { get; set; }

            [Required]
            public DateTime CheckIn { get; set; }

            [Required]
            public DateTime CheckOut { get; set; }

            public string Status { get; set; } // Confirmed, Cancelled, Completed
        }
    }

