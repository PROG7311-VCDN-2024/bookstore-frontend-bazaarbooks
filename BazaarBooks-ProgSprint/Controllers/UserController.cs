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
    public class UserController : Controller
    {
        private readonly BazaarBooksDbContext _context;
        private readonly IHttpContextAccessor accessContext;

        public UserController(BazaarBooksDbContext context,IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            accessContext = httpContextAccessor;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            var user = accessContext.HttpContext.Session.GetString("currentUser");
            int level = _context.Users.Where(u => u.Uuid.Equals(user)).Select(u => u.Level).FirstOrDefault();

            if (user != null)
            {
                if (level ==0)
                {
                    return View(await _context.Users.ToListAsync());
                }
                else
                {
                    ViewBag.Error = "Must Be ADMIN to Access";
                    return RedirectToAction("Index", "Book");
                }
            }
            else
            {
                ViewBag.Error = "Login First";
                return RedirectToAction("Login", "Auth");
            }
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(string id)
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

                    var users = await _context.Users
                        .FirstOrDefaultAsync(m => m.Uuid == id);
                    if (users == null)
                    {
                        return NotFound();
                    }

                    return View(users);
                }
                else
                {
                    ViewBag.Error = "Must Be ADMIN to Access";
                    return RedirectToAction("Index", "Book");
                }
            }
            else
            {
                ViewBag.Error = "Login First";
                return RedirectToAction("Login", "Auth");
            }
        }

        // GET: User/Create
        public IActionResult Create()
        {
            var user = accessContext.HttpContext.Session.GetString("currentUser");
            int level = _context.Users.Where(u => u.Uuid.Equals(user)).Select(u => u.Level).FirstOrDefault();

            if (user != null)
            {
                if (level == 0)
                { return View(); }
                else
                {
                    ViewBag.Error = "Must Be ADMIN to Access";
                    return RedirectToAction("Index", "Book");
                }
            }
            else
            {
                ViewBag.Error = "Login First";
                return RedirectToAction("Login", "Auth");
            }

        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Uuid,Email,FirstName,LastName,Password,Level")] User user)
        {
            var users = accessContext.HttpContext.Session.GetString("currentUser");
            int level = _context.Users.Where(u => u.Uuid.Equals(users)).Select(u => u.Level).FirstOrDefault();

            if (users != null)
            {
                if (level == 0)
                {
                    if (ModelState.IsValid)
                    {
                        _context.Add(user);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    return View(user);
                }
                else
                {
                    ViewBag.Error = "Must Be ADMIN to Access";
                    return RedirectToAction("Index", "Book");
                }
            }
            else
            {
                ViewBag.Error = "Login First";
                return RedirectToAction("Login", "Auth");
            }


        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var users = accessContext.HttpContext.Session.GetString("currentUser");
            int level = _context.Users.Where(u => u.Uuid.Equals(users)).Select(u => u.Level).FirstOrDefault();

            if (users != null)
            {
                if (level == 0)
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var user = await _context.Users.FindAsync(id);
                    if (user == null)
                    {
                        return NotFound();
                    }
                    return View(user);
                }
                else
                {
                    ViewBag.Error = "Must Be ADMIN to Access";
                    return RedirectToAction("Book", "Inde");
                }
            }
            else
            {
                ViewBag.Error = "Login First";
                return RedirectToAction("Login", "Auth");
            }
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Uuid,Email,FirstName,LastName,Password,Level")] User user)
        {
            var users = accessContext.HttpContext.Session.GetString("currentUser");
            int level = _context.Users.Where(u => u.Uuid.Equals(users)).Select(u => u.Level).FirstOrDefault();

            if (users != null)
            {
                if (level == 0)
                {
                    if (id != user.Uuid)
                    {
                        return NotFound();
                    }

                    if (ModelState.IsValid)
                    {
                        try
                        {
                            _context.Update(user);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!UserExists(user.Uuid))
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
                    return View(user);
                }
                else
                {
                    ViewBag.Error = "Must Be ADMIN to Access";
                    return RedirectToAction("Index", "Book");
                }
            }
            else
            {
                ViewBag.Error = "Login First";
                return RedirectToAction("Login", "Auth");
            }
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var users = accessContext.HttpContext.Session.GetString("currentUser");
            int level = _context.Users.Where(u => u.Uuid.Equals(users)).Select(u => u.Level).FirstOrDefault();

            if (users != null)
            {
                if (level == 0)
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var user = await _context.Users
                        .FirstOrDefaultAsync(m => m.Uuid == id);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    return View(user);
                }
                else
                {
                    ViewBag.Error = "Must Be ADMIN to Access";
                    return RedirectToAction("Book", "Inde");
                }
            }
            else
            {
                ViewBag.Error = "Login First";
                return RedirectToAction("Login", "Auth");
            }
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var users = accessContext.HttpContext.Session.GetString("currentUser");
            int level = _context.Users.Where(u => u.Uuid.Equals(users)).Select(u => u.Level).FirstOrDefault();

            if (users != null)
            {
                if (level == 0)
                {
                    var user = await _context.Users.FindAsync(id);
                    if (user != null)
                    {
                        _context.Users.Remove(user);
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = "Must Be ADMIN to Access";
                    return RedirectToAction("Index", "Book");
                }
            }
            else
            {
                ViewBag.Error = "Login First";
                return RedirectToAction("Login", "Auth");
            }
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Uuid == id);
        }
    }
}
