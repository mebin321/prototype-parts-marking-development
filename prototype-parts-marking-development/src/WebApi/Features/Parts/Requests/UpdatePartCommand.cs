namespace WebApi.Features.Parts.Requests
{
    using System.ComponentModel.DataAnnotations;
    using System.Threading;
    using System.Threading.Tasks;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Utilities;

    public class UpdatePartCommand : IRequest<PartDto>
    {
        [Required]
        public string Moniker { get; set; }

        [Required]
        public string Description { get; set; }

        public class Handler : IRequestHandler<UpdatePartCommand, PartDto>
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

            public async Task<PartDto> Handle(UpdatePartCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var part = await dbContext.Parts
                    .FirstOrDefaultAsync(l => l.Moniker == request.Moniker)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(Part), request.Moniker));

                part.Description = request.Description;
                await dbContext.SaveChangesAsync();

                return PartDto.From(part);
            }
        }

        public class Validator : AbstractValidator<UpdatePartCommand>
        {
            public Validator()
            {
                RuleFor(r => r.Moniker).NotEmpty();
                RuleFor(r => r.Description).NotEmpty();
            }
        }
    }
}