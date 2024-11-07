using BillApplication.Interface;
using BillApplication.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BillApplication.Repository
{
    public class ProizvodRepository : IProizvodRepository
    {
        private readonly BillContext _context;
        private readonly string _connectionString;

        public ProizvodRepository(BillContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection"); // Uverite se da koristi tačan naziv
        }

        public ICollection<Stavke> GetBillItems(int productId)
        {
            return _context.BillItems.Where(a => a.ProductID == productId).ToList();
        }

        public Proizvod GetProduct(int productId)
        {
            return _context.Products.Where(a => a.ProductID == productId).FirstOrDefault();
        }

        public ICollection<Proizvod> GetProducts()
        {
            return _context.Products.OrderBy(a => a.ProductID).ToList();
        }

       

        public bool ProductExists(int productId)
        {
            return _context.Products.Any(a => a.ProductID == productId);
        }
        public void CreateProduct(string name, decimal price, string active)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("InUpProduct", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Dodavanje parametara za umetanje (ProductID će biti NULL)
                    command.Parameters.AddWithValue("@ProductID", DBNull.Value);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Price", Math.Round(price, 2));
                    command.Parameters.AddWithValue("@Active", active);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateProduct(int productId, string name, decimal price, string active)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("InUpProduct", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Dodavanje parametara za ažuriranje
                    command.Parameters.AddWithValue("@ProductID", productId);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Price", price);
                    command.Parameters.AddWithValue("@Active", active);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public async Task<IReadOnlyList<Proizvod>> GetAll(string? name, string? sort)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query=query.Where(x=> x.Name == name);

            query = sort switch
            {
                "priceAsc" => query.OrderBy(x => x.Price),
                "priceDesc" => query.OrderByDescending(x => x.Price),
                _ => query.OrderBy(x => x.Name)
            };

            return await query.ToListAsync();
        }
    }
}
