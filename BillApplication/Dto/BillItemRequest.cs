namespace BillApplication.Dto
{
    public class BillItemRequest
    {
        public decimal Total { get; set; }
        public decimal Price { get; set; }
        public int BillID { get; set; }
        public int ProductId { get; set; }
        public int Kolicina { get; set; }
    }
}
