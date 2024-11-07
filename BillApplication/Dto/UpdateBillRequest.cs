



namespace BillApplication.Dto
{
    public class UpdateBillRequest
    {
        public RacunDto Bill { get; set; }
        public List<StavkeDto1> BillItems { get; set; }
    }
}
