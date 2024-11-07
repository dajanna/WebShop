using AutoMapper;
using BillApplication.Dto;
using BillApplication.Interface;
using BillApplication.Models;
using Microsoft.AspNetCore.Mvc;

namespace BillApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatusController : Controller
    {
        private readonly IStatusRepository _statusRepository;
        private readonly IMapper _mapper;

        public StatusController(IStatusRepository statusRepository, IMapper mapper)
        {
            _statusRepository = statusRepository;
            _mapper = mapper;

        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Status>))]
        public IActionResult GetListOfStatus()
        {
            var status = _mapper.Map<List<StatusDto>>(_statusRepository.GetListOfStatus());
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(status);
        }
        [HttpGet("{statusId}")]
        [ProducesResponseType(200, Type = typeof(Status))]
        [ProducesResponseType(400)]

        public IActionResult GetStatus(int statusId)

        {
            if (!_statusRepository.BillStatusExists(statusId))
                return NotFound();
            var status = _mapper.Map<StatusDto>(_statusRepository.GetStatus(statusId));
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(status);


        }
        [HttpGet("GetBillsByStatus/{statusId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<RacunDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetBillsByStatus(int statusId)
        {
            var bills = _statusRepository.GetBillsByStatus(statusId);

            if (bills == null || !bills.Any())
            {
                return BadRequest("No bills found for the given status.");
            }

            var billsDto = _mapper.Map<IEnumerable<RacunDto>>(bills);

            return Ok(billsDto);
        }
        [HttpPost("CreateStatus")]
        public IActionResult CreateStatus([FromBody] StatusDto statusDto)
        {
            if (statusDto == null)
            {
                return BadRequest("Invalid status data.");
            }

            // Pozivanje metode za umetanje proizvoda
            _statusRepository.InsertStatus(statusDto.Name);

            return Ok("Status created successfully.");
        }
        [HttpPut("UpdateStatus/{statusId}")]
        public IActionResult UpdateStatus(int statusId, [FromBody] StatusDto statusDto)
        {
            if (statusDto == null || statusDto.StatusID != statusId)
            {
                return BadRequest("Invalid status data.");
            }

            // Pozivanje metode za ažuriranje proizvoda
            _statusRepository.UpdateStatus(statusId, statusDto.Name);

            return Ok("Status updated successfully.");
        }

        
    }

}




