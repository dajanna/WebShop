namespace BillApplication.Dto
{
    public class BulkInsertRequest
    {
        public List<BillRequest> Bills { get; set; }
        public List<BillItemRequest> BillItems { get; set; }
    }
}
