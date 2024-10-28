

namespace Catalog.API.Products.CreateProduct
{
    public record CreateProductResult(Guid Id);
    public record CreateProductCommand(string Name, List<string> Category, string Description, string ImageFile, decimal Price) :
        ICommand<CreateProductResult>;
    internal class CreateProductCommandHandler(IDocumentSession session) : ICommandHandler<CreateProductCommand, CreateProductResult>
    {
        public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            // -- Business logic to add product---//

            var product = new Product
            {
                Category = command.Category,
                Description = command.Description,
                ImageFile = command.ImageFile,
                Price = command.Price,
                Name = command.Name,
            };

            //---save in database
            session.Store(product);
            await session.SaveChangesAsync();
            return new CreateProductResult(product.Id);
        }
    }
}
