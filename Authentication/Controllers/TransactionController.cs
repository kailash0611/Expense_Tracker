using Microsoft.AspNetCore.Mvc;
using Authentication.Models;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Authorization;

namespace Authentication.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDBContext _dbContext;
        public TransactionController(ApplicationDBContext dBContext) { 
        _dbContext = dBContext;
        }

        [NonAction]
        public string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        [NonAction]
        public void PopulateCategories()
        {
            var CategoryList = _dbContext.Categories.Where(c=>c.UserId == GetUserId()).ToList();
            Category FirstCategory = new Category() { CategoryId = 0, Title="Choose a Category" };
            CategoryList.Insert(0, FirstCategory);
            ViewBag.Categories = CategoryList;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var transactionList = await _dbContext.Transactions.Where(t => t.UserId == GetUserId()).Include(x => x.Category).Include(x => x.user).ToListAsync();
            return View(transactionList);
        }

        [Authorize]
        public IActionResult AddOrEdit(int id = 0)
        {
            PopulateCategories();
            if (id == 0)
            {
                return View(new Transaction());
            }
            return View(_dbContext.Transactions.Find(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddOrEdit(Transaction transaction)
        {
            String userId = GetUserId();
            transaction.UserId = userId;

            User newUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
            transaction.user = newUser;

            Category newCategory = await _dbContext.Categories.FirstOrDefaultAsync(x => x.CategoryId == transaction.CategoryId);
            transaction.Category = newCategory;


            if (ModelState.IsValid)
            {

                if (transaction.TransactionId == 0)
                {
                    _dbContext.Transactions.Add(transaction);
                }
                else
                {
                    _dbContext.Update(transaction);
                }
                await _dbContext.SaveChangesAsync();
                return (RedirectToAction("Index"));
            }

            PopulateCategories();
            return View(transaction);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if(_dbContext.Transactions== null)
            {
                ViewData["DeleteError"] = "Cannot delete anything because there is no transaction";
                return View();
            }
            var transaction = await _dbContext.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _dbContext.Transactions.Remove(transaction);
            }
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
