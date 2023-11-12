using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BitByByte.Data;
using BitByByte.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace BitByByte.Controllers
{
    [Authorize(Roles = "admin")]
    public class ReportsController : Controller
    {
        private readonly RestaurantDbContext _context;

        public ReportsController(RestaurantDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // API - GET reservations by date
        [HttpGet]
        public async Task<IActionResult> GetReservationByDate()
        {
            string date = HttpContext.Request.Query["date"];

            if (date == null)
            {
                return BadRequest("Invalid Date selected");
            }

                var convertedDate = DateTime.Parse(date);

                // return all reservations that occur on the selected date
                var data = await _context.Reservation
                    .Where(r => r.StartTime.Date == convertedDate.Date) // get only date, not hours, mins, secs
                    .ToListAsync();

                var selectListItems = data.Select(reservation => new SelectListItem
                {
                    Text = $"ResID: {reservation.ReservationId} Name: {reservation.FirstName} {reservation.LastName} Guests: {reservation.GuestCount} SittingID: {reservation.SittingId}",
                    Value = reservation.ReservationId.ToString()
                }).ToList();

                return Json(selectListItems);

        }

        // API - GET sittings by date
        [HttpGet]
        public async Task<IActionResult> GetSittingsByDate()
        {
            string date = HttpContext.Request.Query["date"];

            if (date == null)
            {
                return BadRequest("Invalid Date selected");
            }

            var convertedDate = DateTime.Parse(date);

            // return all reservations that occur on the selected date
            var data = await _context.Sitting
                .Where(r => r.StartTime.Date == convertedDate.Date) // get only date, not hours, mins, secs
                .ToListAsync();

            var selectListItems = data.Select(sitting => new SelectListItem
            {
                Text = $"ID: {sitting.SittingId} Time: {sitting.StartTime} - {sitting.EndTime} Capacity: {sitting.CurrentCapacity} / {sitting.Capacity}",
                Value = sitting.SittingId.ToString()
            }).ToList();

            return Json(selectListItems);

        }

    }
}
