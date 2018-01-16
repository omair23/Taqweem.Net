﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Taqweem.Data;
using Taqweem.Models;
using Taqweem.Services;
using Taqweem.Classes;
using Microsoft.Extensions.DependencyInjection.Extensions;
using AutoMapper;
using Taqweem.ViewModels.ManageViewModels;

namespace Taqweem
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer("Data Source=tcp:taqweem.database.windows.net,1433;Initial Catalog=taqweem;User Id=omair@taqweem.database.windows.net;Password=Taqweem@786;"));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddScoped<RazorViewToStringRenderer>();

            services.AddScoped<RazorHelperService>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MasjidEditViewModel, Masjid>().ForMember(x => x.UID, opt => opt.Ignore());

                cfg.CreateMap<SalaahTimeViewModel, SalaahTime>().ForMember(x => x.UID, opt => opt.Ignore());
            });

            var mapper = config.CreateMapper();

            services.AddSingleton(mapper);

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
