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
    public class BookController : Controller
    {
        private readonly BazaarBooksDbContext _context;
        private readonly IHttpContextAccessor accessContext;

        public BookController(BazaarBooksDbContext context, IHttpContextAccessor accessContext)
        {
            _context = context;
            this.accessContext = accessContext;
        }


        [HttpPost]
        public IActionResult AddToCart(string id)
        {
            if (accessContext.HttpContext.Session.GetString("currentUser") != null)
            {
                if (id == null)
                {
                    return NotFound();
                }// end not found

                string user = accessContext.HttpContext.Session.GetString("currentUser");

                // create view to select quantity 

                ShoppingCart newCart = new ShoppingCart
                {
                    Uuid = user,
                    Isbn = id,
                    Quantity = 1,
                    IsPurchased = false

                }; // create new cart

                _context.ShoppingCarts.Add(newCart);
                _context.SaveChanges();

                var bookTitle = (from Book in _context.Books where Book.Isbn.Equals(id) select Book.Title).FirstOrDefault();

                // create temp data to hold display message for user
                TempData["BookAdded"] = $"{bookTitle} Successfully Added To Cart";
                return RedirectToAction("Index");
            }
            else
            {
                // if user is not logged in redirect them to the login page
                TempData["LoginFirst"] = "Please Login First";
                return RedirectToAction("Login", "Auth");
            }//

        }// end add to cart

        // GET: Book
        public async Task<IActionResult> Index(string searchBy, string search)
        {
            if (searchBy == "Genre")
            {
                return View(_context.Books.Where(x => x.Genre.Contains(search) || search == null).ToList());
            }
            else if (searchBy == "Author")
            {
                return View(_context.Books.Where(x => x.Author.Contains(search) || search == null).ToList());
            }
            else if (searchBy == "Title")
            {
                return View(_context.Books.Where(x => x.Title.Contains(search) || search == null).ToList());
            }
            else
            {
                return View(_context.Books.Where(x => x.Isbn.Contains(search) || search == null).ToList());
            }
          
        }

        // GET: Book/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Isbn == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Book/Create
        public IActionResult Create()
        {
            string user = accessContext.HttpContext.Session.GetString("currentUser");
            int level = _context.Users.Where(u => u.Uuid.Equals(user)).Select(u => u.Level).FirstOrDefault();

            if (level ==0)
            { return View(); }
            else
            {
                ViewBag.Error = "Must Be ADMIN to Access";
                return RedirectToAction("Index");
            }
           
        }

        // POST: Book/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Isbn,Title,Description,Price,Author,Genre,AvailableQuantity,ImageUrl")] Book book)
        {
            string user = accessContext.HttpContext.Session.GetString("currentUser");
            int level = _context.Users.Where(u => u.Uuid.Equals(user)).Select(u => u.Level).FirstOrDefault();

            if (level == 0)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(book);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(book);
            }
            else
            {
                ViewBag.Error = "Must Be ADMIN to Access";
                return RedirectToAction("Index");
            }

        }

        // GET: Book/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            string user = accessContext.HttpContext.Session.GetString("currentUser");
            int level = _context.Users.Where(u => u.Uuid.Equals(user)).Select(u => u.Level).FirstOrDefault();

            if (level == 0)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return NotFound();
                }
                return View(book);
            }
            else
            {
                ViewBag.Error = "Must Be ADMIN to Access";
                return RedirectToAction("Index");
            }

        }

        // POST: Book/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Isbn,Title,Description,Price,Author,Genre,AvailableQuantity,ImageUrl")] Book book)
        {
            string user = accessContext.HttpContext.Session.GetString("currentUser");
            int level = _context.Users.Where(u => u.Uuid.Equals(user)).Select(u => u.Level).FirstOrDefault();
            
            if (level == 0) 
            {
                if (id != book.Isbn)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(book);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!BookExists(book.Isbn))
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
                return View(book);
            }
            else
            {
                ViewBag.Error = "Must Be ADMIN to Access";
                return RedirectToAction("Index");
            }

        }

        // GET: Book/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            string user = accessContext.HttpContext.Session.GetString("currentUser");
            int level = _context.Users.Where(u => u.Uuid.Equals(user)).Select(u => u.Level).FirstOrDefault();
            
            if (level == 0)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var book = await _context.Books
                    .FirstOrDefaultAsync(m => m.Isbn == id);
                if (book == null)
                {
                    return NotFound();
                }

                return View(book);
            }
            else
            {
                ViewBag.Error = "Must Be ADMIN to Access";
                return RedirectToAction("Index");
            }

        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            string user = accessContext.HttpContext.Session.GetString("currentUser");
            int level = _context.Users.Where(u => u.Uuid.Equals(user)).Select(u => u.Level).FirstOrDefault();

            if (level == 0) 
            {
                var book = await _context.Books.FindAsync(id);
                if (book != null)
                {
                    _context.Books.Remove(book);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewBag.Error = "Must Be ADMIN to Access";
                return RedirectToAction("Index");
            }

        }

        private bool BookExists(string id)
        {
            return _context.Books.Any(e => e.Isbn == id);
        }
    }
}
