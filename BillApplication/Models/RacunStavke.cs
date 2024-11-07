using BillApplication.Dto;
using BillApplication.Models;

namespace BillApplication.Models
{
    public class RacunStavke
    {
        public int BillID { get; set; }
        public int ItemsID { get; set; }
        public Racun Bill { get; set; }
       
    }
}
