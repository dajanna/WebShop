using BillApplication.Interface;
using BillApplication.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BillApplication.Repository
{
    public class StatusRepository : IStatusRepository
    {
        private readonly BillContext _context;
       

        private readonly string _connectionString;

        public StatusRepository(BillContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection"); // Uverite se da koristi tačan naziv
        }

        public bool BillStatusExists(int id)
        {
            return _context.BillStatuses.Any(c => c.StatusID == id);
        }

        public IEnumerable<Racun> GetBillsByStatus(int statusId)
        {
            return _context.Bills
                         .Where(r => r.StatusId == statusId)
                         .ToList();
        }

        public ICollection<Status> GetListOfStatus()
        {
            return _context.BillStatuses.ToList();
        }

        public Status GetStatus(int id)
        {
            return _context.BillStatuses.Where(a => a.StatusID == id).FirstOrDefault();
        }

       

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
       
      

        public void InsertStatus(string name)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("InUpBillStatus", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;



                    command.Parameters.AddWithValue("@Name", name);


                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateStatus(int statusId,string name)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("InUpBillStatus", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Dodavanje parametara za ažuriranje
                    command.Parameters.AddWithValue("@StatusID", statusId);
                    command.Parameters.AddWithValue("@Name", name);


                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

     
    }
}
