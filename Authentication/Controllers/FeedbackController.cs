using Authentication.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Authentication.Models;

namespace Expense_Tracker.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly ApplicationDBContext _context;

        public FeedbackController(ApplicationDBContext context)
        {
            _context = context;
        }
  
        public string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        public ActionResult Create()
        {
            return View(new Feedback());
        }

        [HttpPost]

        public async Task<ActionResult> Create(Feedback model)
        {
            if (ModelState.IsValid)
            {

                model.UserId = GetUserId();
                _context.Feedbacks.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Form submitted successfully!";
                return RedirectToAction("Create");
            }
            return View();
        }
           
        }
    }
