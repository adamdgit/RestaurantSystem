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
    public class TableInfoesController : Controller
    {
        private readonly RestaurantDbContext _context;

        public TableInfoesController(RestaurantDbContext context)
        {
            _context = context;
        }

        // GET: TableInfoes
        public async Task<IActionResult> Index()
        {
            var restaurantDbContext = _context.TableInfo.Include(t => t.Area);
            return View(await restaurantDbContext.ToListAsync());
        }

        // GET: TableInfoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TableInfo == null)
            {
                return NotFound();
            }

            var tableInfo = await _context.TableInfo
                .Include(t => t.Area)
                .FirstOrDefaultAsync(m => m.TableId == id);
            if (tableInfo == null)
            {
                return NotFound();
            }

            return View(tableInfo);
        }

        // GET: TableInfoes/Create
        public IActionResult Create()
        {
            ViewData["AreaId"] = new SelectList(_context.Area, "AreaId", "Name");
            return View();
        }

        // POST: TableInfoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TableId,Name,Seats,Availability,AreaId")] TableInfo tableInfo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tableInfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AreaId"] = new SelectList(_context.Area, "AreaId", "Name", tableInfo.AreaId);
            return View(tableInfo);
        }

        // GET: TableInfoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TableInfo == null)
            {
                return NotFound();
            }

            var tableInfo = await _context.TableInfo.FindAsync(id);
            if (tableInfo == null)
            {
                return NotFound();
            }
            ViewData["AreaId"] = new SelectList(_context.Area, "AreaId", "Name", tableInfo.AreaId);
            return View(tableInfo);
        }

        // POST: TableInfoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TableId,Name,Seats,Availability,AreaId")] TableInfo tableInfo)
        {
            if (id != tableInfo.TableId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tableInfo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TableInfoExists(tableInfo.TableId))
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
            ViewData["AreaId"] = new SelectList(_context.Area, "AreaId", "Name", tableInfo.AreaId);
            return View(tableInfo);
        }

        // GET: TableInfoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TableInfo == null)
            {
                return NotFound();
            }

            var tableInfo = await _context.TableInfo
                .Include(t => t.Area)
                .FirstOrDefaultAsync(m => m.TableId == id);
            if (tableInfo == null)
            {
                return NotFound();
            }

            return View(tableInfo);
        }

        // POST: TableInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TableInfo == null)
            {
                return Problem("Entity set 'RestaurantDbContext.TableInfo'  is null.");
            }
            var tableInfo = await _context.TableInfo.FindAsync(id);
            if (tableInfo != null)
            {
                _context.TableInfo.Remove(tableInfo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TableInfoExists(int id)
        {
            return (_context.TableInfo?.Any(e => e.TableId == id)).GetValueOrDefault();
        }
    }
}
