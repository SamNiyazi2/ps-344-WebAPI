using AutoMapper;
using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;

namespace CourseLibrary.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // 03/08/2022 06:57 pm - SSN - [20220308-1856] - [001] - M07-06 - Demo - Adding a cache store with the ResponseCaching middleware
            services.AddResponseCaching();


            services.AddControllers(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;

                //// 03/07/2022 02:56 pm - SSN - [20220307-1105] - [010] - M06-06 - Demo - Tightening the contract between client and server with vendor-specific media types
                // We are using as an attribute.
                //setupAction.Filters.Add(new ActionFilters.ProducesActionFilter());

            }).AddNewtonsoftJson(setupAction =>
             {
                 setupAction.SerializerSettings.ContractResolver =
                    new CamelCasePropertyNamesContractResolver();
             })
             .AddXmlDataContractSerializerFormatters()
             .ConfigureApiBehaviorOptions(setupAction =>
            {
                setupAction.InvalidModelStateResponseFactory = context =>
                {
                    // create a problem details object
                    var problemDetailsFactory = context.HttpContext.RequestServices
                            .GetRequiredService<ProblemDetailsFactory>();
                    var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
                            context.HttpContext,
                            context.ModelState);

                    // add additional info not added by default
                    problemDetails.Detail = "See the errors field for details.";
                    problemDetails.Instance = context.HttpContext.Request.Path;

                    // find out which status code to use
                    var actionExecutingContext =
                              context as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

                    // if there are modelstate errors & all arguments were correctly
                    // found/parsed we're dealing with validation errors
                    if ((context.ModelState.ErrorCount > 0) &&
                            (actionExecutingContext?.ActionArguments.Count == context.ActionDescriptor.Parameters.Count))
                    {
                        problemDetails.Type = "https://courselibrary.com/modelvalidationproblem";
                        problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                        problemDetails.Title = "One or more validation errors occurred.";

                        return new UnprocessableEntityObjectResult(problemDetails)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    }

                    // if one of the arguments wasn't correctly found / couldn't be parsed
                    // we're dealing with null/unparseable input
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "One or more errors on input occurred.";
                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };
            });


            // 03/06/2022 10:34 pm - SSN - [20220306-2206] - [004] - M06-04 - Demo - HATEOAS and content negotiation
            services.Configure<MvcOptions>(config =>
            {
                // using using Microsoft.AspNetCore.Mvc.Formatters;
                // using System.Linq
                var newtonsoftJsonOutputFormatter = config.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

                if (newtonsoftJsonOutputFormatter != null)
                {
                    newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add(Helpers.Constants.MEDIA_TYPE_APPLICATION_VND_MARGIN_HATEOAS_JSON);
                }

            });




            // 03/05/2022 09:43 am - SSN - [20220305-0715] - [007] - M03-05 - Creating a property mapping service
            // services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddSingleton<IPropertyMappingService, PropertyMappingService>();




            // 03/07/2022 12:47 pm - SSN - [20220307-1105] - [006] - M06-06 - Demo - Tightening the contract between client and server with vendor-specific media types
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();


            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository>();

            // 03/04/2022 05:03 pm - SSN
            string connectionString = Environment.GetEnvironmentVariable("PS_344_ConnectionString");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("ps-344-webAPI-20220304-1704: Missing or empty environment variable [PS_344_ConnectionString]");
            }

            services.AddDbContext<CourseLibraryContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
                    });
                });

            }




            // 03/07/2022 12:59 pm - SSN - [20220307-1105] - [008] - M06-06 - Demo - Tightening the contract between client and server with vendor-specific media types
            Helpers.HttpHelper.Configure(app.ApplicationServices.GetService<IHttpContextAccessor>());


            // 03/08/2022 06:58 pm - SSN - [20220308-1856] - [002] - M07-06 - Demo - Adding a cache store with the ResponseCaching middleware
            // Must be added before routing and endpoints.
            app.UseResponseCaching();


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
