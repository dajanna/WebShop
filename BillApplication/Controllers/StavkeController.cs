using AutoMapper;
using BillApplication.Dto;
using BillApplication.Interface;
using BillApplication.Models;
using BillApplication.Repository;
using Microsoft.AspNetCore.Mvc;


namespace BillApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StavkeController : Controller
    {
        private readonly IStavkeRepository _stavkeRepository;
        private readonly IMapper _mapper;

        public StavkeController(IStavkeRepository stavkeRepository, IMapper mapper)
        {
            _stavkeRepository = stavkeRepository;
            _mapper = mapper;

        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Stavke>))]

        public IActionResult GetItems()
        {
            var items = _mapper.Map<List<Dto.StavkeDto1>>(_stavkeRepository.GetItems());
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(items);
        }

        [HttpGet("{itemid}")]
        [ProducesResponseType(200, Type = typeof(Stavke))]
        [ProducesResponseType(400)]
        public IActionResult GetItem(int itemid)
        {
            if (!_stavkeRepository.BillItemExists(itemid))
                return NotFound();

            var item = _mapper.Map<Dto.StavkeDto1>(_stavkeRepository.GetItem(itemid));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(item);
        }
        [HttpGet("{itemsID}")]
        public ActionResult<IEnumerable<Proizvod>> GetProizvodByItemsID(int itemsID)
        {
            var proizvodi = _stavkeRepository.GetProizvodByItemsID(itemsID);

            if (proizvodi == null || !proizvodi.Any())
            {
                return NotFound();
            }

            return Ok(proizvodi);
        }
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateStavke([FromBody] Dto.StavkeDto1 stavkeCreate)
        {
            if (stavkeCreate == null)
                return BadRequest(ModelState);

            var stavke = _stavkeRepository.GetItems()
               .FirstOrDefault(c => c.ItemsID == stavkeCreate.ItemsID); ;

            if (stavke != null)
            {
                ModelState.AddModelError("", "Racun vec postoji ");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var stavkeMap = _mapper.Map<Stavke>(stavkeCreate);

            if (!_stavkeRepository.CreateStavke(stavkeMap))
            {
                ModelState.AddModelError("", "Something went wrong while savin");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }
        [HttpPut("{stavkeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateStavke(int stavkeId, [FromBody] Dto.StavkeDto1 updateStavke)
        {
            if (updateStavke == null)
                return BadRequest(ModelState);

            if (stavkeId != updateStavke.ItemsID)
                return BadRequest(ModelState);

            if (!_stavkeRepository.BillItemExists(stavkeId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var stavkeMap = _mapper.Map<Stavke>(updateStavke);

            if (!_stavkeRepository.UpdateStavke(stavkeMap))
            {
                ModelState.AddModelError("", "Something went wrong updating category");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
        [HttpDelete("{stavkeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteStavka(int stavkeId)
        {
            if (!_stavkeRepository.BillItemExists(stavkeId))
            {
                return NotFound();
            }

            var stavkeToDelete = _stavkeRepository.GetItem(stavkeId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_stavkeRepository.DeleteStavka(stavkeToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting owner");
            }

            return NoContent();

        }
       

    }
}