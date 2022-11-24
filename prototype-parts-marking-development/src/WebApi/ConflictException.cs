namespace WebApi
{
    using System.Net;
    using Microsoft.AspNetCore.Mvc;

    public class ConflictException : ApiException
    {
        public ConflictException(ProblemDetails problemDetails)
            : base(HttpStatusCode.Conflict, problemDetails)
        {
        }
    }
}