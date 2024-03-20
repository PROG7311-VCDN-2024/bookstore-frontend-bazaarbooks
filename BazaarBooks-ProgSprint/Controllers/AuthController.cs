using BazaarBooks_ProgSprint.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Firebase.Auth;
using BazaarBooks_ProgSprint.Logger;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace BazaarBooks_ProgSprint.Controllers
{

    public class AuthController : Controller
    {
        private ILog ilog;
        FirebaseAuthProvider auth;
        private readonly IHttpContextAccessor accessContext;
        BazaarBooksDbContext _context = new BazaarBooksDbContext();
       
        public AuthController(BazaarBooksDbContext context, IHttpContextAccessor httpContextAccessor)
        {

            auth = new FirebaseAuthProvider(new FirebaseConfig(Environment.GetEnvironmentVariable("BazaarBooks")));
           accessContext = httpContextAccessor;
            _context = context;
            ilog = Log.GetInstance();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(LoginModel login, string? email, string? firstname, string? lastname, string? password, string? confirmPass)
        {
            if (accessContext.HttpContext.Session.GetString("currentUser") != null)
            {ViewBag.Error = "A user is already logged in"; return View();}// end user already logged in
            else if (email == null)
            { ViewBag.Error = "Please enter First Name"; return View(); }// end user already logged in
            else if (firstname == null)
            { ViewBag.Error = "Please enter First Name"; return View(); }// end user already logged in
            else if (lastname == null)
            { ViewBag.Error = "Please enter Last Name"; return View(); }// end user already logged in
            else if (password == null)
            { ViewBag.Error = "Please enter Password"; return View(); }// end user already logged in
            else if (confirmPass == null)
            { ViewBag.Error = "Please confirm password"; return View(); }// end user already logged in
            else if (! password.Equals(confirmPass))
            { ViewBag.Error = "Password must match confirmed password"; return View(); }// end user already logged in

            else { 
            try
            {
                await auth.CreateUserWithEmailAndPasswordAsync(login.Email, login.Password);

                var fbAuthLink = await auth.SignInWithEmailAndPasswordAsync(login.Email, login.Password);
                string currentUserId = fbAuthLink.User.LocalId;

                if (currentUserId != null)
                {
                    accessContext.HttpContext.Session.SetString("currentUser", currentUserId);

                        BazaarBooks_ProgSprint.Models.User newUser = new BazaarBooks_ProgSprint.Models.User
                        {
                            Uuid = currentUserId,
                            Email = email,
                            FirstName =firstname, 
                            LastName =lastname,
                        };

                        _context.Users.Add(newUser);
                        _context.SaveChanges();

                    return RedirectToAction("Index", "Book");
                }// end if 

            }// end try firebase
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseErrorModel>(ex.ResponseData);
                ModelState.AddModelError(String.Empty, firebaseEx.error.message);
                return View(login);
            }// end catch 

            }//end

            return View();

        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel login, string email)
        {
            if (accessContext.HttpContext.Session.GetString("currentUser") == null)
            {
                try
                {
                    var fbAuthLink = await auth.SignInWithEmailAndPasswordAsync(login.Email, login.Password);
                    string currentUserId = fbAuthLink.User.LocalId;

                    if (currentUserId != null)
                    {
                        accessContext.HttpContext.Session.SetString("currentUser", currentUserId);
                        BazaarBooks_ProgSprint.Models.User user = _context.Users.Where(id => id.Uuid.Equals(currentUserId)).FirstOrDefault();
                        TempData["userName"]= user.FirstName + " " + user.LastName;
                        return RedirectToAction("Index", "Book");
                    }// end if 
                    else
                    {
                        ViewBag.Error = "Incorrect details, Please try again";
                        ilog.LogException("Login Failed" + email);
                        return View();
                    }

                }// end try
                catch (FirebaseAuthException ex)
                {
                    var firebaseEx = JsonConvert.DeserializeObject<FirebaseErrorModel>(ex.ResponseData);
                    ModelState.AddModelError(String.Empty, firebaseEx.error.message);
                    Utils.AuthLogger.Instance.LogError(firebaseEx.error.message + " - User: " + login.Email + " - IP: " + HttpContext.Connection.RemoteIpAddress
                        + " - Browser: " + Request.Headers.UserAgent);
                    return View(login);
                }// end catch

            }// end if
            else 
            {ViewBag.Error = "A User is already logged in";  return View();}

        }

        [HttpGet]
        public IActionResult LogOut()
        {
            accessContext.HttpContext.Session.Clear();
            return View();
        }

      
        public IActionResult Books() 
        { return RedirectToAction("Index", "Book"); }


    }

}// end 
