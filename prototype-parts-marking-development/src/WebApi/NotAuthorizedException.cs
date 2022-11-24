namespace WebApi
{
    using System.Net;
    using Microsoft.AspNetCore.Mvc;

    public class NotAuthorizedException : ApiException
    {
        public NotAuthorizedException(ProblemDetails problemDetails)
            : base(HttpStatusCode.Unauthorized, problemDetails)
        {
        }
    }
}