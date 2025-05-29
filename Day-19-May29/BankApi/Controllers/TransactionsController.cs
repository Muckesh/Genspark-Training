using BankApi.Models;
using BankApi.Models.DTOs;
using BankApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            try
            {
                var transactions = await _transactionService.GetAllTransactions();
                return Ok(transactions);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("deposit/")]
        public async Task<ActionResult<Transaction>> Deposit([FromBody] DepositDto depositDto)
        {
            try
            {
                var deposit = await _transactionService.DepositAmount(depositDto);
                return Created("", deposit);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("withdraw/")]
        public async Task<ActionResult<Transaction>> Withdraw([FromBody] WithdrawDto withdrawDto)
        {
            try
            {
                var withdraw = await _transactionService.WithdrawAmount(withdrawDto);
                return Created("", withdraw);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("transfer/")]
        public async Task<ActionResult<Transaction>> Transfer([FromBody] TransferDto transferDto)
        {
            try
            {
                var transfer = await _transactionService.TransferAmount(transferDto);
                return Created("", transfer);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}