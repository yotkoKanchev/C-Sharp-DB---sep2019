using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.DTO
{
    public class CustomerInfoDto
    {
        public CarInfoDto car { get; set; }

        public string customerName { get; set; }

        public string Discount { get; set; }

        public string price { get; set; }

        public string priceWithDiscount { get; set; }
    }
}
