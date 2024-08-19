using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BitByByte.Data;
using BitByByte.Models;

namespace BitByByte.Controllers
{
    public class AssignedTablesController : Controller
    {
        private readonly RestaurantDbContext _context;

        public AssignedTablesController(RestaurantDbContext context)
        {
            _context = context;
        }

        // GET: AssignedTables
        public async Task<IActionResult> Index()
        {
            var assignedTables = await _context.AssignedTable
                .Include(a => a.Reservation)
                .Include(a => a.Sitting)
                .Include(a => a.TableInfo)
                .ToListAsync();

            ViewData["assignedTables"] = assignedTables;

            return View();
        }

        // GET: AssignedTables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.AssignedTable == null)
            {
                return NotFound();
            }

            var assignedTable = await _context.AssignedTable
                .Include(a => a.Reservation)
                .Include(a => a.Sitting)
                .Include(a => a.TableInfo)
                .FirstOrDefaultAsync(m => m.AssignedTableId == id);
            if (assignedTable == null)
            {
                return NotFound();
            }

            return View(assignedTable);
        }

        // GET reservations where sitting is selected by the user 
        [HttpGet]
        public async Task<IActionResult> GetReservationsBySittingID()
        {
            int id = int.Parse(HttpContext.Request.Query["sittingId"]);

            if (id == null)
            {
                return NotFound("Invalid ID");
            }

            // if Id exists, return all Reservations with the same SittingId foreign key
            var data = await _context.Reservation
                .Where(r => r.SittingId == id)
                .ToListAsync();

            var selectListItems = data.Select(reservation => new SelectListItem
            {
                Text = $"ID: {reservation.ReservationId} - {reservation.FirstName} {reservation.LastName} - {reservation.Email}",
                Value = reservation.ReservationId.ToString()
            }).ToList();

            return Json(selectListItems);
        }

        // Get available tables for a sitting
        [HttpGet]
        public async Task<IActionResult> GetAvailableTablesBySittingId()
        {
            int id = int.Parse(HttpContext.Request.Query["sittingId"]);

            if (id == null)
            {
                return NotFound("Invalid ID");
            }

            // Query the AssignedTable to get the TableIds linked to reservations for the specified SittingId
            var reservedTableIds = await _context.AssignedTable
                .Where(at => at.SittingId == id)
                .Select(at => at.TableId)
                .ToListAsync();

            // Query the TableInfo to get available tables (not linked to any reservations for the specified SittingId)
            var availableTables = await _context.TableInfo
                .Where(ti => !reservedTableIds.Contains(ti.TableId))
                .Select(ti => new SelectListItem
                {
                    Text = $"Table: {ti.Name} Area: {ti.Area.Name}",
                    Value = ti.TableId.ToString()
                })
                .ToListAsync();

            return Json(availableTables);
        }

        // GET: AssignedTables/Create
        public IActionResult Create()
        {
                ViewData["SittingId"] = _context.Sitting.Select(a => new SelectListItem { Text = $"{a.Type} - {a.StartTime:MM-dd-yyyy hh:mm:tt} to {a.EndTime:hh:mm:tt}", Value = a.SittingId.ToString() });
                ViewData["TableId"] = _context.TableInfo.Select(a => new SelectListItem { Text = $"Table: {a.Name} Area: {a.Area.Name}", Value = a.TableId.ToString() });
                return View();
        }

        // POST: AssignedTables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AssignedTableId,TableId,ReservationId,SittingId")] AssignedTable assignedTable)
        {
            if (ModelState.IsValid)
            {
                _context.Add(assignedTable);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReservationId"] = new SelectList(_context.Reservation, "ReservationId", "Email", assignedTable.ReservationId);
            ViewData["SittingId"] = new SelectList(_context.Sitting, "SittingId", "Type", assignedTable.SittingId);
            ViewData["TableId"] = new SelectList(_context.TableInfo, "TableId", "Availability", assignedTable.TableId);
            return View(assignedTable);
        }

        // GET: AssignedTables/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AssignedTable == null)
            {
                return NotFound();
            }

            var assignedTable = await _context.AssignedTable.FindAsync(id);
            if (assignedTable == null)
            {
                return NotFound();
            }
            ViewData["ReservationId"] = new SelectList(_context.Reservation, "ReservationId", "Email", assignedTable.ReservationId);
            ViewData["SittingId"] = new SelectList(_context.Sitting, "SittingId", "Type", assignedTable.SittingId);
            ViewData["TableId"] = new SelectList(_context.TableInfo, "TableId", "Availability", assignedTable.TableId);
            return View(assignedTable);
        }

        // POST: AssignedTables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AssignedTableId,TableId,ReservationId,SittingId")] AssignedTable assignedTable)
        {
            if (id != assignedTable.AssignedTableId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assignedTable);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssignedTableExists(assignedTable.AssignedTableId))
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
            ViewData["ReservationId"] = new SelectList(_context.Reservation, "ReservationId", "Email", assignedTable.ReservationId);
            ViewData["SittingId"] = new SelectList(_context.Sitting, "SittingId", "Type", assignedTable.SittingId);
            ViewData["TableId"] = new SelectList(_context.TableInfo, "TableId", "Availability", assignedTable.TableId);
            return View(assignedTable);
        }

        // GET: AssignedTables/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AssignedTable == null)
            {
                return NotFound();
            }

            var assignedTable = await _context.AssignedTable
                .Include(a => a.Reservation)
                .Include(a => a.Sitting)
                .Include(a => a.TableInfo)
                .FirstOrDefaultAsync(m => m.AssignedTableId == id);
            if (assignedTable == null)
            {
                return NotFound();
            }

            return View(assignedTable);
        }

        // POST: AssignedTables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AssignedTable == null)
            {
                return Problem("Entity set 'RestaurantDbContext.AssignedTable'  is null.");
            }
            var assignedTable = await _context.AssignedTable.FindAsync(id);
            if (assignedTable != null)
            {
                _context.AssignedTable.Remove(assignedTable);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssignedTableExists(int id)
        {
            return (_context.AssignedTable?.Any(e => e.AssignedTableId == id)).GetValueOrDefault();
        }
    }
}
