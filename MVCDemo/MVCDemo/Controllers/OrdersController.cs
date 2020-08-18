using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLibrary.Data;
using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVCDemo.Models;

namespace MVCDemo.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IFoodData foodData;
        private readonly IOrderData orderData;

        public OrdersController(IFoodData foodData, IOrderData orderData)
        {
            this.foodData = foodData;
            this.orderData = orderData;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create() //create page
        {
            var food = await foodData.GetFood();

            OrderCreateModel orderCreateModel = new OrderCreateModel();

            food.ForEach(x =>
            {
                orderCreateModel.FoodItems.Add(new SelectListItem { Value = x.Id.ToString(), Text = x.Title }); // filling the dropdown list - creating a new listitem inside the Add method
            });

            return View(orderCreateModel); // instead of returning the page like with RP, returning the view with the model in it
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderModel order) //overload for creating order
        {
            if (ModelState.IsValid == false)
            {
                return View(); //mvc - return Page() for razor pages
            }

            var food = await foodData.GetFood();

            order.Total = order.Quantity * food.Where(x => x.Id == order.FoodId).First().Price;

            int id = await orderData.CreateOrder(order);

            return RedirectToAction("Display", new { id }); //in mvc each method is called an action
        }

        public async Task<IActionResult> Display(int id)
        {
            OrderDisplayModel displayOrder = new OrderDisplayModel(); //new display order instance
            displayOrder.Order = await orderData.GetOrderById(id); //getting order by id from data layer

            if (displayOrder.Order != null) //if there is an order 
            {
                var food = await foodData.GetFood(); //get food list

                displayOrder.ItemPurchased = food.Where(x => x.Id == displayOrder.Order.FoodId).FirstOrDefault()?.Title; //find food in list whose id matches the order id & grab the title of it
            }

            return View(displayOrder); //returning th view
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, string orderName)
        {
            await orderData.UpdateOrderName(id, orderName);

            return RedirectToAction("Display", new { id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var order = await orderData.GetOrderById(id);

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(OrderModel order)
        {
            await orderData.DeleteOrder(order.Id);

            return RedirectToAction("Create");
        }
    }
}
