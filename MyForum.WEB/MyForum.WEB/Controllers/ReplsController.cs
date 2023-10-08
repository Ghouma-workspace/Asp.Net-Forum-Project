using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyForum.BL.Entities;
using MyForum.DAL;

namespace MyForum.WEB.Controllers
{
    public class ReplsController : Controller
    {
        private readonly MyForumDbContext _context;
        private readonly IWebHostEnvironment _HostEnvironment;

        public ReplsController(MyForumDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _HostEnvironment = hostEnvironment;
        }

        // GET: Repls
        public async Task<IActionResult> Index()
        {
            var myForumDbContext = _context.Repls.Include(r => r.Comment);
            return View(await myForumDbContext.ToListAsync());
        }

        // GET: Repls/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Repls == null)
            {
                return NotFound();
            }

            var repl = await _context.Repls
                .Include(r => r.Comment)
                .FirstOrDefaultAsync(m => m.IdRepl == id);
            if (repl == null)
            {
                return NotFound();
            }

            return View(repl);
        }

        // GET: Repls/Create
        public IActionResult Create(int? id)
        {
            ViewData["idcomment"] = id;
            List<Comment> comments = _context.Comments.ToList();
            foreach (var ele in comments)
            {
                if (id == ele.CommentId)
                    ViewData["idpost"] = ele.PostId;
            }
            return View();
        }

        // POST: Repls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdRepl,Text,Replpic,Picname,CommentId")] Repl repl)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _HostEnvironment.WebRootPath;
                if (repl.Replpic != null)
                {
                    string filename = Path.GetFileNameWithoutExtension(repl.Replpic.FileName);
                    string extension = Path.GetExtension(repl.Replpic.FileName);
                    repl.Picname = filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/Replimage/", filename);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await repl.Replpic.CopyToAsync(fileStream);
                    }
                }

                var comments = await _context.Comments.ToListAsync();
                foreach (var elemnt in comments)
                {
                    if (repl.CommentId == elemnt.CommentId)
                    {
                        repl.Comment = elemnt;
                    }
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                repl.Id = userId;
                _context.Add(repl);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Posts", new { id = repl.Comment.PostId });
            }
            ViewData["CommentId"] = new SelectList(_context.Comments, "CommentId", "CommentId", repl.CommentId);
            return View(repl);
        }

        // GET: Repls/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Repls == null)
            {
                return NotFound();
            }

            var repl = await _context.Repls.FindAsync(id);
            if (repl == null)
            {
                return NotFound();
            }
            ViewData["CommentId"] = new SelectList(_context.Comments, "CommentId", "CommentId", repl.CommentId);
            return View(repl);
        }

        // POST: Repls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRepl,Text,Picname,CommentId")] Repl repl)
        {
            if (id != repl.IdRepl)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string wwwRootPath = _HostEnvironment.WebRootPath;
                    string filename = Path.GetFileNameWithoutExtension(repl.Replpic.FileName);
                    string extension = Path.GetExtension(repl.Replpic.FileName);
                    repl.Picname = filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/Replimage/", filename);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await repl.Replpic.CopyToAsync(fileStream);
                    }
                    _context.Update(repl);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReplExists(repl.IdRepl))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CommentId"] = new SelectList(_context.Comments, "CommentId", "CommentId", repl.CommentId);
            return View(repl);
        }

        // GET: Repls/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Repls == null)
            {
                return NotFound();
            }

            var repl = await _context.Repls
                .Include(r => r.Comment)
                .FirstOrDefaultAsync(m => m.IdRepl == id);
            if (repl == null)
            {
                return NotFound();
            }

            return View(repl);
        }

        // POST: Repls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Repls == null)
            {
                return Problem("Entity set 'MyForumDbContext.Repls'  is null.");
            }
            var repl = await _context.Repls.FindAsync(id);
            if (repl != null)
            {
                var imagepath = Path.Combine(_HostEnvironment.WebRootPath, "CommentImage", repl.Picname);
                if (System.IO.File.Exists(imagepath))
                    System.IO.File.Delete(imagepath);
                _context.Repls.Remove(repl);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReplExists(int id)
        {
          return _context.Repls.Any(e => e.IdRepl == id);
        }
    }
}
