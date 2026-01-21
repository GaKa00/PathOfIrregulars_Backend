
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

            
            builder.Services.AddAuthorization();

        
            builder.Services.AddOpenApi();

            //Add card repository to Api services
            builder.Services.AddSingleton<CardRepository>(sp =>
            {
                var env = sp.GetRequiredService<IHostEnvironment>();
                return new CardRepository(env.ContentRootPath);
            });
            //Add DbConnection
            builder.Services.AddDbContext<POIdbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            //Swagger middleware - remember to add /swagger to the url
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            // Get all cards as per Dto definition
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

            // Get card by id - useable in deck building etc
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


            // Account endpoints

            //Get specific account, and load relevant info.
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

                var result = new AccountDto(
                     account.Id,
                     account.Username,
                     account.Elo,
                     decksDto
                );

                return Results.Ok(result);
            });

            // Mock account creation 
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


            // Get decks for specific account
            app.MapGet("/account{id}/decks", (int id, POIdbContext db) =>
            {
                var account = db.Accounts.FirstOrDefault(p => p.Id == id);
                if (account == null)
                    return Results.NotFound();
                var decks = account.Decks.ToList();
                return Results.Ok(decks);
            });


            app.Run();

        }
    }
}
