namespace WebApi
{
    using System.Net;
    using Microsoft.AspNetCore.Mvc;

    public class BadRequestException : ApiException
    {
        public BadRequestException(ProblemDetails problemDetails)
            : base(HttpStatusCode.BadRequest, problemDetails)
        {
        }
    }
}