using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Models.Model;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace DoctorAPI.Controllers
{
    [Route("api/comment")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        public CommentController(AppDbContext db, UserManager<AppUser> usermanager)
        {
            _db = db;
            _userManager = usermanager;
        }

        [HttpGet("getall/{username}")]
        public async Task<IActionResult> GetProfileComments([FromRoute]string username) 
        {
            AppUser _user = _db.Users.FirstOrDefault(u=>u.UserName == username);
            if (_user != null) 
            { 
                if(await _userManager.IsInRoleAsync(_user,"Doctor"))
                {
                    Doctor _doc = _db.Doctors.First(d=>d.UserId==_user.Id);
                    List<CommentDTO> _comments = new List<CommentDTO>();
                    foreach(var c in  _db.Comments.Where(c => c.DoctorId == _doc.Id).ToList())
                    {
                        _comments.Add(new CommentDTO
                        { 
                            id = c.Id,
                            fname = c.Author.Fname,
                            lname = c.Author.Lname,
                            Content=c.Content,
                            IsRecommended=c.IsRecommended,
                            Created = c.Created,
                            AuthorUserName=_db.Users.First(u=>u.Id==c.AuthortId).UserName
                        }
                        );
                    }
                    return Ok(_comments);
                }
                else
                {
                    return BadRequest(new {error="User must be a doctor"});
                }
            }
            return NotFound(new {error="User Not Found"});
        }

        [HttpPost("add/{username}")]
        public async Task<IActionResult> AddCommentToDoctor([FromRoute]string username, [FromBody]CommentDTO comment)
        {
            AppUser _user = _db.Users.FirstOrDefault(u => u.UserName == username);
            if( _user != null) 
            {
                if (await _userManager.IsInRoleAsync(_user, "Doctor"))
                {
                    if(_user.UserName == User.Identity.Name)
                    {
                        return BadRequest(new {error = "You can't add a review to yourself"});
                    }
                    AppUser CommentAuthor = _db.Users.First(u => u.UserName == User.Identity.Name);
                    Doctor _doc = _db.Doctors.First(d => d.UserId == _user.Id);
                    Comment _newComment = new Comment
                    {
                        Content = comment.Content,
                        IsRecommended = comment.IsRecommended, 
                        AuthortId = CommentAuthor.Id, 
                        DoctorId = _doc.Id,
                        Created = DateTime.Now
                    };
                    if(_newComment.IsRecommended)
                    {
                        _doc.Recommendtaions++;
                    }
                    _db.Comments.Add(_newComment);
                    _db.SaveChanges();

                    return Ok();
                }
                else
                {
                    return BadRequest(new { error = "User must be a doctor" });
                }
            }
            return BadRequest(new {error="User Not Found"});
        }

        [HttpDelete("delete/{id:int}")]
        public IActionResult RemoveComment([FromRoute]int id)
        {
            if(id!=0)
            {
                Comment targetComment = _db.Comments.FirstOrDefault(c=>c.Id==id);
                if(targetComment!=null)
                {
                    AppUser author = _db.Users.First(a=>a.Id==targetComment.AuthortId);
                    if(author.UserName ==  User.Identity.Name)
                    {
                        if (targetComment.IsRecommended)
                        {
                            _db.Doctors.First(d => d.Id == targetComment.DoctorId).Recommendtaions--;
                        }
                        _db.Comments.Remove(targetComment);
                        _db.SaveChanges();
                        return Ok();
                    }
                    return Unauthorized();
                }
                return NotFound();
            }
            return BadRequest();
        }
        [AllowAnonymous]
        [HttpGet("details/{id:int}", Name = "CommentDetails")]
        public IActionResult CommentDetails([FromRoute] int id)
        {
            if(id!=0)
            {
                Comment targetComment = _db.Comments.FirstOrDefault(c => c.Id == id);
                if (targetComment != null)
                {
                    string DocProfileId = _db.Doctors.Find(targetComment.DoctorId).UserId;
                    AppUser Doc = _db.Users.Find(DocProfileId);
                    return Ok(new 
                    {
                        id = targetComment.Id,
                        FromProfile = Doc.UserName,
                        Author = _db.Users.Find(targetComment.AuthortId).UserName,
                        fname = targetComment.Author.Fname,
                        lname = targetComment.Author.Lname,
                        Date = targetComment.Created,
                        Content = targetComment.Content,
                        IsRecommended = targetComment.IsRecommended
                    }
                    );
                }
                return NotFound();
            }
            return BadRequest();
        }

        [HttpPut("edit/{id:int}")]
        public IActionResult EditComment([FromRoute] int id, [FromBody]CommentDTO comment)
        {
            if (id != 0)
            {
                Comment targetComment = _db.Comments.FirstOrDefault(c=>c.Id==id);
                if(targetComment!=null) 
                {
                    AppUser author = _db.Users.Find(targetComment.AuthortId);
                    if(author.UserName == User.Identity.Name)
                    {
                        if (targetComment.IsRecommended != comment.IsRecommended)
                        {
                            _db.Doctors.Find(targetComment.DoctorId).Recommendtaions += targetComment.IsRecommended ? -1 : 1;
                            targetComment.IsRecommended = comment.IsRecommended;
                        }
                        targetComment.Content = comment.Content;
                        _db.SaveChanges();
                        string DocProfileId = _db.Doctors.Find(targetComment.DoctorId).UserId;
                        AppUser Doc = _db.Users.Find(DocProfileId);
                        return CreatedAtRoute("CommentDetails", new { id = id }, new
                        {
                            FromProfile = Doc.UserName,
                            Author = _db.Users.Find(targetComment.AuthortId).UserName,
                            Date = targetComment.Created,
                            Content = targetComment.Content,
                            IsRecommended = targetComment.IsRecommended
                        });
                    }
                    return Unauthorized();
                }
                return NotFound();
            }
            return BadRequest();
        }
    }
}
