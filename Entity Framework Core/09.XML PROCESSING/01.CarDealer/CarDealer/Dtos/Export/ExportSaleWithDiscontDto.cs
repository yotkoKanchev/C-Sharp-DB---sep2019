﻿namespace CarDealer.Dtos.Export
{
    using System.Xml.Serialization;

    [XmlType("sale")]
    public class ExportSaleWithDiscontDto
    {
        [XmlElement("car")]
        public CarSaleDiscountDto Car { get; set; }

        [XmlElement("discount")]
        public decimal Discount { get; set; }

        [XmlElement("customer-name")]
        public string CustomerName { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }

        [XmlElement("price-with-discount")]
        public decimal PriceWithDiscount { get; set; }
    }
}
