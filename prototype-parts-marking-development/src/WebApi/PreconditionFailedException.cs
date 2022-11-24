namespace WebApi
{
    using System.Net;
    using Microsoft.AspNetCore.Mvc;

    public class PreconditionFailedException : ApiException
    {
        public PreconditionFailedException(ProblemDetails problemDetails)
            : base(HttpStatusCode.PreconditionFailed, problemDetails)
        {
        }
    }
}
