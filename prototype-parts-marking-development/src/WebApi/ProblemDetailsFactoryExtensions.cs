namespace WebApi
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public static class ProblemDetailsFactoryExtensions
    {
        public static ProblemDetails NotFound(this IProblemDetailsFactory factory, string title, string details)
            => factory.Create(StatusCodes.Status404NotFound, title, details);

        public static NotFoundException EntityNotFound(this IProblemDetailsFactory factory, string entity, int id)
            => EntityNotFound(factory, entity, id.ToString());

        public static NotFoundException EntityNotFound(this IProblemDetailsFactory factory, string entity, string id)
        {
            return new NotFoundException(
                factory.Create(
                    StatusCodes.Status404NotFound,
                    $"{entity} not found.",
                    $"Could not find {entity} with ID {id}."));
        }

        public static ProblemDetails BadRequest(this IProblemDetailsFactory factory, string title, string details)
            => factory.Create(StatusCodes.Status400BadRequest, title, details);

        public static ProblemDetails Unauthorized(this IProblemDetailsFactory factory, string title, string details)
            => factory.Create(StatusCodes.Status401Unauthorized, title, details);

        public static ProblemDetails Forbidden(this IProblemDetailsFactory factory, string title, string details)
            => factory.Create(StatusCodes.Status403Forbidden, title, details);

        public static ProblemDetails Conflict(this IProblemDetailsFactory factory, string title, string details)
            => factory.Create(StatusCodes.Status409Conflict, title, details);

        public static ProblemDetails PreconditionFailed(this IProblemDetailsFactory factory, string title, string details)
            => factory.Create(StatusCodes.Status412PreconditionFailed, title, details);
    }
}