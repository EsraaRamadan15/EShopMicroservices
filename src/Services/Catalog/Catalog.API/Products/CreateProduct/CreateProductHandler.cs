﻿namespace Catalog.API.Products.CreateProduct
{
    public record CreateProductResult(Guid Id);
    public record CreateProductCommand(string Name, List<string> Category, string Description, string ImageFile, decimal Price) :
        ICommand<CreateProductResult>;

    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is Required");
            RuleFor(x => x.Category).NotEmpty().WithMessage("Category is required");
            RuleFor(x => x.ImageFile).NotEmpty().WithMessage("ImageFile is required");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
        }
    }
    internal class CreateProductCommandHandler
        (IDocumentSession session) : ICommandHandler<CreateProductCommand, CreateProductResult>
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
