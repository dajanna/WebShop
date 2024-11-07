namespace BillApplication.Models
{
    public class Status
    {


        public int StatusID { get; set; }

        public string Name { get; set; } = null!;

        public ICollection<Racun> Bills { get; set; }
    }
}

