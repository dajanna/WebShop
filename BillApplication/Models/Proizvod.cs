namespace BillApplication.Models
{
    public class Proizvod
    {
        public int ProductID { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }
        

        public virtual ICollection<Stavke> Items { get; set; } = new List<Stavke>();
    }
}
