using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN231Project.Models;
using PRN231Project.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Data;

namespace PRN231Project.Controllers
{
    [Authorize(Roles = "Admin")]    
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ProjectPRN231Context _context;

        public AccountsController(ProjectPRN231Context context)
        {
            _context = context;
        }

        // GET: api/Accounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }



            return await _context.Accounts.ToListAsync();
        }
        [HttpGet("SearchAccount/{search}")]
        public async Task<ActionResult<IEnumerable<Account>>> SearchAccounts(string search)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }



            return await _context.Accounts.Where(x => x.Username.Contains(search)).ToListAsync();
        }

        // GET: api/Accounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(int id)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            var account = await _context.Accounts.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }

      // PUT: api/Accounts/5
      // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
      [HttpPut("UpdateAccount/{id}")]
      public async Task<IActionResult> UpdateAccount(int id, AccountDTO accountDTO)
      {
         if (id != accountDTO.AccountId)
         {
            return BadRequest("AccountId in path does not match AccountId in request body");
         }

         var account = await _context.Accounts.FindAsync(id);
         if (account == null)
         {
            return NotFound("Account not found");
         }

         // Cập nhật thông tin tài khoản từ dữ liệu DTO
         account.Username = accountDTO.Username;
         account.Password = accountDTO.Password;
         account.Name = accountDTO.Name;
         account.Address = accountDTO.Address;
         account.Email = accountDTO.Email;
         account.Phone = accountDTO.Phone;

         _context.Entry(account).State = EntityState.Modified;

         try
         {
            await _context.SaveChangesAsync();
         }
         catch (DbUpdateConcurrencyException)
         {
            if (!AccountExists(id))
            {
               return NotFound("Account not found");
            }
            else
            {
               throw;
            }
         }

         return NoContent();
      }
      // POST: api/Accounts
      // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
      [HttpPost("CreateAccountManager")]
        public async Task<ActionResult<Account>> CreateAccountManager(AccountDTO accountDTO)
        {
            if (_context.Accounts == null)
            {
                return Problem("Entity set 'ProjectPRN231Context.Accounts'  is null.");
            }
            var account = new Account
            {

                Username = accountDTO.Username,
                Password = accountDTO.Password,
                Name = accountDTO.Name,
                Address = accountDTO.Address,
                Phone = accountDTO.Phone,
                Email = accountDTO.Email,
                Status = 1,
                Role = "Admin"


            };
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAccount", new { id = account.AccountId }, account);
        }
        [HttpPost("CreateAccountCustomer")]
        public async Task<ActionResult<Account>> CreateAccountCustomer(AccountDTO accountDTO)
        {
            if (_context.Accounts == null)
            {
                return Problem("Entity set 'ProjectPRN231Context.Accounts'  is null.");
            }
            var account = new Account
            {
                Username = accountDTO.Username,
                Password = accountDTO.Password,
                Name = accountDTO.Name,
                Address = accountDTO.Address,
                Phone = accountDTO.Phone,
                Email = accountDTO.Email,
                Status = 1,
                Role = "User"


            };
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAccount", new { id = account.AccountId }, account);
        }

        // DELETE: api/Accounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            account.Status = 0;


            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool AccountExists(int id)
        {
            return (_context.Accounts?.Any(e => e.AccountId == id)).GetValueOrDefault();
        }


    }
}
