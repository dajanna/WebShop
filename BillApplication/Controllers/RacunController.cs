using AutoMapper;
using BillApplication.Dto;
using BillApplication.Interface;
using BillApplication.Models;
using BillApplication.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace BillApplication.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RacunController : Controller
    {

        private readonly IRacunRepository _billrepository;
        private readonly IMapper _mapper;
        private readonly IStavkeRepository _stavkerepository;
        private readonly BillContext _context;

        public RacunController(IRacunRepository billrepository, IMapper mapper, IStavkeRepository stavkerepository, BillContext context)
        {
            _billrepository = billrepository;
            _mapper = mapper;
            _stavkerepository = stavkerepository;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Racun>))]
        public IActionResult GetBills()
        {
            var racuni = _mapper.Map<List<RacunDto>>(_billrepository.GetBills());
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(racuni);
        }

        [HttpGet("{billId}")]
        [ProducesResponseType(200, Type = typeof(Racun))]
        [ProducesResponseType(400)]

        public IActionResult GetBill(int billId)

        {
            if (!_billrepository.BillExists(billId))
                return NotFound();
            var bill = _mapper.Map<RacunDto>(_billrepository.GetBill(billId));
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(bill);

        }

        [HttpGet("StavkeByBillId/{billId}")]
        [ProducesResponseType(200, Type = typeof(StavkeDto1))]
        [ProducesResponseType(400)]

        public IActionResult GetStavkeByBillId(int billId)

        {
            var stavke = _billrepository.GetBillItemsbyBill(billId);

            if (stavke == null || !stavke.Any())
            {
                return BadRequest("No items found for the given bill ID.");
            }


            var stavkeDto = _mapper.Map<IEnumerable<StavkeDto1>>(stavke);

            return Ok(stavkeDto);

        }

        [HttpGet("ProizvodiByRacunid/{racunId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProizvodDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetProizvodiByRacunId(int racunId)
        {
            var proizvodi = _billrepository.GetProizvodiByRacunId(racunId);

            if (proizvodi == null || !proizvodi.Any())
            {
                return BadRequest("No products found for the given bill ID.");
            }

            // Mapiranje entiteta u DTO (ako koristiš DTO)
            var proizvodiDto = _mapper.Map<IEnumerable<ProizvodDto>>(proizvodi);

            return Ok(proizvodiDto);
        }

        [HttpGet("getstatusbybillid/{billid}")]
        [ProducesResponseType(400)]


        public IActionResult GetStatusByBillID(int billid)
        {
            {
                var status = _billrepository.GetStatusByBillId(billid);
                if (status != null)
                {
                    return Ok(status); 
                }

                return NotFound(); 
            }
        }
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateRacun([FromBody] RacunDto racuncreate)
        {
            if (racuncreate == null)
                return BadRequest(ModelState);

            var racun = _billrepository.GetBills()
               .FirstOrDefault(c => c.BillID == racuncreate.BillID); ;

            if (racun != null)
            {
                ModelState.AddModelError("", "Racun vec postoji ");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var racunMap = _mapper.Map<Racun>(racuncreate);

            if (!_billrepository.CreateBill(racunMap))
            {
                ModelState.AddModelError("", "Something went wrong while savin");
                return StatusCode(500, ModelState);
            }

            return Ok(new { billID = racun.BillID });
        }
        [HttpPut("{racunId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateRacun(int racunId, [FromBody] RacunDto updateRacun)
        {
            if (updateRacun == null)
                return BadRequest(ModelState);

            if (racunId != updateRacun.BillID)
                return BadRequest(ModelState);

            if (!_billrepository.BillExists(racunId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var racunMap = _mapper.Map<Racun>(updateRacun);

            if (!_billrepository.UpdateRacun(racunMap))
            {
                ModelState.AddModelError("", "Something went wrong updating category");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
        [HttpGet("last")]
        public IActionResult GetLastCreatedRacun()
        {
            var lastRacun = _context.Bills.OrderByDescending(r => r.Date).FirstOrDefault();
            if (lastRacun != null)
            {
                return Ok(new { billID = lastRacun.BillID });
            }
            return NotFound();
        }
        [HttpPost("insert")]
        public async Task<IActionResult> InsertBills([FromBody] BulkInsertRequest request)
        {
            // Proverite da li je zahtev validan
            if (request == null || request.Bills == null || request.BillItems == null)
            {
                return BadRequest("Invalid input.");
            }

            // Umetnite račune i dobijte generisane BillId-eve
            var billIds = await _billrepository.InsertBillsAsync(request.Bills);

            // Proverite da li ima generisanih BillId-eva
            if (billIds.Count == 0)
            {
                return BadRequest("No bills were inserted.");
            }

           
            foreach (var item in request.BillItems)
            {
               
                item.BillID = billIds[0]; 
            }

            // Umetnite stavke računa
            await _billrepository.InsertBillItemsAsync(request.BillItems);

            return Ok(new { Message = "Bills and BillItems inserted successfully." });
        }
        [HttpPut("update/{billId}")]
        public async Task<IActionResult> UpdateBill(int billId, [FromBody] UpdateBillRequest request)
        {
            if (request == null || request.Bill == null || request.BillItems == null)
            {
                return BadRequest(new { message = "Invalid request." });
            }

            // Proverite da li je ID računa ispravan
            if (request.Bill.BillID != billId)
            {
                return BadRequest(new { message = "Bill ID mismatch." });
            }

            try
            {
                // Poziva servis za ažuriranje računa
                var result = await _billrepository.UpdateBillAsync(request.Bill, request.BillItems);
                if (result)
                {
                    return Ok(new { message = "Bill updated successfully." });
                }
                return NotFound(new { message = "Bill not found." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }


        [HttpDelete("delete/{billId}")]
        public async Task<IActionResult> DeleteBillWithItems(int billId)
        {
            var racun =  _billrepository.GetBill(billId);
            if (racun == null)
            {
                return NotFound("Račun nije pronađen.");
            }

            // Proverite status pre brisanja (status 1 = Aktivno, samo tada može biti obrisan)
            if (racun.StatusId != (int)BillStatus.Active)
            {
                return BadRequest("Račun se ne može obrisati jer nije u aktivnom statusu.");
            }

            var success = await _billrepository.DeleteBill(billId);
            if (!success)
            {
                return StatusCode(500, "Došlo je do greške prilikom brisanja računa.");
            }

            return NoContent();
        }
        [HttpPost("dodajracun")]
        public async Task<IActionResult> InsertBillAndItems([FromBody] BulkInsertRequest request)
        {
            if (request == null || request.Bills == null || request.BillItems == null)
            {
                return BadRequest("Invalid input.");
            }

            foreach (var bill in request.Bills)
            {
                var success = await _billrepository.InsertBillAndItemsAsync(bill.Price, bill.Date, bill.StatusId, request.BillItems);
                if (!success)
                {
                    return BadRequest("Failed to insert bill and items.");
                }
            }

            return Ok(new { Message = "Bills and BillItems inserted successfully." });
        }
        [HttpPut("UpdateStatus/{billId}")]
        public IActionResult UpdateStatus(int billId, [FromBody] int newStatusId)
        {
            if (newStatusId <= 0)
            {
                return BadRequest("Nevalidan ID statusa.");
            }

            // Poziva se servis za ažuriranje statusa
            try
            {
                _billrepository.UpdateBillStatus(billId, newStatusId);
                return Ok("Status uspešno ažuriran.");
            }
            catch (Exception ex)
            {
                // U slučaju greške, možete logovati grešku ili obraditi drugačije
                return StatusCode(500, $"Greška prilikom ažuriranja statusa: {ex.Message}");
            }
        }
    }
}







