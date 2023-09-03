using Castle.Components.DictionaryAdapter;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Model;

namespace DoctorAPI.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AdminController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost("addcity/{city}")]
        [Authorize(Roles = "Admin")]
        public IActionResult AddCity([FromRoute] string city)
        {
            if (!String.IsNullOrEmpty(city))
            {
                City city1 = new City() { Name = city };
                _context.Cities.Add(city1);
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }
        [HttpPost("addpos/{pos}")]
        [Authorize(Roles = "Admin")]
        public IActionResult AddPos([FromRoute] string pos)
        {
            if (!String.IsNullOrEmpty(pos))
            {
                DoctorPosition pos1 = new DoctorPosition() { Name = pos };
                _context.DoctorPositions.Add(pos1);
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }
        [HttpDelete("delcity/{city}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DelCity([FromRoute]string city)
        {
            if (!String.IsNullOrEmpty(city))
            {
                City target = _context.Cities.FirstOrDefault(x => x.Name.ToUpper() == city.ToUpper());
                if(target != null)
                {
                    _context.Cities.Remove(target);
                    _context.SaveChanges();
                    return Ok();
                }
                return NotFound();
            }
            return BadRequest();
        }
        [HttpDelete("delpos/{pos}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DelPos([FromRoute]string pos)
        {
            if (!String.IsNullOrEmpty(pos))
            {
                DoctorPosition target = _context.DoctorPositions.FirstOrDefault(x => x.Name.ToUpper() == pos.ToUpper());
                if (target != null)
                {
                    _context.DoctorPositions.Remove(target);
                    _context.SaveChanges();
                    return Ok();
                }
                return NotFound();
            }
            return BadRequest();
        }
    }

}
