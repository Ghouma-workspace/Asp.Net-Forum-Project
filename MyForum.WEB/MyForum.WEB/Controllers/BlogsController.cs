using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyForum.BL.Entities;
using MyForum.DAL;

namespace MyForum.WEB.Controllers
{
    public class BlogsController : Controller
    {
        private readonly MyForumDbContext _context;
        private readonly UserManager<User> _userManager;

        public BlogsController(MyForumDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Blogs
        public async Task<IActionResult> Index()
        {
            List<Theme> themes = new List<Theme>(_context.Themes);
            List<String> themeTitles = new List<String>();
            for (int i=0; i < themes.Count; ++i)
            {
                themeTitles.Add(themes[i].ThemeTitle);
                ViewData[themes[i].ThemeTitle] = themes[i].ThemebgColor;

            }
            ViewBag.Themes = themeTitles;
            var _users = await _userManager.Users.ToListAsync();
            foreach (var item in _users)
            {
                ViewData[item.Id.ToString()] = item.FirstName + " " + item.LastName;
            }
            return View(await _context.Blogs.ToListAsync());
        }

        //GET : Blogs of logged in user
        public async Task<IActionResult> MyBlogs()
        {
            List<Blog> blogs= new List<Blog>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            foreach (var item in _context.Blogs)
            {
                if (userId == item.Id)
                {
                    blogs.Add(item);
                }
            }
            List<Theme> themes = new List<Theme>(_context.Themes);
            foreach (var item in themes)
            {
                ViewData[item.ThemeId.ToString()] = item.ThemeTitle;
            }
            return View(blogs);
        }

        //Searching for a blog
        public async Task<IActionResult> Filter(string searchString)
        {
            var blogs = _context.Blogs;

            if (!string.IsNullOrEmpty(searchString))
            {
                //var filteredResult = allMovies.Where(n => n.Name.ToLower().Contains(searchString.ToLower()) || n.Description.ToLower().Contains(searchString.ToLower())).ToList();
                List<Theme> themes = new List<Theme>(_context.Themes);
                List<String> themeTitles = new List<String>();
                for (int i = 0; i < themes.Count; ++i)
                {
                    themeTitles.Add(themes[i].ThemeTitle);
                }
                ViewBag.Themes = themeTitles;
                var _users = await _userManager.Users.ToListAsync();
                foreach (var item in _users)
                {
                    ViewData[item.Id.ToString()] = item.FirstName + " " + item.LastName;
                }

                var result = blogs.Where(x => x.Url.Contains(searchString)).ToList();
                if (result == null)
                {
                    return NotFound();
                }

                return View("Index", result);
            }

            return View("Index", blogs);
        }

        // GET: Blogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            List<Post> p = new List<Post>();
            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .FirstOrDefaultAsync(m => m.BlogId == id); ;
            if (blog == null)
            {
                return NotFound();
            }
            foreach (var item in _context.Posts)
            {
                if (item.BlogId == id)
                {
                    p.Add(item);
                }
            }
            ViewData["idblog"] = id;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (userId == blog.Id)
            {
                ViewData["user_id"] = "true";
            }
            else
            {
                ViewData["user_id"] = "false";
            }

            return View(p);
        }

        // GET: Blogs/Create
        public IActionResult Create()
        {
            ViewData["ThemeId"] = new SelectList(_context.Themes, "ThemeId", "ThemeTitle");
            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,Url,Description,ThemeId")] Blog blog)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                blog.Id = userId;
                _context.Add(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(blog);
        }

        // GET: Blogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BlogId,Url,Description,ThemeId")] Blog blog)
        {
            if (id != blog.BlogId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(blog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.BlogId))
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
            return View(blog);
        }

        // GET: Blogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .FirstOrDefaultAsync(m => m.BlogId == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Blogs == null)
            {
                return Problem("Entity set 'MyForumDbContext.Blogs'  is null.");
            }
            var blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.BlogId == id);
        }
    }
}
