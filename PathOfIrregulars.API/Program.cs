using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using PathOfIrregulars.API.Contracts;
using PathOfIrregulars.Application;
using PathOfIrregulars.Application.Services;
using PathOfIrregulars.Application.Services.PathOfIrregulars.Application.Services;
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

                    var card =  cards.GetCardById(id);

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
            app.MapGet("/accounts/{id}", async (int id, POIdbContext db) =>
            {
                
                var account =  await db.Accounts
                                .Include(a => a.Decks)
                                .FirstOrDefaultAsync(p => p.Id == id);

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
                if (dto == null)
                    return Results.BadRequest("Request body is required.");

                if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                    return Results.BadRequest("Username and Password are required.");

                var usernameExists = await db.Accounts.AnyAsync(a => a.Username == dto.Username);
                if (usernameExists)
                    return Results.Conflict($"Username '{dto.Username}' already exists.");

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
            app.MapGet("/account/{id}/decks", async (int id, POIdbContext db) =>
            {
                var account =  await db.Accounts.FirstOrDefaultAsync(p => p.Id == id);
                if (account == null)
                    return Results.NotFound();
                var decks = account.Decks.ToList();
                return Results.Ok(decks);
            });

            //create a deck for user
            app.MapPost("/accounts/{id}/decks", async
                (int id,
                 CreateDeckDto dto,
                 POIdbContext db,
                 CardRepository cards) =>
                {
                    var account = await  db.Accounts
            .Include(a => a.Decks)
            .FirstOrDefaultAsync(a => a.Id == id);

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
                    await db.SaveChangesAsync();

                    return Results.Created(
            $"/accounts/{account.Id}/decks/{deck.Id}",
            deck.Id
        );
                });


            //// create a new match, todo
            //app.MapPost("/Matches", (int p1, int p2, Deck deck1, Deck deck2, POIdbContext db) =>
            //{
            //    var context = new GameContext();
            //    var transformer = new PlayerFactory();

            //    var Account1 = db.Accounts.FirstOrDefault(a => a.Id == p1);
            //    var Account2 = db.Accounts.FirstOrDefault(a => a.Id == p2);

            //    var player1 = null;
            //    var player2 = null;


            //    var player1Deck = db.Decks.FirstOrDefault(d => d.AccountId == p1);
            //    if (player1Deck != null)
            //    {
            //      player1 = transformer.Create(Account1.Username, deck1.Cards);
            //    }
            //    else
            //    {
            //        return Results.NotFound("Player 1 Deck not found");
            //    }

            //    var player2Deck = db.Decks.FirstOrDefault(d => d.AccountId == p2);
            //    if (player2Deck != null)
            //    {
            //        player2 = transformer.Create(Account2.Username, deck2.Cards);
            //    }
            //    else
            //    {
            //        return Results.NotFound("Player 2 Deck not found");
            //    }

            //    context.StartGame(player1, player2);

                


            //});



            app.Run();

        }
    }
}
