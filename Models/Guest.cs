using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel_Booking_Management_System.Models
{
    public class Guest
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public String contactInfo {  get; set; }

        public string? UserId { get; set; }   // FK to AspNetUsers table

        [ForeignKey("UserId")]
        public IdentityUser? User { get; set; }

        public ICollection<Booking> Bookings { get; set; }

    }
}
