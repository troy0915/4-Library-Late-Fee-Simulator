using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Late_Fee_Simulator
{
    public class LibraryItem
    {
        public string Type { get; }
        public DateTime DueDate { get; }
        public DateTime ReturnDate { get; }

        private const int AmnestyDays = 2;
        private static readonly Dictionary<string, decimal> FeePerDay = new Dictionary<string, decimal>
    {
        { "book", 0.50m },
        { "DVD", 1.00m },
        { "mag", 0.75m }
    };

        private const decimal MaxFeeCap = 10.00m;

        public LibraryItem(string type, string dueDate, string returnDate)
        {
            Type = type.ToLower();
            DueDate = DateTime.ParseExact(dueDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            ReturnDate = DateTime.ParseExact(returnDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        public int LateDays()
        {
            if (ReturnDate <= DueDate)
            {
                return 0; 
            }

            int lateDays = (ReturnDate - DueDate).Days;
            return lateDays > AmnestyDays ? lateDays - AmnestyDays : 0; 
        }

        public decimal Fee()
        {
            int lateDays = LateDays();
            if (lateDays <= 0)
            {
                return 0.00m; 
            }

            decimal fee = lateDays * FeePerDay[Type];
            return fee > MaxFeeCap ? MaxFeeCap : fee; 
        }
    }
    public class Library
    {
        private List<LibraryItem> items = new List<LibraryItem>();

        public void AddItem(string type, string dueDate, string returnDate)
        {
            items.Add(new LibraryItem(type, dueDate, returnDate));
        }

        public void SummarizeFees()
        {
            decimal totalFees = 0;
            string highestOffender = null;
            decimal highestFee = 0;

            foreach (var item in items)
            {
                decimal fee = item.Fee();
                totalFees += fee;

                if (fee > highestFee)
                {
                    highestFee = fee;
                    highestOffender = $"{item.Type} (Due: {item.DueDate.ToShortDateString()}, Returned: {item.ReturnDate.ToShortDateString()})";
                }

                Console.WriteLine($"Item Type: {item.Type}, Late Days: {item.LateDays()}, Fee: {fee:C}");
            }

            Console.WriteLine($"Total Fees: {totalFees:C}");
            if (highestOffender != null)
            {
                Console.WriteLine($"Highest Offender: {highestOffender} with Fee: {highestFee:C}");
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Library library = new Library();
         
            library.AddItem("book", "2023-10-01", "2023-10-05");
            library.AddItem("DVD", "2023-10-02", "2023-10-06");
            library.AddItem("mag", "2023-10-03", "2023-10-10");

            library.SummarizeFees();
        }
    }
}
