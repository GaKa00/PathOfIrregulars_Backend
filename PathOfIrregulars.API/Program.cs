
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using PathOfIrregulars.API.Contracts;
using PathOfIrregulars.Application.Services;
using PathOfIrregulars.Domain.Entities;
using PathOfIrregulars.Infrastructure.Database.Data;
using PathOfIrregulars.Infrastructure.Database.Models;
using PathOfIrregulars.Infrastructure.Persistence;

namespace PathOfIrregulars.API
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
            app.MapPost("/accounts", async (
              CreateAccountDto dto,
              POIdbContext db) =>
            {
                var account = new Account
                {
                    Username = dto.Username,
                    PasswordHash = dto.Password, 
                    Elo = 1000,
                   
                };

                db.Accounts.Add(account);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/accounts/{account.Id}",
                    new AccountDto(
                        account.Id,
                        account.Username,
                        account.Elo,
                        new List<DeckDto>()
                    )
                );
            });



            // Get decks for specific account
            app.MapGet("/account/{id}/decks", (int id, POIdbContext db) =>
            {
                var account = db.Accounts.FirstOrDefault(p => p.Id == id);
                if (account == null)
                    return Results.NotFound();
                var decks = account.Decks.ToList();
                return Results.Ok(decks);
            });

            //create a deck for user
            app.MapPost("/accounts/{id}/decks",
                (int id,
                 CreateDeckDto dto,
                 POIdbContext db,
                 CardRepository cards) =>
                {
                    var account = db.Accounts
            .Include(a => a.Decks)
            .FirstOrDefault(a => a.Id == id);

                    if (account == null)
                        return Results.NotFound("Account not found");

                  
                    foreach (var cardId in dto.CardIds)
                    {
                        try
                        {
                            cards.GetCardById(cardId);

                        }
                        catch
                        {
                            return Results.BadRequest($"Invalid card ID: {cardId}");
                        }
                    }

                    var deck = new Deck
                    {
                        Name = dto.Name,
                        AccountId = account.Id,
                        Cards = dto.CardIds
                .Select(id => new Infrastructure.Database.Models.Card
                        {
                            CardId = id,
                            Amount = 1
                        })
                .ToList()
                    };

                    account.Decks.Add(deck);
                    db.SaveChanges();

                    return Results.Created(
            $"/accounts/{account.Id}/decks/{deck.Id}",
            deck.Id
        );
                });


            // create a new match, todo
            //app.MapPost("/Matches", (int p1, int p2, Deck deck1, Deck deck2, POIdbContext db) =>
            //{

            //});



            app.Run();

        }
    }
}
