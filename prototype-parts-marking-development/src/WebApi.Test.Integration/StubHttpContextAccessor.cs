namespace WebApi.Test.Integration
{
    using Microsoft.AspNetCore.Http;

    public class StubHttpContextAccessor : IHttpContextAccessor
    {
        public HttpContext HttpContext { get; set; } = new DefaultHttpContext();
    }
}