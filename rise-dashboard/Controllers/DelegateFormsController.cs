using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rise.Code.DataFetcher;
using rise.Data;
using rise.Models;
using rise.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace rise_dashboard.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class DelegateFormsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DelegateFormsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Set Cookie value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireTime"></param>
        private void SetCookie(string key, string value, int? expireTime)
        {
            CookieOptions option = new CookieOptions();

            if (expireTime.HasValue)
            {
                option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
            }
            else
            {
                option.Expires = DateTime.Now.AddMilliseconds(10);
            }

            Response.Cookies.Append(key, value, option);
        }



        // GET: DelegateForms
        [AllowAnonymous]
        public async Task<IActionResult> Index(string address, long balance = 1000)
        {
            DelegateFormsViewModel delegateviewModel;

            if (!String.IsNullOrEmpty(address))
            {
                SetCookie("addr", address, 100000000);
                ViewBag.SearchText = address;

                var delegate_account = DelegateResult.Current.Delegates.Where(x => x.Username.Contains(address.ToLower()) || x.Address == address).OrderBy(j => j.Username.Length).FirstOrDefault();

                if (delegate_account != null)
                {
                    address = delegate_account.Address;
                }

                delegateviewModel = new DelegateFormsViewModel
                {
                    DelegateForm = await _context.DelegateForms.ToListAsync(),
                    DelegateResult = DelegateResult.Current,
                    walletAccountResult = await WalletAccountFetcher.FetchRiseWalletAccount(address),
                    coinQuoteCol = CoinQuoteResult.Current
                };

                if (balance == 1000)
                {
                    balance = long.Parse(delegateviewModel.walletAccountResult.account.Balance) / 100000000;
                }
            }
            else
            {
                delegateviewModel = new DelegateFormsViewModel
                {
                    DelegateForm = await _context.DelegateForms.ToListAsync(),
                    DelegateResult = DelegateResult.Current,
                    coinQuoteCol = CoinQuoteResult.Current
                };
            }

            ViewBag.Balance = balance;

            return View(delegateviewModel);
        }



        // GET: DelegateForms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var delegateForm = await _context.DelegateForms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (delegateForm == null)
            {
                return NotFound();
            }

            return View(delegateForm);
        }

        // GET: DelegateForms/Create
        public IActionResult Create()
        {
            DelegateForm dl = new DelegateForm();
            dl.Payout_interval = 1;

            return View(dl);
        }



        // POST: DelegateForms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Address,Share,Payout_address,Min_payout,Payout_interval,Fees_covered,Contact,Contact_type,Notes")] DelegateForm delegateForm)
        {
            if (ModelState.IsValid)
            {
                _context.Add(delegateForm);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(delegateForm);
        }



        // GET: DelegateForms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var delegateForm = await _context.DelegateForms.FindAsync(id);
            if (delegateForm == null)
            {
                return NotFound();
            }
            return View(delegateForm);
        }



        // POST: DelegateForms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Address,Share,Payout_address,Min_payout,Payout_interval,Fees_covered,Contact,Contact_type,Notesa")] DelegateForm delegateForm)
        {
            if (id != delegateForm.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(delegateForm);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DelegateFormExists(delegateForm.Id))
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
            return View(delegateForm);
        }

        // GET: DelegateForms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var delegateForm = await _context.DelegateForms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (delegateForm == null)
            {
                return NotFound();
            }

            return View(delegateForm);
        }

        // POST: DelegateForms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var delegateForm = await _context.DelegateForms.FindAsync(id);
            _context.DelegateForms.Remove(delegateForm);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DelegateFormExists(int id)
        {
            return _context.DelegateForms.Any(e => e.Id == id);
        }
    }
}