namespace FastFood.Web.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Linq;

    using Data;
    using ViewModels.Orders;
    using AutoMapper.QueryableExtensions;
    using FastFood.Models;
    using Microsoft.EntityFrameworkCore;

    public class OrdersController : Controller
    {
        private readonly FastFoodContext context;
        private readonly IMapper mapper;

        public OrdersController(FastFoodContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public IActionResult Create()
        {
            var viewOrder = new CreateOrderViewModel()
            {
                Employees = this.context
                  .Employees
                  .OrderBy(e => e.Id)
                  .Select(x => new OrderEmployeeViewModel
                  {
                      EmployeeId = x.Id,
                      EmployeeName = x.Name
                  })
                  .ToList(),
                Items = this.context
                .Items
                .OrderBy(i => i.Id)
                .Select(x => new OrderItemViewModel
                {
                    ItemId = x.Id,
                    ItemName = x.Name
                })
                .ToList(),
            };

            return this.View(viewOrder);
        }

        [HttpPost]
        public IActionResult Create(CreateOrderInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Error", "Home");
            }

            var order = this.mapper.Map<Order>(model);
            order.DateTime = DateTime.UtcNow;

            this.context.Orders.Add(order);
            this.context.SaveChanges();

            return this.RedirectToAction("All", "Orders");
        }

        public IActionResult All()
        {
            var orders = this.context
                .Orders
                .ProjectTo<OrderAllViewModel>(mapper.ConfigurationProvider)
                .ToList();

            return this.View(orders);
        }
    }
}
