using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using P230_Pronia.DAL;
using P230_Pronia.Entities;
using P230_Pronia.ViewModels.Cookie;

namespace P230_Pronia.Controllers
{
	public class ShopController : Controller
	{
		readonly ProniaDbContext _context;

        public ShopController(ProniaDbContext context)
		{
			_context = context;
		}
		public IActionResult  index()
        {
            List<Plant> plants = _context.Plants.
                  Include(p => p.PlantTags).ThenInclude(tp => tp.Tag).
                  Include(p => p.PlantCategories).ThenInclude(cp => cp.Category).
                  Include(p => p.PlantDeliveryInformation).
                   Include(p => p.PlantImages)
                  .ToList();
            return View(plants);
        }
        public IActionResult Detail(int id, int categoryId)
        {  
            Plant? plant = _context.Plants
                  .Include(p => p.PlantTags)
                  .ThenInclude(tp => tp.Tag)
                  .Include(p => p.PlantCategories)
                  .ThenInclude(cp => cp.Category)
                  .Include(p => p.PlantDeliveryInformation)
                  .Include(p => p.PlantImages)
                  .FirstOrDefault(x => x.Id == id);

          
           
            ViewBag.RelatedPlants = _context.Plants
                .Include(p => p.PlantImages)
                .Include(pc => pc.PlantCategories).ThenInclude(c=>c.Category)
                .Where(p => p.PlantCategories.Any(c => c.Category.Id == categoryId))
                .ToList();

            return View(plant);
        }


        public IActionResult AddBasket(int id)
        {
            
            if (id <= 0) return NotFound();
            Plant plant = _context.Plants.FirstOrDefault(p => p.Id == id);
            if (plant is null) return NotFound();
            var cookies = HttpContext.Request.Cookies["basket"];
            CookieBasketVM basket = new();
            if (cookies is null)
            {
                CookieBasketItemVM item = new CookieBasketItemVM
                {
                    Id = plant.Id,
                    Quantity = 1,
                    Price = (double)plant.Price
                };
                basket.CookieBasketItemVMs.Add(item);
                basket.TotalPrice = (double)plant.Price;
            }
            else
            {
                basket = JsonConvert.DeserializeObject<CookieBasketVM>(cookies);
                CookieBasketItemVM existed = basket.CookieBasketItemVMs.Find(c => c.Id == id);
                if (existed is null)
                {
                    CookieBasketItemVM newItem = new()
                    {
                        Id = plant.Id,
                        Quantity = 1,
                        Price = (double)plant.Price
                    };
                    basket.CookieBasketItemVMs.Add(newItem);
                    basket.TotalPrice += newItem.Price;
                }
                else
                {
                    existed.Quantity++;
                    basket.TotalPrice += existed.Price;
                }
               
            }
            var basketStr = JsonConvert.SerializeObject(basket);

            HttpContext.Response.Cookies.Append("basket", basketStr);

            return RedirectToAction("Index", "Home");

        }




    }
}
