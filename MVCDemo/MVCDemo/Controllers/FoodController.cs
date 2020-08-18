using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLibrary.Data;
using Microsoft.AspNetCore.Mvc;

namespace MVCDemo.Controllers
{
    public class FoodController : Controller
    {
        private readonly IFoodData foodData;

        public FoodController(IFoodData foodData)
        {
            this.foodData = foodData;
        }

        public async Task<IActionResult> Index()
        {
            var food = await foodData.GetFood();//getting food list
            return View(food); //passing the food list to the view
        }
    }
}
