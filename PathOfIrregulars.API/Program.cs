
using PathOfIrregulars.Application.Contracts;
using PathOfIrregulars.Application.Services.PathOfIrregulars.Application.Services;
using PathOfIrregulars.Infrastructure.Database.Data;
using PathOfIrregulars.Infrastructure.Database.Repositories;
using PathOfIrregulars.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace PathOfIrregulars.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();


            builder.Services.AddDbContext<GameDbContext>(options =>
          options.UseSqlite(
              builder.Configuration.GetConnectionString("Default")
          )
      );

            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<IDeckRepository, DeckRepository>();
            builder.Services.AddScoped<ICardRepository, CardRepository>();

            builder.Services.AddScoped<PlayerFactory>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

           

            app.Run();
        }
    }
}
