namespace ProductShop.DTO
{
    public class UserDetailsDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int? Age { get; set; }

        public SoldProductsWithCountDto SoldProducts { get; set; }
    }
}
