

using BillApplication.Models;

namespace BillApplication.Interface
{
    public interface IStatusRepository
    {
        ICollection<Status> GetListOfStatus();
        Status GetStatus(int id);

        bool BillStatusExists(int id);
       
       
        bool Save();
        IEnumerable<Racun> GetBillsByStatus(int statusId);
        void InsertStatus(string name);
        void UpdateStatus(int statusId,string name);
       


    }
}
