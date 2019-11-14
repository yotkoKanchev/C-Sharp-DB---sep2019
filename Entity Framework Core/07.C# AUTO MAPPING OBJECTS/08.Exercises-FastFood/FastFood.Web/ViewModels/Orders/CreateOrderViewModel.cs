namespace FastFood.Web.ViewModels.Orders
{
    using System;
    using System.Collections.Generic;

    public class CreateOrderViewModel
    {
        public List<OrderEmployeeViewModel> Employees { get; set; }

        public List<OrderItemViewModel> Items { get; set; }

    }
}
