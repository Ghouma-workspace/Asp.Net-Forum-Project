using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MyForum.BL.Entities;
using MyForum.DAL;

namespace MyForum.WEB.Controllers
{
    public class PostsController : Controller
    {
        private readonly MyForumDbContext _context;
        private readonly IWebHostEnvironment _HostEnvironment;
        private readonly UserManager<User> _userManager;

        public PostsController(MyForumDbContext context, IWebHostEnvironment hostEnvironment, UserManager<User> userManager)
        {
            _context = context;
            this._HostEnvironment = hostEnvironment;
            _userManager = userManager;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var myForumDbContext = _context.Posts.Include(p => p.Blog);
            return View(await myForumDbContext.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            List<Comment> p = new List<Comment>();
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.PostId == id); 
            if (post == null)
            {
                return NotFound();
            }
            var _repls = await _context.Repls.ToListAsync();
            var _users = await _userManager.Users.ToListAsync();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<string> comm = new List<string>();
            foreach (var item in _context.Comments)
            {
                if (item.PostId == id)
                {
                    foreach (var element in _users)
                    {
                        if (item.Id == element.Id)
                        {
                            item.User = element;
                        }
                    }
                    p.Add(item);
                }
                if (userId == item.Id)
                {
                    comm.Add(item.CommentId.ToString());
                }

            }

            ViewData["PostTitle"] = post.Title;
            ViewData["PostDescription"] = post.Description;
            ViewData["PostContent"] = post.Content;
            ViewData["idpost"] = id;
            ViewData["idblog"] = post.BlogId;

            ViewBag.CommentsUsed = comm;

            return View(p);
        }

        // GET: Posts/Create
        public IActionResult Create(int? id)
        {
            ViewData["id"] = id;
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Title,Description,Content,Coverpic,Picname,PublishedDateTime,BlogId")] Post post)
        {
            if (ModelState.IsValid)
            {
                if (post.Coverpic != null)
                {
                    string wwwRootPath = _HostEnvironment.WebRootPath;
                    string filename = Path.GetFileNameWithoutExtension(post.Coverpic.FileName);
                    string extension = Path.GetExtension(post.Coverpic.FileName);
                    post.Picname = filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/images/", filename);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await post.Coverpic.CopyToAsync(fileStream);
                    }
                }
                else
                {
                    post.Picname = "standard pic.png";
                }
                //
                post.PublishedDateTime = DateTime.Now;
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Blogs", new { id = post.BlogId });
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "BlogId", "BlogId", post.BlogId);
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "BlogId", "BlogId", post.BlogId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Content,Description,Coverpic,Picname,PublishedDateTime,BlogId")] Post post)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string wwwRootPath = _HostEnvironment.WebRootPath;
                    string filename = Path.GetFileNameWithoutExtension(post.Coverpic.FileName);
                    string extension = Path.GetExtension(post.Coverpic.FileName);
                    post.Picname = filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/images/", filename);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await post.Coverpic.CopyToAsync(fileStream);
                    }
                    //
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Blogs", new { id = post.BlogId });
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "BlogId", "BlogId", post.BlogId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'MyForumDbContext.Posts'  is null.");
            }
            var post = await _context.Posts.FindAsync(id);

            if (post != null)
            {
                var imagepath = Path.Combine(_HostEnvironment.WebRootPath, "images", post.Picname);
                if (System.IO.File.Exists(imagepath))
                    System.IO.File.Delete(imagepath);
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Blogs", new { id = post?.BlogId });
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
