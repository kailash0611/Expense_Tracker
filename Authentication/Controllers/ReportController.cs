using Microsoft.AspNetCore.Mvc;
using Authentication.ViewModels;
using Microsoft.EntityFrameworkCore;
using Authentication.Models;
using System.Security.Claims;
using Expense_Tracker.Controllers;
using Rotativa;


namespace Authentication.Controllers
{
    public class ReportController : Controller
    {
        ApplicationDBContext _context;
        public ReportController(ApplicationDBContext context)
        {
            _context = context;
        }

        [NonAction]
        public string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        [HttpGet]
        public IActionResult Form()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Form(ReportViewModel model)
        {

            if (ModelState.IsValid)
            {
                DateTime StartDate = DateTime.Today.AddDays(-model.Days);
                DateTime EndDate = DateTime.Today;

                var SelectedTransactions = _context.Transactions.Include(t => t.Category)
                                                  .Include(t => t.user)
                                                  .Where((t => (t.Date >= StartDate && t.Date <= EndDate)))
                                                  .Where(u => u.UserId == GetUserId());

                // Doughnut Chart - Expense By Category

                var ExpenseDoughnutChartData = SelectedTransactions
                                    .Where(x => x.Category.Type == "Expense")
                                    .GroupBy(y => y.CategoryId)
                                    .Select(z => new
                                    {
                                        categoryTitleWithIcon = z.First().Category.Icon + " " + z.First().Category.Title,
                                        amount = z.Sum(a => a.Amount),
                                        formattedAmount = z.Sum(a => a.Amount).ToString("C0")
                                    }).ToList();

                ViewBag.ExpenseDoughnutChartData = ExpenseDoughnutChartData;

                // Bar Chart - Transaction By Mode Of Payment
                var ModeOfPaymentBarChartData = SelectedTransactions
                                                     .GroupBy(x => x.ModeOfPayment)
                                                     .Select(x => new
                                                     {
                                                         categoryTitleWithIcon = x.First().Category.Icon + " " + x.First().Category.Title,
                                                         amount = x.Sum(a => a.Amount),
                                                         ModeOfPayment = x.First().ModeOfPayment,
                                                     }).ToList();

                ViewBag.ModeOfPaymentBarChartData = ModeOfPaymentBarChartData;

                //Recent Expense Transactions
                ViewBag.RecentExpenseTransactions = await _context.Transactions
                    .Where(i => (i.UserId == GetUserId() && i.Category.Type == "Expense"))
                    .Include(i => i.Category)
                    .OrderByDescending(j => j.Date)
                    .Take(10)
                    .ToListAsync();

                //Recent Income Transactions
                ViewBag.RecentIncomeTransactions = await _context.Transactions
                    .Where(i => (i.UserId == GetUserId() && i.Category.Type=="Income"))
                    .Include(i => i.Category)
                    .OrderByDescending(j => j.Date)
                    .Take(10)
                    .ToListAsync();


                //Spline Chart Income vs Expense

                //It calculates the amount of Income Added in one day of each seven day
                List<SplineChartData> IncomeSummary = SelectedTransactions
                                                       .Where(t => t.Category.Type == "Income")
                                                       .GroupBy(x => x.Date)
                                                       .Select(y => new SplineChartData()
                                                       {
                                                           day = y.First().Date.ToString("dd-MMM"),
                                                           income = y.Sum(t => t.Amount),
                                                       }
                    ).ToList();

                //It calculates the amount of Expenses Added in one day of each seven day
                List<SplineChartData> ExpenseSummary = SelectedTransactions
                                                       .Where(t => t.Category.Type == "Expense")
                                                       .GroupBy(x => x.Date)
                                                       .Select(y => new SplineChartData()
                                                       {
                                                           day = y.First().Date.ToString("dd-MMM"),
                                                           expense = y.Sum(t => t.Amount),
                                                       }).ToList();

                // Getting the last day dates 
                //DateTime date = new DateTime();
                //int endRange = date.Day  - StartDate.Day;
                string[] LastDays = Enumerable.Range(0, model.Days)
                    .Select(i => StartDate.AddDays(i).ToString("dd-MMM"))
                    .ToArray();

                //Combine Income & Expense
                ViewBag.SplineChartData = from day in LastDays
                                          join income in IncomeSummary on day equals income.day into dayIncomeJoined
                                          from income in dayIncomeJoined.DefaultIfEmpty()
                                          join expense in ExpenseSummary on day equals expense.day into expenseJoined
                                          from expense in expenseJoined.DefaultIfEmpty()
                                          select new
                                          {
                                              day = day,
                                              income = income == null ? 0 : income.income,
                                              expense = expense == null ? 0 : expense.expense,
                                          };

                return View("Report");
            }
            return View();
        }


      
    }
}


// Form --> Gives Days -->
// 