using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;

namespace App
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            string constring = @"User Id=V00811876;
                                Password=V00811876;
                                Data Source=128.172.188.107:1521/xe";

            using(OracleConnection con = new OracleConnection(constring))
            {
                using(OracleCommand command = con.CreateCommand())
                {
                    try {
                        con.Open();
                        command.BindByName = true;

                        command.CommandText = "select first_name from employees where department_id = :id";

                        OracleParameter id = new OracleParameter("id", 50);
                        command.Parameters.Add(id);

                        OracleDataReader reader = command.ExecuteReader();
                        while(reader.Read())
                        {
                            Console.WriteLine(reader.GetString(0));
                        }

                        reader.Dispose();
                    } catch (OracleException ex)
                    {
                        Console.WriteLine($"ORACLE EXCEPTION: {ex.Message}");
                    }
                }
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc();
        }
    }
}
