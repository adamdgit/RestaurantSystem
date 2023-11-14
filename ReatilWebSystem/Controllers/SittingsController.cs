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
    // Sittings controller and methods are only accessible to admins
    [Authorize(Roles = "admin")]
    public class SittingsController : Controller
    {
        private readonly RestaurantDbContext _context;

        public SittingsController(RestaurantDbContext context)
        {
            _context = context;
        }

        // Gets sitting status list from enum, reused in many methods
        public IActionResult getViewData()
        {
            ViewData["SittingStatusList"] = Enum.GetValues(typeof(sittingStatus))
                .Cast<sittingStatus>()
                .Select(value => new SelectListItem
                {
                    Text = value.ToString(),
                    Value = value.ToString()
                })
                .ToList();

            return View();
        }

        // GET: Sittings
        public async Task<IActionResult> Index()
        {

            var Sittings = await _context.Sitting.ToListAsync();

            if (Sittings == null)
            {
                return NotFound();
            }

            return View(Sittings);

        }

        // API to GET sittings by user query
        [HttpGet]
        public async Task<IActionResult> GetAvailableSittings()
        {
            // get query from url
            string query = HttpContext.Request.Query["query"];

            if (query == null)
            {
                return NotFound("Invalid query");
            }

            // if Id exists, return all Reservations with the same SittingId foreign key
            var data = await _context.Sitting
                .Where(r => r.Type.Contains(query))
                .ToListAsync();

            var selectListItems = data.Select(sitting => new SelectListItem
            {
                Text = $"{sitting.Type} - {sitting.StartTime:dd/MM/yyyy hh:mm:tt} to {sitting.EndTime:hh:mm:tt} - Seats: {sitting.CurrentCapacity}/{sitting.Capacity}",
                Value = sitting.SittingId.ToString()
            }).ToList();

            return Json(selectListItems);
        }

        // GET: Sittings/Details/:ID
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Sitting == null)
            {
                return NotFound();
            }

            var sitting = await _context.Sitting
                .FirstOrDefaultAsync(m => m.SittingId == id);
            if (sitting == null)
            {
                return NotFound();
            }

            return View(sitting);
        }

        // GET: Sittings/Create
        public IActionResult Create()
        {
            getViewData();
            return View();
        }

        // POST: Sittings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SittingId,Type,StartTime,EndTime,Capacity,Status")] Sitting sitting)
        {

            getViewData();

            if (ModelState.IsValid)
            {
                _context.Add(sitting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(sitting);
        }

        // GET: Sittings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            getViewData();

            if (id == null || _context.Sitting == null)
            {
                return NotFound();
            }

            var sitting = await _context.Sitting.FindAsync(id);
            if (sitting == null)
            {
                return NotFound();
            }
            return View(sitting);
        }

        // POST: Sittings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SittingId,Type,StartTime,EndTime,Capacity,Status")] Sitting sitting)
        {

            getViewData();

            if (id != sitting.SittingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var currentSitting = await _context.Sitting.FindAsync(id);
                if (currentSitting != null)
                {
                    currentSitting.Type = sitting.Type;
                    currentSitting.Status = sitting.Status;
                    currentSitting.StartTime = sitting.StartTime;
                    currentSitting.EndTime = sitting.EndTime;
                    currentSitting.Capacity = sitting.Capacity;
                    _context.Update(currentSitting);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return View(sitting);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sitting);
        }

        // GET: Sittings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Sitting == null)
            {
                return NotFound();
            }

            var sitting = await _context.Sitting
                .FirstOrDefaultAsync(m => m.SittingId == id);
            if (sitting == null)
            {
                return NotFound();
            }

            return View(sitting);
        }

        // POST: Sittings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Sitting == null)
            {
                return Problem("Entity set 'RestaurantDbContext.Sitting'  is null.");
            }
            var sitting = await _context.Sitting.FindAsync(id);
            if (sitting != null)
            {
                _context.Sitting.Remove(sitting);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SittingExists(int id)
        {
            return (_context.Sitting?.Any(e => e.SittingId == id)).GetValueOrDefault();
        }
    }
}
