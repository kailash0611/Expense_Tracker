using Authentication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Authentication.ViewModel;
using Authentication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Authentication.Filters;

namespace Authentication.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDBContext _dbContext;

        public AccountController(UserManager<User> userManager,
                                      SignInManager<User> signInManager,
                                      ApplicationDBContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext; 
        }



        [RedirectAuthenticatedUserFilter]
        public IActionResult Login()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {


            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Find a user by email
            var User = await _userManager.FindByEmailAsync(model.Email);

            // if User is found
            if (User != null)
            {
                var isPasswordCorrect = await _userManager.CheckPasswordAsync(User, model.Password);

                if (isPasswordCorrect)
                {
                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }
                }
                TempData["Error"] = "Wrong Password, Please try Again";
                return View(model);
            }
            // If user not found
            TempData["Error"] = "Wrong Credentials. Please try again";
            return View(model);
        }


        [RedirectAuthenticatedUserFilter]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model, IFormFile profilePicture)
        {


            if (ModelState.IsValid)
            {

                if (profilePicture != null && profilePicture.Length > 0)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads/Profile_Picture/", profilePicture.FileName);
                    string[] FilePathArray = filePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    string NewFilePath = "~/" + string.Join("/", FilePathArray[^3..^0]);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await profilePicture.CopyToAsync(stream);
                    }

                    var user = new User()
                    {
                        UserName = model.Email,
                        Email = model.Email,
                    };


                    model.ProfilePictureURL = NewFilePath;
                    user.ProfilePictureURL = NewFilePath;

                    await _userManager.UpdateAsync(user);
                    var result = await _userManager.CreateAsync(user, model.Password);
                    

                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("index", "Dashboard");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    ViewBag.Errors = ModelState.ErrorCount;
                }
            }
            return View();
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {

            return View();
        }
    }
}
