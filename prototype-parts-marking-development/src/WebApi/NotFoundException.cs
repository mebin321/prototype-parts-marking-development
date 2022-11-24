namespace WebApi
{
    using System.Net;
    using Microsoft.AspNetCore.Mvc;

    public class NotFoundException : ApiException
    {
        public NotFoundException(ProblemDetails problemDetails)
            : base(HttpStatusCode.NotFound, problemDetails)
        {
        }
    }
}