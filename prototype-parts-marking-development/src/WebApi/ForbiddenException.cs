namespace WebApi
{
    using System.Net;
    using Microsoft.AspNetCore.Mvc;

    public class ForbiddenException : ApiException
    {
        public ForbiddenException(ProblemDetails problemDetails)
            : base(HttpStatusCode.Forbidden, problemDetails)
        {
        }
    }
}