namespace BillApplication.Models
{
    public class Racun
    {
        public int BillID { get; set; }

        public decimal Price { get; set; }

        public DateTime Date { get; set; }

        public virtual ICollection<Stavke> Items { get; set; } = new List<Stavke>();

        public int StatusId { get; set; }
        public Status Status { get; set; }
    }
}
