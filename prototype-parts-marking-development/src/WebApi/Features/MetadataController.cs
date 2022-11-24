namespace WebApi.Features
{
    using System;
    using System.Reflection;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    [ApiController]
    [Route("api")]
    public class MetadataController : ControllerBase
    {
        private static readonly string AppVersion = typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "N/A";
        private static readonly string ClrVersion = typeof(object).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "N/A";
        private static readonly string FrameworkVersion = typeof(Uri).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "N/A";

        [AllowAnonymous]
        [HttpGet]
        [Produces(MediaTypes.ApplicationJson)]
        public IActionResult GetApiMetadata()
        {
            return Ok(new
            {
                Version = AppVersion,
                CoreCLR = ClrVersion,
                CoreFX = FrameworkVersion,
            });
        }
    }
}
