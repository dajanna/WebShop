namespace BillApplication.Dto
{
    public class BillRequest
    {
        public decimal Price { get; set; } // Bez BillId
        public DateTime Date { get; set; }
        public int StatusId { get; set; }
    }
}
