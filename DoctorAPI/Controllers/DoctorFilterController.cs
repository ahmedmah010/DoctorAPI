using Data;
using Data.Repos.IRepos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.DTO;
using Models.Model;
using System.Linq.Expressions;

namespace DoctorAPI.Controllers
{
    [Route("api/filterby")]
    [ApiController]
    public class DoctorFilterController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IRepo<Doctor> _DoctorRepo;
        private readonly AppDbContext _db;

        public DoctorFilterController(IRepo<Doctor> docRepo, UserManager<AppUser> userMang, AppDbContext db)
        {
            _DoctorRepo = docRepo;
            _userManager = userMang;
            _db = db;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<List<DoctorDTO>> MainFilterFunction(Expression<Func<Doctor, bool>> filter)
        {
            List<DoctorDTO> docs = new List<DoctorDTO>();
            foreach (var doc in _DoctorRepo.GetAll().AsQueryable().Where(filter))
            {
                docs.Add(new DoctorDTO
                {
                    Roles = (List<string>)await _userManager.GetRolesAsync(doc.User),
                    UserName = doc.User.UserName,
                    Fname = doc.User.Fname,
                    Lname = doc.User.Lname,
                    Email = doc.User.Email,
                    img = doc.User.img,
                    Phone = doc.User.PhoneNumber,
                    City = doc.User.city,
                    Education = doc.education,
                    Experience = doc.experience,
                    Position = doc.position,
                    WorkAdress = doc.WorkAddress,
                    Price = doc.Price,
                    TotalRates = doc.Comments.Count,
                    RecommendedBy = $"{(doc.Recommendtaions * 100)}%"
                }
                );
            }
            return docs;
        }
        [HttpGet("city")]
        public async Task<IActionResult> FilterByCity([FromQuery] string city)
        {
            List<DoctorDTO> docs = await MainFilterFunction(d => d.User.city.ToUpper() == city.ToUpper());
            if (docs.IsNullOrEmpty())
            {
                return NotFound();
            }
            return Ok(docs);
        }

        [HttpGet("price")]
        public async Task<IActionResult> FilterByPrice([FromQuery] string comparison, [FromQuery] float price)
        {
            List<DoctorDTO> docs = new List<DoctorDTO>();
            switch (comparison)
            {
                case ">":
                    docs = await MainFilterFunction(d => d.Price > price);
                    break;
                case "<":
                    docs = await MainFilterFunction(d => d.Price < price);
                    break;
                case ">=":
                    docs = await MainFilterFunction(d => d.Price >= price);
                    break;
                case "<=":
                    docs = await MainFilterFunction(d => d.Price <= price);
                    break;
                case "==":
                    docs = await MainFilterFunction(d => d.Price == price);
                    break;
                default:
                    docs = null;
                    break;
            }

            if (docs.IsNullOrEmpty())
            {
                return NotFound();
            }
            return Ok(docs);
        }

        [HttpGet("pagination/{pageIndex:int}/{pageSize:int}")]
        public async Task<IActionResult> Paginate(int pageIndex, int pageSize)
        {

            List<DoctorDTO> docs = (await MainFilterFunction(d => true)).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return Ok(docs);
        }

        [HttpGet("city/getallcities")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAllCities()
        {
            return Ok(_db.Cities.ToList());
        }

        [HttpGet("position/getallpositions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAllPos()
        {
            return Ok(_db.DoctorPositions.ToList());
        }

        [HttpGet("{city}/{position}")]
        public async Task<IActionResult> GetDocs([FromRoute]string city, [FromRoute]string position)
        {
            if(!String.IsNullOrEmpty(city) && !String.IsNullOrEmpty(position))
            {
                List<DoctorDTO> docs = await MainFilterFunction(d => d.User.city.ToUpper() == city.ToUpper() && d.position.ToUpper() == position.ToUpper());
                if(!docs.IsNullOrEmpty())
                {
                    return Ok(docs);
                }
                return NotFound();
            }
            return BadRequest();
        }


    }
}
