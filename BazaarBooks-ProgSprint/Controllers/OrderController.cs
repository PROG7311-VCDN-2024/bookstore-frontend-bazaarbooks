using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BazaarBooks_ProgSprint.Models;

namespace BazaarBooks_ProgSprint.Controllers
{
    public class OrderController : Controller
    {
        private readonly BazaarBooksDbContext _context;
        private readonly IHttpContextAccessor accessContext;

        public OrderController(BazaarBooksDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            accessContext = httpContextAccessor;
        }

        // GET: Order
        public async Task<IActionResult> Index(string searchBy, string search)
        {
            var user = accessContext.HttpContext.Session.GetString("currentUser");
            
            if (user != null)
            {
                int level = _context.Users.Where(u => u.Uuid.Equals(user)).Select(u => u.Level).FirstOrDefault();

                if (level == 0)
                {
                    
                    if (searchBy == "Email")
                    {
                       
                        return View(_context.Orders.Join(_context.Users, s => s.Uuid, u => u.Uuid, (s, u) => new { Order = s, User = u })
                            .Where(s => s.User.Email.Contains(search) || search == null).Select(s => s.Order).ToList());
                    }
                    else if (searchBy == "Name")
                    {
                        return View(_context.Orders.Join(_context.Users, c => c.Uuid, u => u.Uuid, (c, u) => new
                        { Order = c, User = u }).Where(x => x.User.FirstName.Contains(search) || search == null).Select(x => x.Order).ToList());
                    }
                    else if (searchBy == "Surname")
                    {
                        return View(_context.Orders.Join(_context.Users, c => c.Uuid, u => u.Uuid, (c, u) => new
                        { Order = c, User = u }).Where(x => x.User.LastName.Contains(search) || search == null).Select(x => x.Order).ToList());
                    }
                    else
                    {
                        var orders = _context.Orders.Include(u => u.Uu).ToList();
                        return View(orders);
                    }
                }// if user is admin 
                else
                {
                    var orders = _context.Orders.Include(o => o.Uu).Where(o => o.Uuid.Equals(user)).ToList();
                    return View(orders);
                }//
            }// if 
            else
            {
                // if user is not logged in redirect them to the login page
                TempData["LoginFirst"] = "Please Login First";
                return RedirectToAction("Login", "Auth");
            }//
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user = accessContext.HttpContext.Session.GetString("currentUser");

            if (user != null)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var order = await _context.Orders
                    .Include(o => o.Uu)
                    .FirstOrDefaultAsync(m => m.OrderId == id);
                if (order == null)
                {
                    return NotFound();
                }

                return View(order);
            }
            else
            {
                // if user is not logged in redirect them to the login page
                TempData["LoginFirst"] = "Please Login First";
                return RedirectToAction("Login", "Auth");
            }//
        }

        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            var user = accessContext.HttpContext.Session.GetString("currentUser");
            int level = _context.Users.Where(u => u.Uuid.Equals(user)).Select(u => u.Level).FirstOrDefault();

            if (user != null)
            {
                if (level == 0)
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var order = await _context.Orders.FindAsync(id);
                    if (order == null)
                    {
                        return NotFound();
                    }
                    ViewData["Uuid"] = new SelectList(_context.Users, "Uuid", "Uuid", order.Uuid);
                    return View(order);
                }
                else
                {
                    ViewBag.Error = "Must Be ADMIN to Access";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                // if user is not logged in redirect them to the login page
                TempData["LoginFirst"] = "Please Login First";
                return RedirectToAction("Login", "Auth");
            }//
        }

        // POST: Order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,Uuid,PurchaseDate,Total")] Order order)
        {
            var user = accessContext.HttpContext.Session.GetString("currentUser");
            int level = _context.Users.Where(u => u.Uuid.Equals(user)).Select(u => u.Level).FirstOrDefault();

            if (user != null)
            {
                if (level == 0)
                {

                    if (id != order.OrderId)
                    {
                        return NotFound();
                    }

                    if (ModelState.IsValid)
                    {
                        try
                        {
                            _context.Update(order);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!OrderExists(order.OrderId))
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
                    ViewData["Uuid"] = new SelectList(_context.Users, "Uuid", "Uuid", order.Uuid);
                    return View(order);
                }
                else
                {
                    ViewBag.Error = "Must Be ADMIN to Access";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                // if user is not logged in redirect them to the login page
                TempData["LoginFirst"] = "Please Login First";
                return RedirectToAction("Login", "Auth");
            }//

        }

        // GET: Order/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var order = await _context.Orders
        //        .Include(o => o.Uu)
        //        .FirstOrDefaultAsync(m => m.OrderId == id);
        //    if (order == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(order);
        //}

        //// POST: Order/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var order = await _context.Orders.FindAsync(id);
        //    if (order != null)
        //    {
        //        _context.Orders.Remove(order);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
