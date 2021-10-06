using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace HackathonE1.Website.Server
{
	public class Startup
	{
		public Startup( IConfiguration configuration )
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices( IServiceCollection services )
		{

			_ = services.AddControllersWithViews();
			_ = services.AddRazorPages();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
		{
			if ( env.IsDevelopment() )
			{
				_ = app.UseDeveloperExceptionPage();
				app.UseWebAssemblyDebugging();
			}
			else
			{
				_ = app.UseExceptionHandler( "/Error" );
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				_ = app.UseHsts();
			}

			_ = app.UseHttpsRedirection();
			_ = app.UseBlazorFrameworkFiles();
			_ = app.UseStaticFiles();

			_ = app.UseRouting();

			_ = app.UseEndpoints( endpoints =>
			   {
				   _ = endpoints.MapRazorPages();
				   _ = endpoints.MapControllers();
				   _ = endpoints.MapFallbackToFile( "index.html" );
			   } );
		}
	}
}
