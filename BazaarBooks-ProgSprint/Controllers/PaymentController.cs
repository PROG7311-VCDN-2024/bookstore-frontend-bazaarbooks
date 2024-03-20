using BazaarBooks_ProgSprint.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BazaarBooks_ProgSprint.Controllers
{
    public class PaymentController : Controller
    {
        BazaarBooksDbContext _context = new BazaarBooksDbContext();
        private readonly IHttpContextAccessor accessContext;

        public PaymentController(BazaarBooksDbContext _context, IHttpContextAccessor httpContextAccessor) 
        {
            this._context = _context;  
            accessContext = httpContextAccessor;
        }

        public IActionResult Payment()
        {
            var user = accessContext.HttpContext.Session.GetString("currentUser");
            if(user != null) 
            {
                var total = (from cartItem in _context.ShoppingCarts
                             where
                cartItem.Uuid.Equals(user)
                             join book in _context.Books on cartItem.Isbn equals book.Isbn
                             select cartItem.Quantity * book.Price).Sum();

                TempData["TotalPayment"] = $"Total Payment: R{total}";
                return View();
            }
            else
            {
                // if user is not logged in redirect them to the login page
                TempData["LoginFirst"] = "Please Login First";
                return RedirectToAction("Login", "Auth");
            }//

        }

        public IActionResult ThankYou()
        {
            return View();
        }

        public IActionResult Logout()
        { return RedirectToAction("LogOut", "Auth"); }

        public IActionResult Books()
        { return RedirectToAction("Index", "Book"); }


        [HttpPost]
        public IActionResult Payment(string nameOnCard, string cardNumber, string cvv)
        {
            if (accessContext.HttpContext.Session.GetString("currentUser") != null)

            {
                string user = accessContext.HttpContext.Session.GetString("currentUser");


                if (nameOnCard == null)
                {
                    ViewBag.Error = "Please enter name";
                    return View();
                }
                else if (cardNumber == null)
                {
                    ViewBag.Error = "Please enter card number";
                    return View();
                }
                else if (cvv == null)
                {
                    ViewBag.Error = "Please enter cvv";
                    return View();
                }
                else
                {
                    string userLogged = accessContext.HttpContext.Session.GetString("currentUser");
                    var userCarts = _context.ShoppingCarts.Include(u => u.IsbnNavigation).Where(c => c.Uuid.Equals(userLogged) && c.IsPurchased == false).ToList();

                    var total = (from cartItem in _context.ShoppingCarts
                                 where
                    cartItem.Uuid.Equals(userLogged)
                                 join book in _context.Books on cartItem.Isbn equals book.Isbn
                                 select cartItem.Quantity * book.Price).Sum();

                    Order order = new Order
                    {
                        Uuid = userLogged,
                        PurchaseDate = DateTime.Now,
                        Total = total,

                    };

                    
                    _context.Orders.Add(order);
                    _context.SaveChanges();
                   

                    foreach (var cart in userCarts)
                    {
                        cart.OrderId = order.OrderId;
                        cart.IsPurchased = true;
                    }
                    _context.SaveChanges();

                    return RedirectToAction("ThankYou", "Payment");
                }

            }
            else
            {
                // if user is not logged in redirect them to the login page
                TempData["LoginFirst"] = "Please Login First";
                return RedirectToAction("Login", "Auth");
            }//

        }
    }
}
