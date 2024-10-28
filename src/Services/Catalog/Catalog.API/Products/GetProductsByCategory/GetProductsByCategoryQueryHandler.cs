
namespace Catalog.API.Products.GetProductByCategory
{

    public record GetProductsByCategoryQuery(string category) : IQuery<GetProductByCategoryResult>;
    public record GetProductByCategoryResult(IEnumerable<Product> Products);
    internal class GetProductsByCategoryQueryHandler(IDocumentSession session,
            ILogger<GetProductsByCategoryQueryHandler> logger) : IQueryHandler<GetProductsByCategoryQuery, GetProductByCategoryResult>
    {
        public async Task<GetProductByCategoryResult> Handle(GetProductsByCategoryQuery query, CancellationToken cancellationToken)
        {
            logger.LogInformation("GetProductByCategoryQueryHandler.handle called with {@query}", query);
            var products = await session.Query<Product>()
                .Where(q => q.Category.Contains(query.category))
                .ToListAsync(cancellationToken);

            return new GetProductByCategoryResult(products);
        }
    }
}


