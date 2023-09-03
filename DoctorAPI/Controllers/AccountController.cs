using Data;
using Data.Repos.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.DTO;
using Models.Model;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace DoctorAPI.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IRepo<Doctor> _DOCRepo;
        private readonly IRepo<NormalUser> _NormalUserRepo;
        private readonly IRepo<Comment> _commentRepo;

        public AccountController(UserManager<AppUser> userManger, IConfiguration config, IRepo<Doctor> docrepo, IRepo<NormalUser> NormalUserRepo, IRepo<Comment> commentrepo)
        {
            _userManager = userManger;
            _config = config;
            _DOCRepo = docrepo;
            _NormalUserRepo = NormalUserRepo;
            _commentRepo = commentrepo;
        }
        
        [HttpPost("register/user")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UserRegister([FromBody]UserRegisterDTO usereg)
        {

            if (ModelState.IsValid)
            {
                //Create User
                AppUser user = new AppUser();
                NormalUser normalUser = new NormalUser();
                user.Fname = usereg.fname;
                user.Lname = usereg.lname;
                user.UserName = usereg.username;
                user.Email = usereg.email;
                user.PhoneNumber = usereg.phone;
                user.city = usereg.city;
                user.img = usereg.imgurl;
                IdentityResult res = await _userManager.CreateAsync(user, usereg.password);
                if (res.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    normalUser.UserId = user.Id;
                    _NormalUserRepo.Add(normalUser);
                    _NormalUserRepo.SaveChanges();
                    return Ok();

                }

                foreach (var error in res.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return BadRequest(ModelState);
        }

        [HttpPost("register/doctor")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DoctorRegister([FromBody]DoctorRegisterDTO docReg)
        {
            if (ModelState.IsValid) 
            {
                AppUser _appuser = new AppUser();
                Doctor _doc = new Doctor();
                _appuser.Fname = docReg.fname;
                _appuser.Lname = docReg.lname;
                _appuser.UserName = docReg.username;
                _appuser.Email = docReg.email;
                _appuser.PhoneNumber = docReg.phone;
                _appuser.city = docReg.city;
                _appuser.img = docReg.imgurl;

                _doc.education = docReg.education;
                _doc.experience = docReg.experience;
                _doc.position = docReg.position;
                _doc.WorkAddress = docReg.WorkAddress;
                _doc.Price = docReg.Price;
                IdentityResult _res = await _userManager.CreateAsync(_appuser, docReg.password);
                if(_res.Succeeded)
                {
                    await _userManager.AddToRoleAsync(_appuser, "Doctor");
                    _doc.UserId = _appuser.Id;
                    _DOCRepo.Add(_doc);
                    _DOCRepo.SaveChanges();
                    return Ok();
                }
                IEnumerable<IdentityError> _errors = _res.Errors;
                foreach (var error in _errors) 
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> login([FromBody]UserLoginDTO userlogin)
        {
            if (ModelState.IsValid) 
            {
                AppUser user = await _userManager.FindByNameAsync(userlogin.username);
                if(user != null)
                {
                    bool CorrectPass = await _userManager.CheckPasswordAsync(user, userlogin.password);
                    if(CorrectPass) 
                    {
                        List<Claim> _claims = new List<Claim>();
                        _claims.Add(new Claim(ClaimTypes.NameIdentifier, userlogin.password));
                        _claims.Add(new Claim(ClaimTypes.Name, user.UserName));
                        _claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                        var roles = await _userManager.GetRolesAsync(user);
                        foreach (var role in roles)
                        {
                            _claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                        }
                        SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:key"]));
                        SigningCredentials cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        JwtSecurityToken token = new JwtSecurityToken(
                            issuer: _config["JWT:issuer"],
                            audience: _config["JWT:aud"],
                            claims: _claims,
                            expires: DateTime.Now.AddDays(15),
                            signingCredentials: cred
                            );
                        return Ok(new 
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expire = token.ValidTo
                        });
                    }
                 
                        return Unauthorized(new {error="Wrong Password"});
                    
                }  
                else
                {
                    return Unauthorized(new { error = "No user with this username." });
                }
            }
            return BadRequest(ModelState);
        }

        [HttpGet("details/{username}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ProfileInfo([FromRoute]string username)
        {
            AppUser user = await _userManager.FindByNameAsync(username);
            if(user!=null) 
            {
                List<string> roles = new List<string>();
                foreach(var role in await _userManager.GetRolesAsync(user))
                {
                    roles.Add(role);
                }
                JsonObject info = new JsonObject();
                info.Add("Roles", JsonConvert.SerializeObject(roles));
                info.Add("UserName", user.UserName);
                info.Add("First Name", user.Fname);
                info.Add("Last Name", user.Lname);
                info.Add("Email", user.Email);
                info.Add("City", user.city);
                info.Add("Phone", user.PhoneNumber);
                if(await _userManager.IsInRoleAsync(user,"Doctor"))
                {
                    Doctor doc = _DOCRepo.GetAll("Comments").First(d => d.UserId == user.Id);
                    info.Add("Education", doc.education);
                    info.Add("Experience", doc.experience);
                    info.Add("Position", doc.position);
                    info.Add("Work Address", doc.WorkAddress);
                    double totalRates = (double)doc.Comments.Count;
                    double recommended = 0;
                    if (totalRates > 0) 
                    {
                        recommended = ((double)doc.Recommendtaions / totalRates) * 100;
                    }
                    info.Add("Recommended By", $"{recommended}%");
                    info.Add("Total Rates", totalRates);
                    info.Add("Price", doc.Price);
                }
                info.Add("img",user.img);
                return Ok(info);
            }
            else
            {
                return BadRequest(new {error="User Not Found"});
            }

        }

        [HttpPut("changepassword/{username}")]
        public async Task<IActionResult> ChangePassword([FromRoute] string username, [FromBody] ChangePassDTO changepassDTO)
        {
            AppUser target = await _userManager.FindByNameAsync(username);
            if (target != null)
            {
                if (target.UserName == User.Identity.Name)
                {
                    IdentityResult res = await _userManager.ChangePasswordAsync(target, changepassDTO.CurrentPass, changepassDTO.NewPass);
                    if (res.Succeeded)
                    {
                        return Ok();
                    }
                    JsonObject errors = new JsonObject();
                    foreach (var e in res.Errors)
                    {
                        errors.Add("", e.Description);
                    }
                    return BadRequest(errors);
                }
                return Unauthorized();
            }
            return NotFound();
        }

        [HttpPut("edit/{username}")]
        [Authorize]
        public async Task<IActionResult> EditProfile([FromRoute]string username, [FromBody]EditDTO editDTO)
        {
            AppUser target = await _userManager.FindByNameAsync(username);
            if(target!=null)
            {
                if(target.UserName==User.Identity.Name)
                {
                    target.Fname = editDTO.Fname ?? target.Fname;
                    target.Lname = editDTO.Lname ?? target.Lname;
                    target.Email = editDTO.Email ?? target.Email;
                    target.city = editDTO.City ?? target.city;
                    target.img = editDTO.img ?? target.img;
                    target.PhoneNumber = editDTO.Phone ?? target.PhoneNumber;
                    Doctor _doc = _DOCRepo.Get(d => d.UserId == target.Id);
                    if(_doc!=null) 
                    {
                        _doc.Price = editDTO.Price ?? _doc.Price;
                        _doc.education = editDTO.Education ?? _doc.education;
                        _doc.position = editDTO.Position ?? _doc.position;
                        _doc.WorkAddress = editDTO.WorkAdress ?? _doc.WorkAddress;
                        _doc.experience = editDTO.Experience ?? _doc.experience;
                        _DOCRepo.SaveChanges();
                    }
                    IdentityResult res = await _userManager.UpdateAsync(target);
                    if(res.Succeeded)
                    {
                        return Ok();
                    }
                    JsonObject errors = new JsonObject();
                    foreach (var e in res.Errors)
                    {
                        errors.Add("", e.Description);
                    }
                    return BadRequest(errors);
                }
                return Unauthorized();
            }
            return NotFound();
        }

        [HttpDelete("delete/{username}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Remove([FromRoute]string username)
        {
            AppUser target = await _userManager.FindByNameAsync(username);
            if (target != null) 
            {
                foreach(string role in await _userManager.GetRolesAsync(target))
                {
                    if(role == "Doctor")
                    {
                        Doctor doc = _DOCRepo.Get(d => d.User.Id == target.Id);
                        foreach (Comment comment in doc.Comments)
                        {
                            _commentRepo.Delete(comment);
                        }
                        _DOCRepo.Delete(doc);
                        _DOCRepo.SaveChanges();
                    }
                    else if(role == "User")
                    {
                        NormalUser normalUser = _NormalUserRepo.Get(d => d.User.Id == target.Id);
                        _NormalUserRepo.Delete(normalUser);
                        _NormalUserRepo.SaveChanges();
                    }
                }
                IdentityResult res = await _userManager.DeleteAsync(target);
                if(res.Succeeded)
                {
                    return Ok();
                }
                JsonObject errors = new JsonObject();
                foreach(var e in res.Errors)
                {
                    errors.Add("",e.Description);
                }
                return BadRequest(errors);
            }
            return NotFound();
        }

        [HttpPost("SetRole/{username}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetRole([FromRoute]string username, [FromQuery]string role)
        {
            AppUser target = await _userManager.FindByNameAsync(username);
            if(target!=null)
            {
                IdentityResult res = await _userManager.AddToRoleAsync(target, role);
                if (res.Succeeded)
                {
                    return Ok();
                }
                JsonObject errors = new JsonObject();
                foreach (var e in res.Errors)
                {
                    errors.Add("", e.Description);
                }
                return BadRequest(errors);
            }
            return NotFound();
        }



    }
}
