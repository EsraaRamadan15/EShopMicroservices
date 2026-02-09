namespace Shopping.Web.Models.Catalog
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
    }

    public record GetProductsResponse(IEnumerable<Product> Products);
    public record GetProductsByCateogryResponse(IEnumerable<Product> Products);


    public record GetProductByIdResponse(Product Product);
}
