namespace ProductShop.DTO
{
    public class SoldProductsWithCountDto
    {
        public int Count { get; set; }

        public ProductDetailsDto[] Products { get; set; }
    }
}
