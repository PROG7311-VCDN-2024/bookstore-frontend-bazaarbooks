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
    public class ShoppingCartController : Controller
    {
        private readonly BazaarBooksDbContext _context;
        private readonly IHttpContextAccessor accessContext;

        public ShoppingCartController(BazaarBooksDbContext context, IHttpContextAccessor accessContext)
        {
            _context = context;
            this.accessContext = accessContext;
        }

        // GET: ShoppingCart
        public async Task<IActionResult> Index(string searchBy, string search)
        {
            var user = accessContext.HttpContext.Session.GetString("currentUser");
            int level = _context.Users.Where(u => u.Uuid.Equals(user)).Select(u => u.Level).FirstOrDefault();
            
            if (user != null)
            {
                if (level ==0)
                {
                    var shoppingCartItems = _context.ShoppingCarts.Include(s => s.IsbnNavigation).Include(s => s.Order).Include(s => s.Uu);

                    if (searchBy == "Email")
                    {
                        return View(_context.ShoppingCarts.Join(_context.Users, s => s.Uuid, u => u.Uuid, (s,u) => new {ShoppingCart = s, User = u})
                            .Where(s => s.User.Email.Contains(search) || search == null).Select(s => s.ShoppingCart).ToList());
                    }
                    else if (searchBy == "Name")
                    {
                        return View(_context.ShoppingCarts.Join(_context.Users, c => c.Uuid, u => u.Uuid, (c, u) => new
                        { ShoppingCart = c, User = u }).Where(x => x.User.FirstName.Contains(search) || search == null).Select(x => x.ShoppingCart).ToList());
                    }
                    else if (searchBy == "Surname")
                    {
                        return View(_context.ShoppingCarts.Join(_context.Users, c => c.Uuid, u => u.Uuid, (c, u) => new
                        { ShoppingCart = c, User = u }).Where(x => x.User.LastName.Contains(search) || search == null).Select(x => x.ShoppingCart).ToList());
                    }
                    else
                    {
                        return View(shoppingCartItems);
                    }
                }// end else if 
                else
                {
                    var shoppingCartItems = await _context.ShoppingCarts.Include(c => c.IsbnNavigation)
                        .Where(c => c.Uuid.Equals(user) && c.IsPurchased == false).ToListAsync();

                    var total = (from cartItem in _context.ShoppingCarts
                                 where
                                 cartItem.Uuid.Equals(user)
                                 join book in _context.Books on cartItem.Isbn equals book.Isbn
                                 select cartItem.Quantity * book.Price).Sum();

                    TempData["TotalPayment"] = $"Total Payment: R{total}";


                    return View(shoppingCartItems);
                }//

            }
            else
            {
                // if user is not logged in redirect them to the login page
                TempData["LoginFirst"] = "Please Login First";
                return RedirectToAction("Login", "Auth");
            }//

        }

        // GET: ShoppingCart/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user = accessContext.HttpContext.Session.GetString("currentUser");

            if (user != null)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var shoppingCart = await _context.ShoppingCarts
                    .Include(s => s.IsbnNavigation)
                    .Include(s => s.Order)
                    .Include(s => s.Uu)
                    .FirstOrDefaultAsync(m => m.CartItemId == id);
                if (shoppingCart == null)
                {
                    return NotFound();
                }

                return View(shoppingCart);
            }
            else
            {
                // if user is not logged in redirect them to the login page
                TempData["LoginFirst"] = "Please Login First";
                return RedirectToAction("Login", "Auth");
            }//
        }

        public IActionResult Payment()
        {
            if (accessContext.HttpContext.Session.GetString("currentUser") != null)
            { return RedirectToAction("Payment", "Payment");}
            else
            {
                // if user is not logged in redirect them to the login page
                TempData["LoginFirst"] = "Please Login First";
                return RedirectToAction("Login", "Auth");
            }//
        }

        
        // GET: ShoppingCart/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = accessContext.HttpContext.Session.GetString("currentUser");

            if (user != null)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var shoppingCart = await _context.ShoppingCarts.FindAsync(id);
                if (shoppingCart == null)
                {
                    return NotFound();
                }
                ViewData["Isbn"] = new SelectList(_context.Books, "Isbn", "Isbn", shoppingCart.Isbn);
                ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", shoppingCart.OrderId);
                ViewData["Uuid"] = new SelectList(_context.Users, "Uuid", "Uuid", shoppingCart.Uuid);
                return View(shoppingCart);
            }
            else
            {
                // if user is not logged in redirect them to the login page
                TempData["LoginFirst"] = "Please Login First";
                return RedirectToAction("Login", "Auth");
            }//

        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, int Quantity, string Isbn)
        {
            var user = accessContext.HttpContext.Session.GetString("currentUser");

            if (user != null)
            {

                ShoppingCart cart = _context.ShoppingCarts.Where(c => c.CartItemId == id).FirstOrDefault();
                
                if (cart != null)
                {
                    cart.Quantity = Quantity;
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                else
                { return NotFound(); }
            }
            else
            {
                // if user is not logged in redirect them to the login page
                TempData["LoginFirst"] = "Please Login First";
                return RedirectToAction("Login", "Auth");
            }

            return RedirectToAction(nameof(Index));

        }
        // POST: ShoppingCart/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("CartItemId,Uuid,OrderId,Isbn,Quantity,IsPurchased")] ShoppingCart shoppingCart)
        //{
        //    var user = accessContext.HttpContext.Session.GetString("currentUser");

        //    if (user != null)
        //    {
        //        if (id != shoppingCart.CartItemId)
        //        {
        //            return NotFound();
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            try
        //            {
        //                _context.Update(shoppingCart);
        //                await _context.SaveChangesAsync();
        //            }
        //            catch (DbUpdateConcurrencyException)
        //            {
        //                if (!ShoppingCartExists(shoppingCart.CartItemId))
        //                {
        //                    return NotFound();
        //                }
        //                else
        //                {
        //                    throw;
        //                }
        //            }
        //            return RedirectToAction(nameof(Index));
        //        }
        //        ViewData["Isbn"] = new SelectList(_context.Books, "Isbn", "Isbn", shoppingCart.Isbn);
        //        ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", shoppingCart.OrderId);
        //        ViewData["Uuid"] = new SelectList(_context.Users, "Uuid", "Uuid", shoppingCart.Uuid);
        //        return View(shoppingCart);
        //    }
        //    else
        //    {
        //        // if user is not logged in redirect them to the login page
        //        TempData["LoginFirst"] = "Please Login First";
        //        return RedirectToAction("Login", "Auth");
        //    }//

        //}

        // GET: ShoppingCart/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var user = accessContext.HttpContext.Session.GetString("currentUser");

            if (user != null)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var shoppingCart = await _context.ShoppingCarts
                    .Include(s => s.IsbnNavigation)
                    .Include(s => s.Order)
                    .Include(s => s.Uu)
                    .FirstOrDefaultAsync(m => m.CartItemId == id);
                if (shoppingCart == null)
                {
                    return NotFound();
                }

                return View(shoppingCart);
            }
            else
            {
                // if user is not logged in redirect them to the login page
                TempData["LoginFirst"] = "Please Login First";
                return RedirectToAction("Login", "Auth");
            }//

        }

        // POST: ShoppingCart/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shoppingCart = await _context.ShoppingCarts.FindAsync(id);
            if (shoppingCart != null)
            {
                _context.ShoppingCarts.Remove(shoppingCart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShoppingCartExists(int id)
        {
            return _context.ShoppingCarts.Any(e => e.CartItemId == id);
        }
    }
}
