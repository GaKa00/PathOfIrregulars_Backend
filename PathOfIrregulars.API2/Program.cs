
using Microsoft.EntityFrameworkCore;
using PathOfIrregulars.API2.Contracts;
using PathOfIrregulars.Infrastructure.Database.Data;
using PathOfIrregulars.Infrastructure.Persistence;

namespace PathOfIrregulars.API2
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

            builder.Services.AddSingleton<CardRepository>(sp =>
            {
                var env = sp.GetRequiredService<IHostEnvironment>();
                return new CardRepository(env.ContentRootPath);
            });
            builder.Services.AddDbContext<POIdbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapGet("/cards", (CardRepository cards) =>
            {
                var result = cards.GetAll()
                    .Select(card => new CardDto(
                        card.Id,
                        card.Name,
                        card.Type,
                        card.Power
                    ))
                    .ToList();

                return Results.Ok(result);
            });

            app.MapGet("/cards/{id}", (string id, CardRepository cards) =>
            {
                try
                {

                    var card = cards.GetCardById(id);

                    var result = new CardDto(
                        card.Id,
                        card.Name,
                        card.Type,
                        card.Power
                    );
                    return Results.Ok(result);
                }
                catch (KeyNotFoundException)
                {
                    return Results.NotFound();
                }
            });

            app.MapGet("/accounts/{id}", (int id, POIdbContext db) =>
            {
                
                var account = db.Accounts
                                .Include(a => a.Decks)
                                .FirstOrDefault(p => p.Id == id);

                if (account == null)
                    return Results.NotFound();

                var decksDto = account.Decks
                    .Select(d => new DeckDto
                    {
                        Id = d.Id,
                        Name = d.Name
                    })
                    .ToList();

                var dto = new AccountDto(
                     account.Id,
                     account.Username,
                     account.Elo,
                     decksDto
                );

                return Results.Ok(dto);
            });

            app.MapPost("/accounts", async (string username, string password, POIdbContext db) =>
            {
                var account = new Infrastructure.Database.Models.Account
                {
                    Username = username,
                    PasswordHash = password, 
                    Elo = 1000
                };
                db.Accounts.Add(account);
                await db.SaveChangesAsync();
                var dto = new AccountDto(
                    account.Id,
                    account.Username,
                    account.Elo,
                    new List<DeckDto>()
                );
                return Results.Created($"/accounts/{account.Id}", dto);
            });


            app.Run();

        }
    }
}
