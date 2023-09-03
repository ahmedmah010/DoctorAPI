
using Data;
using Data.Repos;
using Data.Repos.IRepos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Model;
using System.Text;
namespace DoctorAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
          
            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddDbContext<AppDbContext>(op => 
            {
                op.UseLazyLoadingProxies().UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                    );
            });
            
            builder.Services.AddIdentity<AppUser,IdentityRole>().AddEntityFrameworkStores<AppDbContext>();

            builder.Services.AddAuthentication(
                op => {
                    op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    op.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(op => {
                    op.SaveToken = true;
                    op.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = builder.Configuration["JWT:issuer"],
                        ValidAudience = builder.Configuration["JWT:aud"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:key"]))

                    };
                });

            builder.Services.AddTransient(typeof(IRepo<>), typeof(Repo<>));

            builder.Services.AddCors(op => op.AddPolicy("MyPolicy", op => op.AllowAnyOrigin().AllowAnyMethod()));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseCors("MyPolicy");

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}