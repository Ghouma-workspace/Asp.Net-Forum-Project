using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MyForum.BL.Entities;
using MyForum.DAL;

namespace MyForum.WEB.Controllers
{
    public class CommentsController : Controller
    {
        private readonly MyForumDbContext _context;
        private readonly IWebHostEnvironment _HostEnvironment;

        public CommentsController(MyForumDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _HostEnvironment= webHostEnvironment;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            var myForumDbContext = _context.Comments.Include(c => c.Post);
            return View(await myForumDbContext.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comments/Create
        public IActionResult Create(int id)
        {
            ViewData["idpost"] = id;
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CommentId,Text,Commentpic,Picname,PostId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _HostEnvironment.WebRootPath;
                if (comment.Commentpic != null)
                {
                    string filename = Path.GetFileNameWithoutExtension(comment.Commentpic.FileName);
                    string extension = Path.GetExtension(comment.Commentpic.FileName);
                    comment.Picname = filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/CommentImage/", filename);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await comment.Commentpic.CopyToAsync(fileStream);
                    }
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                comment.Id = userId;
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Posts", new { id = comment.PostId });
            }
            ViewData["PostId"] = new SelectList(_context.Posts, "PostId", "Content", comment.PostId);
            return View(comment);
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["PostId"] = new SelectList(_context.Posts, "PostId", "PostId", comment.PostId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CommentId,Text,Commentpic,Picname,Id,PostId")] Comment comment)
        {
            if (id != comment.CommentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string wwwRootPath = _HostEnvironment.WebRootPath;
                    string filename = Path.GetFileNameWithoutExtension(comment.Commentpic.FileName);
                    string extension = Path.GetExtension(comment.Commentpic.FileName);
                    comment.Picname = filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/CommentImage/", filename);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await comment.Commentpic.CopyToAsync(fileStream);
                    }
                    //
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.CommentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Posts", new { id = comment.PostId });
            }
            ViewData["PostId"] = new SelectList(_context.Posts, "PostId", "Content", comment.PostId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Comments == null)
            {
                return Problem("Entity set 'MyForumDbContext.Comments'  is null.");
            }
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                var imagepath = Path.Combine(_HostEnvironment.WebRootPath, "CommentImage", comment.Picname);
                if (System.IO.File.Exists(imagepath))
                    System.IO.File.Delete(imagepath);
                _context.Comments.Remove(comment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Posts", new { id = comment?.PostId });
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.CommentId == id);
        }
    }
}
