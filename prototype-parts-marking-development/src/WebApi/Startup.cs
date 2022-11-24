namespace WebApi
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using Autofac;
    using Common.ActiveDirectory;
    using Data;
    using Features.Prototypes.Services;
    using FluentValidation.AspNetCore;
    using Hangfire;
    using Hangfire.PostgreSql;
    using MediatR;
    using MediatrPipeline;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using Prometheus;
    using Serilog;
    using WebApi.BackgroundJobs;
    using WebApi.Common;
    using WebApi.Common.PrototypeIdentifier;
    using WebApi.Common.ResourceVersioning;
    using WebApi.Common.Sorting;
    using WebApi.Configuration;
    using WebApi.Features.Authentication.Services;
    using WebApi.Features.Authorization.Policies;
    using WebApi.Features.GlobalProjects.Services;

    public class Startup
    {
        private const string CorsPolicy = "DefaultCorsPolicy";

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddMvc().AddFluentValidation(c =>
            {
                c.RegisterValidatorsFromAssemblyContaining<Startup>();
                c.DisableDataAnnotationsValidation = true;
            });
            services.Configure<ApiBehaviorOptions>(o =>
            {
                o.SuppressModelStateInvalidFilter = true;
            });

            services.AddMediatR(typeof(Startup));

            services.AddControllers(o => o.ReturnHttpNotAcceptable = true);

            services.AddDbContextFactory<PrototypePartsDbContext>(o =>
            {
                o.UseNpgsql(Configuration.GetConnectionString("PPMT"));
            });

            services.Configure<Authentication>(Configuration.GetSection("Authentication"));
            services.Configure<ActiveDirectoryDomains>(Configuration.GetSection("ActiveDirectory"));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Prototype Parts Marking", Version = "v1" });

                var scheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                };

                c.AddSecurityDefinition("Bearer", scheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        scheme,
                        new List<string>()
                    },
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.EnableAnnotations();
            });

            services.AddCors(o =>
            {
                o.AddPolicy(CorsPolicy, policy =>
                {
                    // TODO AllowAnyOrigin is an insecure configuration
                    // https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.1`
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithExposedHeaders("WWW-Authenticate")
                        .WithExposedHeaders("ETag")
                        .AllowAnyOrigin();
                });
            });

            services
                .AddAuthorization(o =>
                {
                    o.AddPolicy(nameof(CanModifyLocation), b => b.AddRequirements(new CanModifyLocation()));
                    o.AddPolicy(nameof(CanListAdUsers), b => b.AddRequirements(new CanListAdUsers()));
                    o.AddPolicy(nameof(CanModifyEvidenceYear), b => b.AddRequirements(new CanModifyEvidenceYear()));
                    o.AddPolicy(nameof(CanModifyGateLevel), b => b.AddRequirements(new CanModifyGateLevel()));
                    o.AddPolicy(nameof(CanPerformMaintenance), b => b.AddRequirements(new CanPerformMaintenance()));
                    o.AddPolicy(nameof(CanModifyOutlet), b => b.AddRequirements(new CanModifyOutlet()));
                    o.AddPolicy(nameof(CanModifyPart), b => b.AddRequirements(new CanModifyPart()));
                    o.AddPolicy(nameof(CanModifyProductGroup), b => b.AddRequirements(new CanModifyProductGroup()));
                    o.AddPolicy(nameof(CanCreatePrototypeSet), b => b.AddRequirements(new CanCreatePrototypeSet()));
                    o.AddPolicy(nameof(CanModifyRole), b => b.AddRequirements(new CanModifyRole()));
                    o.AddPolicy(nameof(CanModifyUserRole), b => b.AddRequirements(new CanModifyUserRole()));
                    o.AddPolicy(nameof(CanModifyUsers), b => b.AddRequirements(new CanModifyUsers()));
                    o.AddPolicy(nameof(CanDisableUsers), b => b.AddRequirements(new CanDisableUsers()));
                    o.AddPolicy(nameof(CanModifyPrototypeVariants), b => b.AddRequirements(new CanModifyPrototypeVariants()));
                    o.AddPolicy(nameof(CanModifyPrototypePackages), b => b.AddRequirements(new CanModifyPrototypePackages()));
                    o.AddPolicy(nameof(CanCreatePrototypePackages), b => b.AddRequirements(new CanCreatePrototypePackages()));
                    o.AddPolicy(nameof(CanScrapPrototypes), b => b.AddRequirements(new CanScrapPrototypes()));
                    o.AddPolicy(nameof(CanReactivatePrototypes), b => b.AddRequirements(new CanReactivatePrototypes()));
                    o.AddPolicy(nameof(CanModifyPrototypes), b => b.AddRequirements(new CanModifyPrototypes()));
                    o.AddPolicy(nameof(CanCreatePrototypes), b => b.AddRequirements(new CanCreatePrototypes()));
                    o.AddPolicy(nameof(CanModifyEntityRelation), b => b.AddRequirements(new CanModifyEntityRelation()));
                    o.AddPolicy(nameof(CanReactivatePrototypesPackages), b => b.AddRequirements(new CanReactivatePrototypesPackages()));
                    o.AddPolicy(nameof(CanScrapPrototypesPackages), b => b.AddRequirements(new CanScrapPrototypesPackages()));
                    o.AddPolicy(nameof(CanReactivatePrototypeSets), b => b.AddRequirements(new CanReactivatePrototypeSets()));
                    o.AddPolicy(nameof(CanScrapPrototypeSets), b => b.AddRequirements(new CanScrapPrototypeSets()));
                    o.AddPolicy(nameof(CanEnableUsers), b => b.AddRequirements(new CanEnableUsers()));
                    o.AddPolicy(nameof(CanReadEntityRelations), b => b.AddRequirements(new CanReadEntityRelations()));
                    o.AddPolicy(nameof(CanModifyPrintingLabels), b => b.AddRequirements(new CanModifyPrintingLabels()));
                })
                .AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Authentication:TokenSigningKey"])),
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                    };
                });

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.Configure<QueryPartitionerOptions>(o => o.PartitionSize = 1000);
        }

        public void ConfigureProductionServices(IServiceCollection services)
        {
            ConfigureServices(services);

            services
                .AddHangfire(c =>
                {
                    c.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UsePostgreSqlStorage(Configuration.GetConnectionString("Hangfire"));
                })
                .AddHangfireServer();

            services.AddHealthChecks()
                .AddNpgSql(npgsqlConnectionString: Configuration.GetConnectionString("PPMT"))
                .ForwardToPrometheus();
        }

        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<UtcDateTime>().As<IDateTime>();
            builder.RegisterType<MonikerFormatter>().As<IMonikerFormatter>();
            builder.RegisterType<ResourceVersionManager>().As<IResourceVersionManager>();
            builder.RegisterType<ETagGenerator>().As<IETagGenerator>();
            builder.RegisterType<UrlCreator>().As<IUrlCreator>();

            builder.RegisterType<PrototypeIdentifierGenerator>().As<IPrototypeIdentifierGenerator>();
            builder.RegisterType<Base36Converter>().As<IPrototypeCounterConverter>();
            builder.RegisterType<PrototypeIdentifierCounter>().As<IPrototypeIdentifierCounter>();
            builder.RegisterDecorator<PrototypeIdentifierCounterSynchronizationDecorator, IPrototypeIdentifierCounter>();

            builder.RegisterType<CurrentUserAccessor>().As<ICurrentUserAccessor>();
            builder.RegisterType<TokenGenerator>().As<ITokenGenerator>();

            builder.RegisterType<ActiveDirectoryProxy>().As<IActiveDirectory>();

            if (Configuration.GetSection("Authentication").GetValue<bool>("UseAd"))
            {
                builder.RegisterType<ActiveDirectoryValidator>().As<ICredentialsValidator>();
            }
            else
            {
                builder.RegisterType<StubCredentialsValidator>().As<ICredentialsValidator>();
            }

            builder
                .RegisterType<ProblemDetailsAdapterFactory>()
                .As<IProblemDetailsFactory>()
                .InstancePerLifetimeScope();

            builder
                .RegisterAssemblyTypes(typeof(Startup).Assembly)
                .As<IAuthorizationHandler>()
                .SingleInstance();

            builder.RegisterInstance(new MetricsBehaviorOptions { DefaultThreshold = TimeSpan.FromSeconds(2) }).AsSelf();
            builder.RegisterGeneric(typeof(LoggingContextBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(MetricsBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(RequestValidationBehavior<,>)).As(typeof(IPipelineBehavior<,>));

            builder.RegisterType<CobraRepository>().As<ICobraRepository>();
            builder.RegisterType<QueryPartitioner>().As<IQueryPartitioner>();

            builder.AddSortColumnMapping<Prototype>(m =>
            {
                m["SetId"] = nameof(Prototype.PrototypeSetId);
                m["PartTypeCode"] = nameof(Prototype.PartTypeCode);
                m["PartTypeTitle"] = nameof(Prototype.PartTypeTitle);
                m["Type"] = nameof(Prototype.Type);
                m["Index"] = nameof(Prototype.Index);
                m["MaterialNumber"] = nameof(Prototype.MaterialNumber);
                m["RevisionCode"] = nameof(Prototype.RevisionCode);
                m["OwnerId"] = nameof(Prototype.OwnerId);
                m["CreatedAt"] = nameof(Prototype.CreatedAt);
                m["ModifiedAt"] = nameof(Prototype.ModifiedAt);
                m["DeletedAt"] = nameof(Prototype.DeletedAt);
            });
            builder.AddSortColumnMapping<PrototypeSet>(m =>
            {
                m["OutletCode"] = nameof(PrototypeSet.OutletCode);
                m["OutletTitle"] = nameof(PrototypeSet.OutletTitle);
                m["ProductGroupCode"] = nameof(PrototypeSet.ProductGroupCode);
                m["ProductGroupTitle"] = nameof(PrototypeSet.ProductGroupTitle);
                m["GateLevelCode"] = nameof(PrototypeSet.GateLevelCode);
                m["GateLevelTitle"] = nameof(PrototypeSet.GateLevelTitle);
                m["EvidenceYearCode"] = nameof(PrototypeSet.EvidenceYearCode);
                m["EvidenceYearTitle"] = nameof(PrototypeSet.EvidenceYearTitle);
                m["LocationCode"] = nameof(PrototypeSet.LocationCode);
                m["LocationTitle"] = nameof(PrototypeSet.LocationTitle);
                m["SetIdentifier"] = nameof(PrototypeSet.SetIdentifier);
                m["CreatedAt"] = nameof(PrototypeSet.CreatedAt);
                m["ModifiedAt"] = nameof(PrototypeSet.ModifiedAt);
                m["DeletedAt"] = nameof(PrototypeSet.DeletedAt);
            });
            builder.AddSortColumnMapping<PrototypesPackage>(m =>
            {
                m["OutletCode"] = nameof(PrototypesPackage.OutletCode);
                m["OutletTitle"] = nameof(PrototypesPackage.OutletTitle);
                m["ProductGroupCode"] = nameof(PrototypesPackage.ProductGroupCode);
                m["ProductGroupTitle"] = nameof(PrototypesPackage.ProductGroupTitle);
                m["GateLevelCode"] = nameof(PrototypesPackage.GateLevelCode);
                m["GateLevelTitle"] = nameof(PrototypesPackage.GateLevelTitle);
                m["EvidenceYearCode"] = nameof(PrototypesPackage.EvidenceYearCode);
                m["EvidenceYearTitle"] = nameof(PrototypesPackage.EvidenceYearTitle);
                m["LocationCode"] = nameof(PrototypesPackage.LocationCode);
                m["LocationTitle"] = nameof(PrototypesPackage.LocationTitle);
                m["PartTypeCode"] = nameof(PrototypesPackage.PartTypeCode);
                m["PartTypeTitle"] = nameof(PrototypesPackage.PartTypeTitle);
                m["PackageIdentifier"] = nameof(PrototypesPackage.PackageIdentifier);
                m["InitialCount"] = nameof(PrototypesPackage.InitialCount);
                m["ActualCount"] = nameof(PrototypesPackage.ActualCount);
                m["OwnerId"] = nameof(PrototypesPackage.OwnerId);
                m["CreatedAt"] = nameof(PrototypesPackage.CreatedAt);
                m["ModifiedAt"] = nameof(PrototypesPackage.ModifiedAt);
                m["DeletedAt"] = nameof(PrototypesPackage.DeletedAt);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<CorrelationIdMiddleware>();

            app.UseSerilogRequestLogging();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();
            if (env.IsProduction() || env.IsStaging())
            {
                app.UseHttpMetrics();
            }

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseCors(CorsPolicy);

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Prototype Parts Marking");
                c.RoutePrefix = "api/v1";
                c.EnableFilter();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapFallbackToFile("index.html");
                endpoints.MapControllers().RequireCors(CorsPolicy);

                if (env.IsProduction() || env.IsStaging())
                {
                    endpoints.MapHangfireDashboard("/hangfire-internal");
                    endpoints.MapHangfireDashboard(new DashboardOptions
                    {
                        IsReadOnlyFunc = _ => true,
                        Authorization = new[]
                        {
                            new PublicDashboardAccess(),
                        },
                        DisplayStorageConnectionString = false,
                    });
                    endpoints.MapMetrics();
                }
            });

            if (env.IsProduction() || env.IsStaging())
            {
                RecurringJob.AddOrUpdate<RemoveExpiredRefreshTokensJob>(
                    nameof(RemoveExpiredRefreshTokensJob),
                    job => job.Execute(),
                    "0 0 * * *");

                RecurringJob.AddOrUpdate<SynchronizeGlobalPsDataJob>(
                    nameof(SynchronizeGlobalPsDataJob),
                    job => job.Execute(CancellationToken.None),
                    "0 * * * *");
            }
        }
    }
}
