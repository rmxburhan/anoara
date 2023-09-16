using System.Security.Claims;
using System.Collections.Immutable;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApi.Common;
using WebApi.dto.Post;
using WebApi.Models;
using WebApi.dto.Comment;

namespace WebApi.Controllers;

[ApiController]
[Route("api/posts")]
public class PostController : ControllerBase
{
    private readonly ApiDataContext dataContext;
    private readonly IUploadPath uploadPath;

    public PostController(ApiDataContext dataContext, IUploadPath uploadPath)
    {
        this.dataContext = dataContext;
        this.uploadPath = uploadPath;
    }

    [Authorize, HttpGet]
    public async Task<IActionResult> GetPosts()
    {
        var posts = await dataContext.Posts.OrderByDescending(x => x.CreatedAt).Include(x => x.Comments).Include(x => x.Teacher).Where(x => x.DeletedAt == null).ToListAsync();
        return Ok(new
        {
            message = "Get data posts success",
            data = posts
        });
    }

    [Authorize, HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPost(Guid id)
    {
        var post = await dataContext.Posts.Include(x => x.Teacher).Include(x => x.Comments).ThenInclude(x => x.Student).FirstOrDefaultAsync(x => x.DeletedAt == null && x.Id == id);
        if (post == null)
            return NotFound();
        return Ok(new
        {
            message = "Get data post success",
            data = post
        });
    }

    // /// <summary>
    // /// This is an enpoint for teacher so teacher id coming from jsontoken
    // /// </summary>
    // /// <returns></returns>
    // [HttpPut("{id:guid}")]
    // public async Task<IActionResult> UpdatePost()
    // {


    // }

    [Authorize(Roles = "Teacher"), HttpPost]
    public async Task<IActionResult> AddPost([FromForm] AddPostRequest request)
    {
        ClaimsPrincipal claims = HttpContext.User;
        var teacher_id = claims.FindFirstValue(ClaimTypes.NameIdentifier);
        var teacher = await dataContext.Teachers.FirstOrDefaultAsync(x => x.Id == Guid.Parse(teacher_id) && x.DeletedAt == null);
        if (teacher == null)
            return Unauthorized();
        var post = new Post
        {
            Title = request.Title,
            Content = request.Content,
            TeacherId = teacher.Id,
            CreatedAt = DateTime.Now
        };

        if (request.Banner != null)
        {
            var filename = Guid.NewGuid().ToString() + request.Banner.FileName;
            request.Banner.CopyTo(new FileStream(Path.Combine(uploadPath.PostImage(), filename), FileMode.Create));
            post.Banner = Path.Combine("images","post",filename);
        }

        dataContext.Posts.Add(post);
        await dataContext.SaveChangesAsync();

        return Ok(new
        {
            message = "Post data has been added",
            data = post
        });
    }

    [Authorize(Roles = "Teacher"), HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdatePost([FromForm] UpdatePostRequest request, Guid id)
    {
        var post = await dataContext.Posts.FirstOrDefaultAsync(x => x.DeletedAt == null && x.Id == id);

        if (post == null)
            return NotFound();

        if (!request.Title.IsNullOrEmpty())
            post.Title = request.Title;
        if (!request.Content.IsNullOrEmpty())
            post.Content = request.Content;

        if (request.Banner != null)
        {
            if (!Directory.Exists(uploadPath.PostImage()))
                Directory.CreateDirectory(uploadPath.PostImage());

            var filename = Guid.NewGuid().ToString() + request.Banner.FileName;
            request.Banner.CopyTo(new FileStream(Path.Combine(uploadPath.PostImage(), filename), FileMode.Create));
            if (!String.IsNullOrEmpty(post.Banner))
                if (System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", post.Banner)))
                    System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", post.Banner));
            post.Banner = Path.Combine("images", "post", filename);
        }
        post.UpdatedAt = DateTime.Now;
        dataContext.Posts.Update(post);
        await dataContext.SaveChangesAsync();

        return Ok(new
        {
            message = "Post data has been updated",
            data = post
        });
    }


    [Authorize(Roles = "Teacher"), HttpDelete("{id:guid}")]
    public async Task<IActionResult> UpdatePost(Guid id)
    {
        var post = await dataContext.Posts.FirstOrDefaultAsync(x => x.DeletedAt == null && x.Id == id);

        if (post == null)
            return NotFound();

        post.DeletedAt = DateTime.Now;
        dataContext.Posts.Update(post);
        await dataContext.SaveChangesAsync();

        return NoContent();
    }


    /// <summary>
    /// This endpoint is for adding a comment to a post 
    /// can acces for student only  
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = "Student"), HttpPost("{id:guid}/comment")]
    public async Task<IActionResult> AddComment(AddCommentRequest request, Guid id)
    {
        ClaimsPrincipal claims = HttpContext.User;
        var studentId = claims.FindFirstValue(ClaimTypes.NameIdentifier);
        var student = await dataContext.Students.FirstOrDefaultAsync(x => x.Id == Guid.Parse(studentId) && x.DeletedAt == null);
        if (student == null)
            return Unauthorized();
        var post = await dataContext.Posts.FirstOrDefaultAsync(x => x.DeletedAt == null && x.Id == id);
        if (post == null)
            return NotFound();

        var comment = new Comment
        {
            Message = request.Message,
            CreatedAt = DateTime.Now,
            StudentId = student.Id,
            PostId = post.Id
        };

        dataContext.Comments.Add(comment);
        await dataContext.SaveChangesAsync();

        return Ok(comment);
    }

    /// <summary>
    /// This endpoint is for adding a comment to a post 
    /// can acces for student only  
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = "Student"), HttpPost("{id:guid}/comment/{commentId:guid}")]
    public async Task<IActionResult> UpdateComment(UpdateCommentRequest request, Guid id, Guid commentId)
    {
        ClaimsPrincipal claims = HttpContext.User;
        var studentId = claims.FindFirstValue(ClaimTypes.NameIdentifier);
        var student = await dataContext.Students.FirstOrDefaultAsync(x => x.Id == Guid.Parse(studentId) && x.DeletedAt == null);
        if (student == null)
            return Unauthorized();
        var post = await dataContext.Posts.FirstOrDefaultAsync(x => x.DeletedAt == null && x.Id == id);
        if (post == null)
            return NotFound();

        var comment = await dataContext.Comments.FirstOrDefaultAsync(x => x.DeletedAt == null && x.Id == id);
        if (comment == null)
            return NotFound();

        if (!request.Message.IsNullOrEmpty())
            comment.Message = request.Message;
        comment.UpdatedAt = DateTime.Now;
        dataContext.Comments.Update(comment);
        await dataContext.SaveChangesAsync();

        return Ok(comment);
    }
}