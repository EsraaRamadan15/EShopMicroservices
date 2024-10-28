using Catalog.API.Products.GetProducts;

namespace Catalog.API.Products.GetProductById
{
    public record GetProductByIdsResponse(Product Product);
    public class GetProductByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products/{id}", async (Guid id, ISender sender) =>
            {
                // var query = request.Adapt<GetProductByIdQuery>();

                var result = await sender.Send(new GetProductByIdQuery(id));

                var response = result.Adapt<GetProductByIdsResponse>();

                return Results.Ok(response);
            })
            .WithName("Get Product By Id")
            .Produces<GetProductsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Product By Id")
            .WithDescription("Get Product By Id");
        }
    }
}
