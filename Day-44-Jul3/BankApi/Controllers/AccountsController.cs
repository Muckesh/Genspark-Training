using BankApi.Interfaces;
using BankApi.Models;
using BankApi.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("all/")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            try
            {
                var accounts = await _accountService.GetAllAccounts();
                return Ok(accounts);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("active/")]
        public async Task<ActionResult<IEnumerable<Account>>> GetActiveAccounts()
        {
            try
            {
                var accounts = await _accountService.GetAllAccounts();
                accounts = accounts.Where(a => a.Status == "Active");
                return Ok(accounts);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("inactive/")]
        public async Task<ActionResult<IEnumerable<Account>>> GetInactiveAccounts()
        {
            try
            {
                var accounts = await _accountService.GetAllAccounts();
                accounts = accounts.Where(a => a.Status == "Inactive");
                return Ok(accounts);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Account>> AddAccount([FromBody] AccountAddDto accountAddDto)
        {
            try
            {
                var newAccount = await _accountService.CreateAccount(accountAddDto);
                return Created("", newAccount);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("delete/")]
        public async Task<ActionResult<Account>> Delete(Guid accountId)
        {
            try
            {
                var account = await _accountService.DeleteAccount(accountId);
                if (account == null)
                {
                    throw new Exception("Unable to delete the account.");
                }
                return Ok(account);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
    }
}