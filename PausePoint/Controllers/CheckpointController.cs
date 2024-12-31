using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PausePoint.Data;
using PausePoint.Models;

namespace PausePoint.Controllers
{
    [Authorize]
    public class CheckpointController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CheckpointController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Checkpoint
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var checkpoints = await _context.Checkpoint.Where(c => c.UserId == userId).ToListAsync();
            return View(checkpoints);
        }

        // GET: Checkpoint/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkpoint = await _context.Checkpoint
                .FirstOrDefaultAsync(m => m.Id == id);
            if (checkpoint == null)
            {
                return NotFound();
            }

            return View(checkpoint);
        }

        // GET: Checkpoint/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Checkpoint/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Link,Timestamp")] Checkpoint checkpoint)
        {
            ModelState.Remove("UserId"); // Remove model check error for user id during Bind
            
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the current user's ID
            checkpoint.UserId = userId; // Assign the UserId to the checkpoint object

            var timeStampInSeconds = checkpoint.Timestamp.TotalSeconds;
            // Check if the URL contains a timestamp already ("&t=")
            int index = checkpoint.Link.IndexOf("&t=");
            if (index != -1)
            {
                // Remove everything starting from "&t="
                checkpoint.Link = checkpoint.Link.Remove(index);
            }
            checkpoint.Link += $"&t={timeStampInSeconds}";
            
            if (ModelState.IsValid)
            {
                _context.Add(checkpoint);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(checkpoint);
        }

        // GET: Checkpoint/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkpoint = await _context.Checkpoint.FindAsync(id);
            if (checkpoint == null)
            {
                return NotFound();
            }
            return View(checkpoint);
        }

        // POST: Checkpoint/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Link,Timestamp")] Checkpoint checkpoint)
        {
            if (id != checkpoint.Id)
            {
                return NotFound();
            }
            
            ModelState.Remove("UserId"); // Remove model check error for user id during Bind
            
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the current user's ID
            checkpoint.UserId = userId; // Assign the UserId to the checkpoint object
            
            var timeStampInSeconds = checkpoint.Timestamp.TotalSeconds;
            // Check if the URL contains a timestamp already ("&t=")
            int index = checkpoint.Link.IndexOf("&t=");
            if (index != -1)
            {
                // Remove everything starting from "&t="
                checkpoint.Link = checkpoint.Link.Remove(index);
            }
            checkpoint.Link += $"&t={timeStampInSeconds}";

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(checkpoint);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CheckpointExists(checkpoint.Id))
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
            return View(checkpoint);
        }

        // GET: Checkpoint/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkpoint = await _context.Checkpoint
                .FirstOrDefaultAsync(m => m.Id == id);
            if (checkpoint == null)
            {
                return NotFound();
            }

            return View(checkpoint);
        }

        // POST: Checkpoint/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var checkpoint = await _context.Checkpoint.FindAsync(id);
            if (checkpoint != null)
            {
                _context.Checkpoint.Remove(checkpoint);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CheckpointExists(int id)
        {
            return _context.Checkpoint.Any(e => e.Id == id);
        }
    }
}
