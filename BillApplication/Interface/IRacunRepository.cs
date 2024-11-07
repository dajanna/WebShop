using BillApplication.Dto;
using BillApplication.Models;


namespace BillApplication.Interface
{
    public interface IRacunRepository
    {
        ICollection<Racun> GetBills();
       
        bool Save();
        Racun GetBill(int id);
        bool BillExists(int id);

        ICollection<Stavke> GetBillItemsbyBill(int billId);
        IEnumerable<Proizvod> GetProizvodiByRacunId(int racunId);
        void InsertBillAndBillItems( decimal total, DateTime date, decimal price, int productId, int quantity, int statusId, decimal totalItems);
        void UpdateBillAndBillItems(int billId, int itemsId, decimal total, DateTime date, decimal price, int productId, int quantity, int statusId, decimal totalItems);
        Task  <decimal> GetTotalAsync(int racunId);
        bool CreateBill(Racun racun);
        Status GetStatusByBillId(int billId);
        bool UpdateRacun(Racun racun);
        Task<List<int>> InsertBillsAsync(List<BillRequest> bills);
        Task InsertBillItemsAsync(List<BillItemRequest> billItems);
        Task<bool> UpdateBillAsync(RacunDto bill, List<StavkeDto1> billItems);
       
        Task<bool> DeleteBill(int billId);
        Task<bool> InsertBillAndItemsAsync(decimal price, DateTime date, int statusId, IEnumerable<BillItemRequest> billItems);
        void UpdateBillStatus(int billId, int newStatusId);
    }
}
