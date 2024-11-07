namespace BillApplication.Dto
{
    public class ProizvodDto
    {
        public int? ProductID { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }
        public string Active { get; set; }
    }
}
