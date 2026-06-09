using Hotel_Booking_Management_System.Models;
using Hotel_Booking_Management_System.Models.ViewModels;
using HotelApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel_Booking_Management_System.Controllers
{
   
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            // Check if user is Admin
            if (User.IsInRole("Admin"))
            {
                // Admin sees everything
                var allBookings = await _context.Bookings
                    .Include(b => b.Guest)
                    .Include(b => b.Room)
                    .ToListAsync();

                return View(allBookings);
            }

            // Check if user is Guest
            if (User.IsInRole("Guest"))
            {
                // Get logged-in user ID
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                // Find the corresponding Guest entity
                var guest = await _context.Guests
                    .FirstOrDefaultAsync(g => g.UserId == userId);
                if (guest == null)
                {
                    return Unauthorized(); // No guest profile found
                }

                // Load ONLY this guest's bookings
                var myBookings = await _context.Bookings
                    .Where(b => b.GuestId == guest.Id)
                    .Include(b => b.Room)
                    .Include(b => b.Guest)
                    .ToListAsync();

                return View(myBookings);
            }

            // Any other role → not allowed
            return Unauthorized();
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Room)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            if (User.IsInRole("Admin"))
            {
                // Admin sees all guests
                ViewData["GuestId"] = new SelectList(_context.Guests, "Id", "Name");
            }
            else if (User.IsInRole("Guest"))
            {
                // Guest sees only their own name
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var guest = _context.Guests.FirstOrDefault(g => g.UserId == userId);
                if (guest == null)
                {
                    return Unauthorized();
                }
                ViewData["GuestId"] = new SelectList(new List<Guest> { guest }, "Id", "Name");
            }

            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNumber");
            ViewData["StatusList"] = new List<string> { "Confirmed", "Cancelled", "Completed" };
            return View();

        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RoomId,GuestId,CheckIn,CheckOut,Status")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                booking.CheckIn = booking.CheckIn.Date;
                booking.CheckOut = booking.CheckOut.Date;

                if (booking.CheckOut < booking.CheckIn)
                {
                    ModelState.AddModelError("", "Check-out date must be after the check-in date.");
                    LoadDropDowns(booking);   // helper to reload dropdown lists
                    return View(booking);
                }

                if (booking.CheckOut == booking.CheckIn)
                {
                    ModelState.AddModelError("", "Booking must be at least 1 night.");
                    LoadDropDowns(booking);
                    return View(booking);
                }

                // Overbooking prevention: check for overlapping bookings
                var overlapping = await _context.Bookings
                .Where(b => b.RoomId == booking.RoomId &&
                b.Id != booking.Id &&
                b.CheckIn < booking.CheckOut &&
                booking.CheckIn < b.CheckOut)
                .AnyAsync();

                if (overlapping)
                {
                    ModelState.AddModelError("", "Room is already booked for these dates.");
                    LoadDropDowns(booking);
                    return View(booking);
                }

                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["GuestId"] = new SelectList(_context.Guests, "Id", "Name", booking.GuestId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNumber", booking.RoomId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            // Guest dropdown logic based on role
            if (User.IsInRole("Admin"))
            {
                // Admin sees all guests
                ViewData["GuestId"] = new SelectList(_context.Guests, "Id", "Name", booking.GuestId);
            }
            else if (User.IsInRole("Guest"))
            {
                // Guest sees only themselves
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var guest = await _context.Guests.FirstOrDefaultAsync(g => g.UserId == userId);
                if (guest == null)
                {
                    return Unauthorized();
                }
                ViewData["GuestId"] = new SelectList(new List<Guest> { guest }, "Id", "Name", booking.GuestId);
            }

            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNumber", booking.RoomId);
            ViewData["StatusList"] = new List<string> { "Confirmed", "Cancelled", "Completed" };
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RoomId,GuestId,CheckIn,CheckOut,Status")] Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                booking.CheckIn = booking.CheckIn.Date;
                booking.CheckOut = booking.CheckOut.Date;

                if (booking.CheckOut < booking.CheckIn)
                {
                    ModelState.AddModelError("", "Check-out date must be after check-in date.");
                    LoadDropDowns(booking);
                    return View(booking);
                }

                if (booking.CheckOut == booking.CheckIn)
                {
                    ModelState.AddModelError("", "Booking must be at least 1 night.");
                    LoadDropDowns(booking);
                    return View(booking);
                }

                // Check for overlapping bookings (Edit)
                var overlapping = await _context.Bookings
                    .Where(b => b.RoomId == booking.RoomId &&
                                b.Id != booking.Id && // exclude the current booking
                                b.CheckIn < booking.CheckOut &&
                                booking.CheckIn < b.CheckOut)
                    .AnyAsync();

                if (overlapping)
                {
                    ModelState.AddModelError("", "Room is already booked for these dates.");
                    LoadDropDowns(booking); // reload dropdowns
                    return View(booking);
                }

                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GuestId"] = new SelectList(_context.Guests, "Id", "Name", booking.GuestId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNumber", booking.RoomId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Room)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }

        private void LoadDropDowns(Booking booking)
        {
            ViewData["GuestId"] = new SelectList(_context.Guests, "Id", "Name", booking.GuestId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "RoomNumber", booking.RoomId);
            ViewData["StatusList"] = new List<string> { "Confirmed", "Cancelled", "Completed" };
        }

        // Guest Receipt
        public IActionResult Receipt(int id, bool exportToPdf = false)
        {
            // Step 2a: Get the booking by ID
            var booking = _context.Bookings
                .Include(b => b.Guest) // Include guest details
                .Include(b => b.Room)  // Include room details
                .FirstOrDefault(b => b.Id == id);

            if (booking == null)
            {
                return NotFound(); // If booking ID is invalid
            }

            // Step 2b: Calculate total price
            var totalDays = (booking.CheckOut - booking.CheckIn).Days;
            var totalPrice = totalDays * booking.Room.Price;

            var viewModel = new ReceiptViewModel
            {
                Booking = booking,
                TotalPrice = totalPrice
            };

            if (exportToPdf)
            {
                // Return the View as a PDF file instead of standard HTML
                return new ViewAsPdf("Receipt", viewModel)

                {

                    // Optional: Customize the PDF filename
                    FileName = $"Receipt_{booking.Guest.Name}_{booking.Id}.pdf",

                    // Optional: Adjust page size/orientation
                    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,

                    // Optional: Tell Rotativa not to use the standard shared layout (for a cleaner receipt)
                    PageMargins = new Rotativa.AspNetCore.Options.Margins(5, 5, 5, 5) // Minimal margins
                };


            }
            // Return the view with booking info
            return View(viewModel);
        }



    }
}