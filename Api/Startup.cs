using System;
using System.Text;
using System.Threading.Tasks;
using Api.Config;
using Api.Services;
using Application.Features;
using Application.Interfaces;
using Application.Service;
using Data;
using Data.Models;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Config class
            JwtConfig jwtConfig = new();
            CookieConfig cookieConfig = new();
            Configuration.Bind(nameof(JwtConfig), jwtConfig);
            Configuration.Bind(nameof(CookieConfig), cookieConfig);
            services.AddSingleton(jwtConfig);
            services.AddSingleton(cookieConfig);

            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(
                    Configuration.GetConnectionString("Postgres"),
                    x => x.MigrationsAssembly(nameof(Data)));
            });
            services.AddDatabaseDeveloperPageExceptionFilter();
            
            // Automapper
            services.AddAutoMapper(typeof(PingQuery).Assembly);
            
            // User Manager Services
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.SignIn.RequireConfirmedEmail = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Mediatr
            services.AddMediatR(typeof(PingQuery).Assembly);
            
            // Auth and jwt
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    var key = Encoding.ASCII.GetBytes(jwtConfig.Key);

                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false, // should be true in production
                        ValidateAudience = false, // should be true in production
                        //ValidateActor = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        //ValidIssuer = jwtConfig.Issuer,
                        //ValidAudience = jwtConfig.Audience,
                        ClockSkew = TimeSpan.Zero,
                        RequireExpirationTime = false,
                    };

                    options.Events = new JwtBearerEvents()
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Cookies[cookieConfig.JwtKey];
                            return Task.CompletedTask;
                        }
                    };
                });
            
            // App services
            services.AddSingleton<IDateTime, DateTimeService>();
            
            // Api Services
            //services.AddHostedService<SeedIdentityData>();

            // Controllers and fluentValidation
            services.AddControllers().AddFluentValidation(options =>
            {
                options.DisableDataAnnotationsValidation = false;
                options.RegisterValidatorsFromAssemblyContaining<PingQuery>();
            });
            
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Api", Version = "v1"}); });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));
            }

            app.UseHttpsRedirection();
            
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}