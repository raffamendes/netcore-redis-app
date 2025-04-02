using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using ExpensesApi.Models;
using System.Data;
using StackExchange.Redis;
using System.Text.Json;


namespace ExpensesApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ExpensesController : ControllerBase
    {
        private readonly IDatabase _redisDb;
        private static int _nextId = 1; // To simulate auto-incrementing IDs

        public ExpensesController(IConnectionMultiplexer redis){
            _redisDb = redis.GetDatabase();
        }

        [HttpPost]
        public ActionResult<Expense> CreateExpense(Expense expense)
        {
            if (expense == null)
            {
                return BadRequest();
            }

            expense.Id = _nextId++; // Assign and increment the ID
            _redisDb.StringSet($"expense:{expense.Id}", JsonSerializer.Serialize(expense));
            return CreatedAtAction(nameof(GetExpenseById), new { id = expense.Id }, expense);
        }

        [HttpGet("{id}")]
        public ActionResult<Expense> GetExpenseById(int id)
        {
            string expenseJson = _redisDb.StringGet($"expense:{id}");
            if (expenseJson == null)
            {
                return NotFound();
            }
            return JsonSerializer.Deserialize<Expense>(expenseJson);
        }

        [HttpPut]
        public IActionResult UpdateExpense(Expense updatedExpense)
        {
            if (updatedExpense == null)
            {
                return BadRequest("Expense object is null.");
            }

            string existingExpenseJson = _redisDb.StringGet($"expense:{updatedExpense.Id}");
            if (existingExpenseJson == null)
            {
                return NotFound();
            }

            _redisDb.StringSet($"expense:{updatedExpense.Id}", JsonSerializer.Serialize(updatedExpense));

            return NoContent();
        }

        [HttpDelete]
        public IActionResult DeleteExpense(Expense expenseToDelete)
        {
            if (expenseToDelete == null)
            {
                return BadRequest("Expense object is null.");
            }

            string expenseToRemoveJson = _redisDb.StringGet($"expense:{expenseToDelete.Id}");

            if (expenseToRemoveJson == null)
            {
                return NotFound();
            }

            _redisDb.KeyDelete($"expense:{expenseToDelete.Id}");
            return NoContent();
        }

        [HttpGet]
        public ActionResult<IEnumerable<Expense>> GetAllExpenses()
        {
            var expenseKeys = _redisDb.Multiplexer.GetServer(_redisDb.Multiplexer.GetEndPoints().First()).Keys(pattern: "expense:*");
            var expenses = new List<Expense>();

            foreach(var key in expenseKeys){
                string expenseJson = _redisDb.StringGet(key);
                if(!string.IsNullOrEmpty(expenseJson)){
                    expenses.Add(JsonSerializer.Deserialize<Expense>(expenseJson));
                }
            }
            return expenses;
        }
    }

    
}