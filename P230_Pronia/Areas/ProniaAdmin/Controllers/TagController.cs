using Microsoft.AspNetCore.Mvc;
using P230_Pronia.DAL;
using P230_Pronia.Entities;
using P230_Pronia.Migrations;


namespace P230_Pronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class TagController : Controller
    {
        private readonly ProniaDbContext _context;

        public TagController(ProniaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<Tag> tags = _context.Tags.AsEnumerable();
            return View(tags);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        [AutoValidateAntiforgeryToken]
        public IActionResult Creates(Tag newTag)
        {
            if (!ModelState.IsValid)
            {
                var existingTag = _context.Tags.FirstOrDefault(c => c.Name == newTag.Name);
                if (existingTag == null)
                {
                    _context.Tags.Add(newTag);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("Name", "You cannot duplicate category name");
                }

            }
            return View(newTag);
        }

        public IActionResult Edit(int id)
        {
            if (id == 0) return NotFound();
            Tag tag = _context.Tags.FirstOrDefault(c => c.Id == id);
            if (tag is null) return NotFound();
            return View(tag);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(int id, Tag edited)
        {
            if (id != edited.Id) return BadRequest();
            Tag tag = _context.Tags.FirstOrDefault(c => c.Id == id);
            if (tag is null) return NotFound();
            bool duplicate = _context.Tags.Any(c => c.Name == edited.Name);
            if (duplicate)
            {
                ModelState.AddModelError("", "You cannot duplicate category name");
                return View();
            }
            tag.Name = edited.Name;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Detail(int id)
        {

            if (id == 0) return NotFound();
            Tag tag = _context.Tags.FirstOrDefault(c => c.Id == id);
            if (tag is null) return NotFound();
            return View(tag);
        }

        public IActionResult Delete(int id)
        {
            if (id == 0) return NotFound();
            Tag tag = _context.Tags.FirstOrDefault(c => c.Id == id);
            if (tag is null) return NotFound();

            return View(tag);
        }


        [HttpPost]
        public IActionResult Delete(int id, Category delete)
        {
            if (id == 0) return NotFound();
            Tag tag = _context.Tags.FirstOrDefault(c => c.Id == id);
            if (tag is null) return NotFound();


            if (id == delete.Id)
            {
                _context.Tags.Remove(tag);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(tag);
        }

    }
}

