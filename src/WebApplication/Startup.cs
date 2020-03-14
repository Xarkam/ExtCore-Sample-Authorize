// Copyright © 2017 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using ExtCore.Data.EntityFramework;
using ExtCore.WebApplication.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WebApplication
{
    public class Startup
    {
        private string extensionsPath;

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
          this.Configuration = configuration;
          this.extensionsPath = webHostEnvironment.ContentRootPath + this.Configuration["Extensions:Path"];
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            
            services.Configure<CookiePolicyOptions>(options =>
           {
              // This lambda determines whether user consent for non-essential cookies is needed for a given request.
              options.CheckConsentNeeded = context => true;
              options.MinimumSameSitePolicy = SameSiteMode.None;
           });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(cookieOptions =>
            {
              cookieOptions.LoginPath = "/Account/LogIn";
            });

            services.AddMvc().AddRazorPagesOptions(options =>
            {
              options.Conventions.AuthorizeFolder("/");
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddExtCore(this.extensionsPath);
            services.Configure<StorageContextOptions>(options =>
            {
              options.ConnectionString = this.Configuration.GetConnectionString("Default");
            }
            );
        }

        public void Configure(IApplicationBuilder applicationBuilder, IWebHostEnvironment webHostEnvironment)
        {
            if (webHostEnvironment.IsDevelopment())
              applicationBuilder.UseDeveloperExceptionPage();

            applicationBuilder.UseHttpsRedirection();
            applicationBuilder.UseRouting();
            applicationBuilder.UseAuthentication();
            applicationBuilder.UseExtCore();
        }
    }
}