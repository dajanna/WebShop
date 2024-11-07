

using BillApplication.Models;

namespace BillApplication.Interface
{
    public interface IProizvodRepository
    {
        ICollection<Proizvod> GetProducts();
        Proizvod GetProduct(int productId);
        bool ProductExists(int productId);
        ICollection<Stavke> GetBillItems(int productId);
        void CreateProduct(string name, decimal price, string active);
        void UpdateProduct(int productId, string name, decimal price, string active);
        bool Save();
        Task <IReadOnlyList<Proizvod>> GetAll(string? name, string? sort);
       
    }
}
