using BillApplication.Dto;
using BillApplication.Interface;
using BillApplication.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BillApplication.Repository
{

    public class RacunRepository : IRacunRepository
    {
        private readonly BillContext _context;
        private readonly IDbConnection _dbconnection;
        private readonly string _connectionString;


        public RacunRepository(BillContext context, IConfiguration configuration, IDbConnection dbconnection)
        {
            _context = context;
            _dbconnection = dbconnection;
            _connectionString = configuration.GetConnectionString("DefaultConnection");

        }

        public bool BillExists(int id)
        {
            return _context.Bills.Any(a => a.BillID == id);
        }

        public Racun GetBill(int id)
        {
            return _context.Bills.Where(a => a.BillID == id).FirstOrDefault();
        }



        public ICollection<Stavke> GetBillItemsbyBill(int billId)
        {
            return _context.BillItems.Where(a => a.BillID == billId).ToList();
        }

        public ICollection<Racun> GetBills()
        {
            return _context.Bills.OrderBy(a => a.BillID).ToList();
        }




        public IEnumerable<Proizvod> GetProizvodiByRacunId(int racunId)
        {
            return _context.BillItems.Where(sr => sr.BillID == racunId).Select(sr => sr.Product).ToList();
        }

        public void InsertBillAndBillItems(decimal total, DateTime date, decimal price, int productId, int quantity, int statusId, decimal totalItems)
        {

            var parameters = new[]
            {
             new SqlParameter("@BillID", DBNull.Value), // Novi BillID (null za kreiranje)
             new SqlParameter("@ItemsID", DBNull.Value), // Novi ItemsID (null za kreiranje)
              new SqlParameter("@Total", total),
             new SqlParameter("@Date", date),
             new SqlParameter("@Price", price),
             new SqlParameter("@ProductID", productId),
             new SqlParameter("@Kolicina", quantity),
              new SqlParameter("@StatusId", statusId),
              new SqlParameter("@TotalItems", totalItems)
                 };

            _context.Database.ExecuteSqlRaw("EXEC InUpBillAndBillItems @BillID, @ItemsID, @Total, @Date, @Price, @ProductID, @Kolicina, @StatusId, @TotalItems", parameters);


        }
        public void UpdateBillAndBillItems(int billId, int itemsId, decimal total, DateTime date, decimal price, int productId, int quantity, int statusId, decimal totalItems)
        {
            var parameters = new[]
            {
         new SqlParameter("@BillID", billId), // Postojeći BillID
         new SqlParameter("@ItemsID", itemsId), // Postojeći ItemsID
         new SqlParameter("@Total", total),
         new SqlParameter("@Date", date),
         new SqlParameter("@Price", price),
         new SqlParameter("@ProductID", productId),
         new SqlParameter("@Kolicina", quantity),
         new SqlParameter("@StatusId", statusId),
         new SqlParameter("@TotalItems", totalItems)
    };

            _context.Database.ExecuteSqlRaw("EXEC InUpBillAndBillItems @BillID, @ItemsID, @Total, @Date, @Price, @ProductID, @Kolicina, @StatusId, @TotalItems", parameters);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public Status GetStatusByBillId(int billId)
        {
            return _context.Bills.Where(c => c.BillID == billId).Select(c => c.Status).FirstOrDefault();
        }

        public async Task<decimal> GetTotalAsync(int racunId)
        {
            var racun = await _context.Bills.Include(r => r.Items)
                                          .ThenInclude(s => s.Product)
                                          .FirstOrDefaultAsync(r => r.BillID == racunId);

            if (racun == null) throw new Exception("Racun ne postoji.");

            decimal total = 0;

            foreach (var stavka in racun.Items)
            {
                total += stavka.Kolicina * (stavka.Product?.Price ?? 0);

            }


            racun.Price = total;
            await _context.SaveChangesAsync();

            return total;
        }

        public bool CreateBill(Racun racun)
        {
            _context.Add(racun);
            return Save();
        }
        public bool UpdateRacun(Racun racun)
        {
            _context.Update(racun);
            return Save();
        }

        public async Task<List<int>> InsertBillsAsync(List<BillRequest> bills)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var billIds = new List<int>();

                var sql = "INSERT INTO Bill (Price, Date, StatusId) " +
                          "OUTPUT INSERTED.BillID " +
                          "VALUES (@Price, @Date, @StatusId);"; // Koristite OUTPUT za generisane ID-eve

                foreach (var bill in bills)
                {
                    var billId = await db.ExecuteScalarAsync<int>(sql, bill);
                    billIds.Add(billId);
                }

                return billIds;
            }
        }

        public async Task InsertBillItemsAsync(List<BillItemRequest> billItems)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var sql = "INSERT INTO BillItems (Total, Price, BillID, ProductID, Kolicina) " +
                          "VALUES (@Total, @Price, @BillID, @ProductID, @Kolicina);";

                // Ovdje dodajte BillId kao što ste već uradili u kontroleru
                await db.ExecuteAsync(sql, billItems);
            }
        }

        public async Task<bool> UpdateBillAsync(RacunDto bill, List<StavkeDto1> billItems)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // Otvorite konekciju
                await connection.OpenAsync();

                // Proverite trenutni status računa
                var currentStatus = await connection.QueryFirstOrDefaultAsync<int>(
                    "SELECT StatusId FROM Bill WHERE BillID = @BillID", new { BillID = bill.BillID });

                if (currentStatus != (int)BillStatus.Active)
                {
                    throw new InvalidOperationException("Only bills with status 'Active' can be updated.");
                }

                // Ažuriranje osnovnih informacija o računu
                var updateBillQuery = @"
        UPDATE Bill 
        SET Price = @Price, Date = @Date, StatusId = @StatusId
        WHERE BillID = @BillID";

                await connection.ExecuteAsync(updateBillQuery, bill);

                // Ažuriranje stavki računa
                foreach (var item in billItems)
                {
                    if (item.ItemsID > 0) // Ako stavka već postoji
                    {
                        var updateItemQuery = @"
                UPDATE BillItems 
                SET Total = @Total, Price = @Price, ProductID = @ProductID, Kolicina = @Kolicina
                WHERE ItemsID = @ItemsID AND BillID = @BillID";

                        await connection.ExecuteAsync(updateItemQuery, new
                        {
                            item.Total,
                            item.Price,
                            item.ProductID,
                            item.Kolicina,
                            item.ItemsID,
                            BillID = bill.BillID
                        });
                    }
                    else // Ako stavka ne postoji, dodajte je
                    {
                        var insertItemQuery = @"
                INSERT INTO BillItems (Total, Price, ProductID, Kolicina, BillID)
                VALUES (@Total, @Price, @ProductID, @Kolicina, @BillID)";

                        await connection.ExecuteAsync(insertItemQuery, new
                        {
                            item.Total,
                            item.Price,
                            item.ProductID,
                            item.Kolicina,
                            BillID = bill.BillID
                        });
                    }
                }

                return true;
            }
        }

   
        public async Task<bool> DeleteBill(int billId)
        {
            if (_dbconnection.State == ConnectionState.Closed)
            {
                _dbconnection.Open();
            }

            using (var transaction = _dbconnection.BeginTransaction())
            {
                try
                {
                    // Prvo brišemo sve stavke povezane sa računom
                    var deleteItemsQuery = "DELETE FROM BillItems WHERE BillID = @BillID";
                    await _dbconnection.ExecuteAsync(deleteItemsQuery, new { BillID = billId }, transaction);

                    // Zatim brišemo sam račun
                    var deleteBillQuery = "DELETE FROM Bill WHERE BillID = @BillID";
                    var affectedRows = await _dbconnection.ExecuteAsync(deleteBillQuery, new { BillID = billId }, transaction);

                    transaction.Commit(); // Potvrđujemo transakciju ako su oba upita uspešna

                    return affectedRows > 0; // Vraća true ako je brisanje računa uspešno
                }
                catch
                {
                    transaction.Rollback(); // Poništavamo transakciju u slučaju greške
                    throw; // Ponovno podižemo izuzetak
                }
                finally
                {
                    if (_dbconnection.State == ConnectionState.Open)
                    {
                        _dbconnection.Close();
                    }
                }
            }
        }
        public async Task<bool> InsertBillAndItemsAsync(decimal price, DateTime date, int statusId, IEnumerable<BillItemRequest> billItems)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var billItemTable = new DataTable();
                billItemTable.Columns.Add("Total", typeof(int));
                billItemTable.Columns.Add("Price", typeof(decimal));
                billItemTable.Columns.Add("BillID", typeof(int));
                billItemTable.Columns.Add("ProductID", typeof(int));
                billItemTable.Columns.Add("Kolicina", typeof(int));
               

                foreach (var item in billItems)
                {
                    billItemTable.Rows.Add(item.Total, item.Price,0, item.ProductId, item.Kolicina);
                }

                var parameters = new DynamicParameters();
                parameters.Add("@Price", price);
                parameters.Add("@Date", date);
                parameters.Add("@StatusId", statusId);
                parameters.Add("@BillItems", billItemTable.AsTableValuedParameter("BillItemT")); // Uverite se da je tip odgovarajući

                var result = await db.ExecuteAsync("InsertBillAndItemsss", parameters, commandType: CommandType.StoredProcedure);

                return result > 0; // Vraća true ako je bar jedan red umetnut
            }
        }
        public void UpdateBillStatus(int billId, int newStatusId)
        {
            var parameters = new[]
            {
        new SqlParameter("@BillID", billId),
        new SqlParameter("@StatusId", newStatusId)
            };

            _context.Database.ExecuteSqlRaw("EXEC UpdateBillStatus @BillID, @StatusId", parameters);
        }
    }

}


