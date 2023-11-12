using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BitByByte.Data;
using BitByByte.Models;
using Microsoft.AspNetCore.Identity;
using BitByByte.Service;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace BitByByte.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly RestaurantDbContext _context;
        private IWebHostEnvironment _env;
        private readonly IUserService _authService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationsController(RestaurantDbContext context, IWebHostEnvironment env, IUserService authService, UserManager<ApplicationUser> userManager)
        {
            _context = context; 
            _env = env;
            _authService = authService;
            _userManager = userManager;
        }

        public async Task<IActionResult> getViewData()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["UserDetails"] = user;
            }

            ViewData["SittingId"] = _context.Sitting.Select(a => new SelectListItem { Text = $"{a.Type} - {a.StartTime:dd/MM/yyyy hh:mm:tt} to {a.EndTime:hh:mm:tt}", Value = a.SittingId.ToString() });
            ViewData["ReservationSourceList"] = Enum.GetValues(typeof(Source))
                .Cast<Source>()
                .Select(value => new SelectListItem
                {
                    Text = value.ToString(),
                    Value = value.ToString()
                })
                .ToList();
            ViewData["StatusList"] = Enum.GetValues(typeof(Status))
                .Cast<Status>()
                .Select(value => new SelectListItem
                {
                    Text = value.ToString(),
                    Value = value.ToString()
                })
                .ToList();

            return View();
        } 

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                // if user is an Admin, show all reservations
                if (await _userManager.IsInRoleAsync(user, "admin"))
                {
                    var allSittings = await _context.Reservation.Include(r => r.Sitting).ToListAsync();
                    return View(allSittings);
                }
                else
                {
                    // If not an admin, only show user's reservations
                    var usersSittings = await _context.Reservation.Where(r => r.Email == user.Email).ToListAsync();
                    return View(usersSittings);
                }
            }

            return View();
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reservation == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .Include(r => r.Sitting)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public async Task<IActionResult> Create()
        {
            await getViewData();

            var availableSittings = await _context.Sitting
                .Where(r => r.CurrentCapacity != r.Capacity)
                .Where(r => r.Status != sittingStatus.Closed)
                .ToListAsync();

            // override to only show available sittings, not full sittings
            ViewData["SittingId"] = availableSittings.Select(a => new SelectListItem { Text = $"{a.Type} - {a.StartTime:dd/MM/yyyy hh:mm:tt} to {a.EndTime:hh:mm:tt} - Seats: {a.CurrentCapacity}/{a.Capacity}", Value = a.SittingId.ToString() });

            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservationId,SittingId,FirstName,LastName,StartTime,Duration,Email,PhoneNumber,Notes,Status,GuestCount,ReservationSource")] Reservation reservation) 
        {
            var user = await _userManager.GetUserAsync(User);

            // ensure users email, matches account email
            if (user != null && user.Email != reservation.Email)
            {
                ModelState.AddModelError("Email", "You must use your accounts linked email address.");
                return View(reservation);
            }

            // if user is creating a reservation, and deosn't have an account, check email provided is available
            if (user == null)
            {
                var emailInUse = _userManager.FindByEmailAsync(reservation.Email);
                if (emailInUse.Result != null)
                {
                    ModelState.AddModelError("Email", "Email already in use.");
                    return View(reservation);
                }
            }

            // reservation status should always be pending default, which is 0 in the enum
            reservation.Status = 0;

            await getViewData();

            // Retrieve the Sitting model based on the SittingId
            var sitting = _context.Sitting.SingleOrDefault(s => s.SittingId == reservation.SittingId);

            if (sitting == null)
            {
                return View(reservation);
            } 
            else
            {
                int newCapacity = sitting.CurrentCapacity + reservation.GuestCount;
                // StartTime must occur within the Sittings timeframe
                if (reservation.StartTime < sitting.StartTime || reservation.StartTime > sitting.EndTime)
                {
                    ModelState.AddModelError("StartTime", "The Reservation's start time must occur within the Sitting's timeframe.");
                    return View(reservation);
                }

                // Check Sitting capacity, only insert reservation if sitting capacity is availalbe
                if (newCapacity > sitting.Capacity)
                {
                    ModelState.AddModelError("GuestCount", "The selected sittings capacity is at: " + sitting.CurrentCapacity + "/" + sitting.Capacity);
                    return View(reservation);
                }

                if (ModelState.IsValid)
                {
                    // if capacity will be full, also update sitting status to closed
                    if (newCapacity == sitting.Capacity)
                    {
                        sitting.Status = sittingStatus.Closed;
                    }

                    sitting.CurrentCapacity += reservation.GuestCount;
                    _context.Update(sitting);
                    await _context.SaveChangesAsync();

                    _context.Add(reservation);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(reservation);
        }

        // GET: Reservations/Edit/5
        [Authorize(Roles = "admin, staff")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reservation == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            ViewData["startTime"] = reservation.StartTime;
            await getViewData();

            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReservationId,SittingId,FirstName,LastName,StartTime,Duration,Email,PhoneNumber,Notes,Status,GuestCount,ReservationSource")] Reservation reservation)
        {
            if (id != reservation.ReservationId)
            {
                return NotFound();
            }

            ViewData["startTime"] = reservation.StartTime;
            await getViewData();

            // Update the sitting capacity, when deleting a reservation
            var sitting = _context.Sitting.SingleOrDefault(s => s.SittingId == reservation.SittingId);
            var currentGuestCount = _context.Reservation
                .Where(r => r.ReservationId == reservation.ReservationId)
                .Select(r => r.GuestCount)
                .SingleOrDefault();

            // Minus the old guest count from the sitting, add back the new guestcount
            int newCapacity = sitting.CurrentCapacity - currentGuestCount + reservation.GuestCount;

            // StartTime must occur within the Sittings timeframe
            if (reservation.StartTime < sitting.StartTime || reservation.StartTime > sitting.EndTime)
            {
                ModelState.AddModelError("StartTime", "The Reservation's start time must occur within the Sitting's timeframe.");
                return View(reservation);
            }

            // Check Sitting capacity, only insert reservation if sitting capacity is availalbe
            if (newCapacity > sitting.Capacity)
            {
                ModelState.AddModelError("GuestCount", "The selected sittings capacity is at: " + sitting.CurrentCapacity + "/" + sitting.Capacity);
                return View(reservation);
            }

            if (ModelState.IsValid)
            {
                _context.Update(reservation);
                await _context.SaveChangesAsync();

                if (sitting != null)
                {
                    sitting.CurrentCapacity = newCapacity;
                    _context.Update(sitting);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            return View(reservation);
        }

        // GET: Reservations/Delete/5
        [Authorize(Roles = "admin, staff")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reservation == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .Include(r => r.Sitting)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reservation == null)
            {
                return Problem("Entity set 'RestaurantDbContext.Reservation'  is null.");
            }
            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservation.Remove(reservation);

                // Update the sitting capacity, when deleting a reservation
                var sitting = _context.Sitting.SingleOrDefault(s => s.SittingId == reservation.SittingId);
                if (sitting != null)
                {
                    sitting.CurrentCapacity -= reservation.GuestCount;
                    _context.Update(sitting);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return (_context.Reservation?.Any(e => e.ReservationId == id)).GetValueOrDefault();
        }
    }
}
