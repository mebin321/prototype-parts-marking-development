namespace WebApi
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;

    public interface IProblemDetailsFactory
    {
        ProblemDetails Create(int statusCode, string title, string detail);

        ValidationProblemDetails CreateValidationProblemDetails(Dictionary<string, string[]> errors);
    }
}