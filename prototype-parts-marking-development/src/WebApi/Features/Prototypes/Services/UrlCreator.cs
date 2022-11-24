namespace WebApi.Features.Prototypes.Services
{
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Utilities;

    public class UrlCreator : IUrlCreator
    {
        private readonly IUrlHelperFactory urlHelperFactory;

        private readonly IActionContextAccessor actionContextAccessor;

        public UrlCreator(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor)
        {
            Guard.NotNull(actionContextAccessor, nameof(actionContextAccessor));
            Guard.NotNull(urlHelperFactory, nameof(urlHelperFactory));

            this.urlHelperFactory = urlHelperFactory;
            this.actionContextAccessor = actionContextAccessor;
        }

        public string CreateUrl(string routeName, object values)
        {
            var urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
            return urlHelper.Link(routeName, values);
        }
    }
}
