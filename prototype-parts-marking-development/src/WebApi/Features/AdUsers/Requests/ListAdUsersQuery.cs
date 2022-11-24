namespace WebApi.Features.AdUsers.Requests
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Common.ActiveDirectory;
    using Common.Paging;
    using MediatR;
    using Models;
    using Utilities;

    public class ListAdUsersQuery : IRequest<PagedDataDto<AdUserDto>>
    {
        private const int MaxPageSize = 100;

        private int page = 1;
        private int pageSize = 20;

        [Range(1, double.PositiveInfinity)]
        public int Page
        {
            get => page;
            set => page = Math.Max(1, value);
        }

        [Range(1, MaxPageSize)]
        public int PageSize
        {
            get => pageSize;
            set => pageSize = Math.Clamp(value, 1, MaxPageSize);
        }

        public string Username { get; set; }

        public string Email { get; set; }

        public class Handler : RequestHandler<ListAdUsersQuery, PagedDataDto<AdUserDto>>
        {
            private readonly IActiveDirectory activeDirectory;

            public Handler(IActiveDirectory activeDirectory)
            {
                Guard.NotNull(activeDirectory, nameof(activeDirectory));

                this.activeDirectory = activeDirectory;
            }

            protected override PagedDataDto<AdUserDto> Handle(ListAdUsersQuery request)
            {
                var results = activeDirectory.FindUsers(request.Username, request.Email);

                var paged = results.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize);

                var users = new PagedList<AdUser>(paged, results.Count, request.Page, request.PageSize);

                return new PagedDataDto<AdUserDto>
                {
                    Items = users.Select(u => AdUserDto.From(u)).ToList(),
                    Pagination = users.CreatePagination(),
                };
            }
        }
    }
}
