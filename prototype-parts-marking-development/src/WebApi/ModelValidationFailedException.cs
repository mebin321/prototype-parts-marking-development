namespace WebApi
{
    using System;
    using Microsoft.AspNetCore.Mvc;

    public class ModelValidationFailedException : Exception
    {
        public ModelValidationFailedException(ValidationProblemDetails validationProblemDetails)
        {
            ValidationProblemDetails = validationProblemDetails;
        }

        public ValidationProblemDetails ValidationProblemDetails { get; }
    }
}
