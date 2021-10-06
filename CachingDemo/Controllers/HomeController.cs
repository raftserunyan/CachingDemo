using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CachingDemo.Data;
using CachingDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CachingDemo.Controllers
{
	public class HomeController : Controller
	{
		private readonly AppDbContext _context;
		private readonly IMemoryCache _cache;

		public HomeController(AppDbContext context, IMemoryCache cache)
		{
			_context = context;
			_cache = cache;
		}

		//If you pass to this action an argument with value 1, it's gonna use memory cache
		public IActionResult Fruits(int a)
		{
			var sw = new Stopwatch();
			List<Fruit> fruits;
			sw.Reset();

			sw.Start();

			if (a == 1)
			{
				if (!_cache.TryGetValue("fruitsList", out fruits))
				{
					fruits = _context.Fruits.ToList();

					var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(60));

					_cache.Set("fruitsList", fruits, cacheEntryOptions);
				}
			}
			else
			{
				fruits = _context.Fruits.ToList();
			}
			

			sw.Stop();

			return Content($"Time elapsed to read the data:\n {sw.Elapsed.ToString()}");
		}



		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
