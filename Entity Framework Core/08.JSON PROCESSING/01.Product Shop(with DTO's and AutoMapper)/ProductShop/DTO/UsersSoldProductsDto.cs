namespace ProductShop.DTO
{
    public class UsersSoldProductsDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public SoldProductsDto[] SoldProducts { get; set; }
    }
}
