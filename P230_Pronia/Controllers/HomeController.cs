using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P230_Pronia.DAL;
using P230_Pronia.Entities;
using System.Diagnostics;

namespace P230_Pronia.Controllers
{
    public class HomeController : Controller
    {
        readonly ProniaDbContext _context;

        public HomeController(ProniaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Slider> slider = _context.Sliders.OrderBy(s=>s.Order).ToList();

         
                                            ViewBag.RelatedPlants = _context.Plants
                .Include(p => p.PlantImages)
                .Include(pc => pc.PlantCategories).ThenInclude(c => c.Category)
                                            .Take(8)
                                            .ToList();
            return View(slider);
        }
    }
}