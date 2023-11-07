using Authentication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;

namespace Expense_Tracker.Controllers
{
    public class DashboardController : Controller
    {

        private readonly ApplicationDBContext _context;

        public DashboardController(ApplicationDBContext context)
        {
            _context = context;
        }
        public string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        [Authorize]
        public async Task<ActionResult> Index()
        {
            

                // Create an object of CultureInfo
                CultureInfo IndianCulture = new CultureInfo("en-IN");

                //Transactions of a particular user in Last 7 Days
                DateTime StartDate = DateTime.Today.AddDays(-6);
                DateTime EndDate = DateTime.Today;
                var SelectedTransactions = _context.Transactions.Include(t => t.Category)
                                                   .Include(t => t.user)
                                                   .Where((t => (t.Date >= StartDate && t.Date <= EndDate)))
                                                   .Where(u => u.UserId == GetUserId());


                //Total Income of 7 days
                int TotalIncome = SelectedTransactions
                                  .Where(t => t.Category.Type == "Income")
                                  .Sum(a => a.Amount);
                ViewBag.TotalIncome = TotalIncome.ToString("C0", IndianCulture);

                //Total Expense of 7 days
                int TotalExpense = SelectedTransactions
                                   .Where(t => t.Category.Type == "Expense")
                                   .Sum(a => a.Amount);
                ViewBag.TotalExpense = TotalExpense.ToString("C0", IndianCulture);

                //Balance 
                int Balance = TotalIncome - TotalExpense;
                ViewBag.Balance = String.Format(IndianCulture, "{0:C0}", Balance);


                // Expense By ModeOfPayment
                var ModeOfPaymentDoughnutChartData = SelectedTransactions.
                                                      GroupBy(x => x.ModeOfPayment)
                                                     .Select(x => new
                                                     {   amount = x.Sum(a => a.Amount),
                                                         ModeOfPayment = x.First().ModeOfPayment,
                                                     }).ToList();

                ViewBag.ModeOfPaymentDoughnutChartData = ModeOfPaymentDoughnutChartData;

                //Doughnut Chart - Expense By Category
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

                var IncomeDoughnutChartData = SelectedTransactions
                                   .Where(x => x.Category.Type == "Income")
                                   .GroupBy(y => y.CategoryId)
                                   .Select(z => new
                                   {
                                       categoryTitleWithIcon = z.First().Category.Icon + " " + z.First().Category.Title,
                                       amount = z.Sum(a => a.Amount),
                                       formattedAmount = z.Sum(a => a.Amount).ToString("C0")
                                   }).ToList();

                ViewBag.IncomeDoughnutChartData = IncomeDoughnutChartData;

                //Spline Chart - Income vs Expense

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

                // Getting the last 7 day dates 

                string[] Last7Days = Enumerable.Range(0, 7)
                    .Select(i => StartDate.AddDays(i).ToString("dd-MMM"))
                    .ToArray();

                //Combine Income & Expense
                ViewBag.SplineChartData = from day in Last7Days
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
                //Recent Transactions
                ViewBag.RecentTransactions = await _context.Transactions
                    .Where(i => i.UserId == GetUserId())
                    .Include(i => i.Category)
                    .OrderByDescending(j => j.Date)
                    .Take(10)
                    .ToListAsync();

                return View();
            }
            
        }
    }

    public class SplineChartData
    {
        public string day;
        public int income;
        public int expense;
    }

