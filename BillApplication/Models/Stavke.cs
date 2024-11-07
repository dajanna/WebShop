namespace BillApplication.Models
{
    public class Stavke
    {
        public int ItemsID { get; set; }

        public decimal Total { get; set; }
        public decimal Price { get; set; }

        public int BillID { get; set; }

        public int ProductID { get; set; }

        public virtual Racun Bill { get; set; } = null!;

        public virtual Proizvod Product { get; set; } = null!;
        public int Kolicina { get; set; }
    }
}
