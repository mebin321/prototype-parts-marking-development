namespace WebApi.Features.ProductGroup.Requests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Utilities;

    public class UpdateProductGroupCommand : IRequest<ProductGroupDto>
    {
        public string Moniker { get; set; }

        public string Description { get; set; }

        public class Handler : IRequestHandler<UpdateProductGroupCommand, ProductGroupDto>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;
            private readonly IProblemDetailsFactory problemDetailsFactory;

            public Handler(IDbContextFactory<PrototypePartsDbContext> dbContextFactory, IProblemDetailsFactory problemDetailsFactory)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));
                Guard.NotNull(problemDetailsFactory, nameof(problemDetailsFactory));

                this.dbContextFactory = dbContextFactory;
                this.problemDetailsFactory = problemDetailsFactory;
            }

            public async Task<ProductGroupDto> Handle(UpdateProductGroupCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var productGroup = await dbContext.ProductGroups
                    .FirstOrDefaultAsync(l => l.Moniker == request.Moniker);

                if (productGroup is null)
                {
                    throw new NotFoundException(problemDetailsFactory.NotFound(
                        "Product Group not found.",
                        $"Could not find Product Group with ID {request.Moniker}."));
                }

                productGroup.Description = request.Description;
                await dbContext.SaveChangesAsync();

                return ProductGroupDto.From(productGroup);
            }
        }

        public class Validator : AbstractValidator<UpdateProductGroupCommand>
        {
            public Validator()
            {
                RuleFor(r => r.Moniker).NotEmpty();
                RuleFor(r => r.Description).NotEmpty();
            }
        }
    }
}
