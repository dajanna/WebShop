using BillApplication.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace BillApplication.Interface
{
    public interface IStavkeRepository
    {
        ICollection<Stavke> GetItems();
        bool CreateBillItem(Stavke billItem);
        bool Save();
        Stavke GetItem(int id);
        bool BillItemExists(int itemid);
        // U IBillItemRepository
     


        IEnumerable<Proizvod>GetProizvodByItemsID(int itemsID);

        bool CreateStavke(Stavke stavke);
        bool UpdateStavke(Stavke stavke);
        bool DeleteStavka(Stavke stavke);


    }
}
