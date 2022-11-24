namespace WebApi
{
    using System;
    using System.Net;
    using Microsoft.AspNetCore.Mvc;

    public class ApiException : Exception
    {
        public ApiException(HttpStatusCode statusCode, ProblemDetails problemDetails)
        {
            StatusCode = statusCode;
            ProblemDetails = problemDetails;
        }

        public HttpStatusCode StatusCode { get; }

        public ProblemDetails ProblemDetails { get; }
    }
}