
using System.Text;
using Chrestin_Project.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Chrestin_Project
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			/////// Handle Context Options 
			builder.Services.AddDbContext<Context>(options =>
			{
				options.UseNpgsql(builder.Configuration.GetConnectionString("myconnection"));
			});
			////// Handle Identity Options 

			builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<Context>();

			///// Handle CORS Security
			builder.Services.AddCors(CorsOptions =>
			{
				CorsOptions.AddPolicy("mycors",CorsPolicyBuilder =>
				{
					CorsPolicyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
				});
			});

			/////Handle Authorize Attribute
			builder.Services.AddAuthentication(options =>
			{
				////// Make Authentication based on Token that you created 
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //// schema of Authentication 
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; //// schema  indicate that if you are not regesterd to go to register page 
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; ///// another default schemas
			}).AddJwtBearer(options =>
			{
				options.SaveToken = true; //// token still in expirtaion date 
				options.RequireHttpsMetadata = false; /// indicate the protocol of request 
				options.TokenValidationParameters = new TokenValidationParameters() /// check on parameters in token to be from the same provider {NOT FAKE TOKEN}
				{
					ValidateIssuer = true,
					ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
					ValidateAudience = true,	
					ValidAudience = builder.Configuration["JWT:ValidAudience"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecutiryKey"])),

				};  

			});

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}
			app.UseCors("mycors");//// take the name of the policy that you already added in the services
			
			app.UseRouting();
			app.UseStaticFiles();///// midddle ware used to handle the static files in wwwroot like html pages, images

			app.UseAuthentication();
			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}