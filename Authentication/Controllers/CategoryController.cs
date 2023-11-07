using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Authentication.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Authentication.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDBContext _context;

        public CategoryController(ApplicationDBContext context)
        {
            _context = context;
        }

        [NonAction]
        public string GetUserId()
        { 
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        [NonAction]
        public IEnumerable<string> GetCategorTitle()
        {
            IEnumerable<string> TitleNames= _context.Categories.Where(c=>c.UserId == GetUserId()).Select(c => c.Title);
            return TitleNames;
        }


        // 
        [Authorize]
        public async Task<IActionResult> Index()
        {
            // Here we are using Include method to load all the data in one time. If Include is not used than 
            // it will load only the data related to the Category and not the navigation Property (user)
            var applicationDBContext = _context.Categories.Where(c=>c.UserId== GetUserId()).Include(c=>c.user);
            return View(await applicationDBContext.ToListAsync());
        }

        [HttpGet]
        [Authorize]
        public IActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
            {   
                return View(new Category());
                // We have to return a new object of Category since in AddOrEdit View we have a condition regarding the categoryId.
            }
            return View(_context.Categories.Find(id));
        }

        [HttpPost]

        public async Task<IActionResult> AddOrEdit(Category category)
        {
            // getting the userId of the current user
            string userId = GetUserId();
            category.UserId = userId;
            category.user = _context.Users.Find(userId);

            // storing category name in lower case
           string NewTitle = category.Title.ToLower();



            if (ModelState.IsValid)
            {
                if (category.CategoryId == 0)
                {
                    //Comparing the provided category Title with the all title present in the Category Table
                    IEnumerable<string> Titles = GetCategorTitle();
                    foreach(var Title in Titles)
                    {    
                       string lowerTitle = Title.ToLower();
                        if(NewTitle == lowerTitle)
                        {
                            TempData["Error"] = $"Category {category.Title} is already Defined";
                            return View(category);
                        }
                    }
                    _context.Add(category);
                }
                else
                {
                    _context.Update(category);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            
            var category = await _context.Categories.FindAsync(id);
            if(category!= null)
            {
                _context.Categories.Remove(category);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}

