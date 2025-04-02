// Models/Expense.cs
namespace ExpensesApi.Models
{
    public class Expense
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }
    }
}