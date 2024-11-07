using BillApplication.Interface;
using BillApplication.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BillApplication.Repository
{
    public class StavkeRepository : IStavkeRepository
    {
        private readonly BillContext _context;

        public StavkeRepository(BillContext context)
        {
            _context = context;
        }
        public bool BillItemExists(int itemid)
        {
            return _context.BillItems.Any(a => a.ItemsID == itemid);
        }

        public bool CreateBillItem(Stavke billItem)
        {
            throw new NotImplementedException();
        }

        public Stavke GetItem(int id)
        {
            return _context.BillItems.Where(a => a.ItemsID == id).FirstOrDefault();
        }

        public ICollection<Stavke> GetItems()
        {
            return _context.BillItems.OrderBy(a => a.ItemsID).ToList();
           
        }

       


        public IEnumerable<Proizvod> GetProizvodByItemsID(int itemsID)
        {
            return _context.Products
             .Where(p => p.Items.Any(s => s.ItemsID == itemsID)) 
             .ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
        public bool CreateStavke(Stavke stavke)
        {
            _context.Add(stavke);
            return Save();
        }
        public bool UpdateStavke(Stavke stavke)
        {
            _context.Update(stavke);
            return Save();
        }
        public bool DeleteStavka(Stavke stavke)
        {
            _context.Remove(stavke);
            return Save();
        }

    }
}
