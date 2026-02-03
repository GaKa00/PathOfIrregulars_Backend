using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using PathOfIrregulars.API.Contracts;
using PathOfIrregulars.API.Contracts.GameRelated;
using PathOfIrregulars.API.Mappers;
using PathOfIrregulars.Application;
using PathOfIrregulars.Application.Mappers;
using PathOfIrregulars.Application.Services;

using PathOfIrregulars.Domain.Entities;
using PathOfIrregulars.Infrastructure.Database.Data;
using PathOfIrregulars.Infrastructure.Database.Models;
using PathOfIrregulars.Infrastructure.Persistence;
using System.Text.RegularExpressions;

namespace PathOfIrregulars.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
            builder.Services.AddAuthorization();

        
            builder.Services.AddOpenApi();

            //Add MatchStore to API
            builder.Services.AddSingleton<MatchStore>();

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
                var account =  await db.Accounts.Include(a => a.Decks).ThenInclude(d => d.Cards).FirstOrDefaultAsync(a => a.Id == id);
                if (account == null)
                    return Results.NotFound();

                return Results.Ok(account.Decks);
            });

            //create a deck for user
            app.MapPost("/accounts/{id}/decks", async
                (int id,
                 CreateDeckDto dto,
                 POIdbContext db,
                 CardRepository cards) =>
                {
                    //ADD VALIDATION , Decks should not have the same name, be 40 cards. if lower they are invalid. -TODO
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
                        Cards = dto.CardIds.GroupBy(id => id)
                .Select(c => new Infrastructure.Database.Models.Card
                        {
                            CardId = c.Key,
                            Amount = c.Count()
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


        

            app.MapPost("/matches", async (CreateMatchDto data, POIdbContext db, CardRepository cardRepo) =>
            {


                var account1 =  await db.Accounts.FindAsync(data.PlayerOneId);
                var account2 = await db.Accounts.FindAsync(data.PlayerTwoId);


                if ( account1 == null || account2 == null)
                {
                    return Results.NotFound("Accounts could not be retrieved.");
                }
                //add check for decks existing?

                var deck1 = await db.Decks.Include(d => d.Cards).FirstOrDefaultAsync(d => d.Id == data.PlayerOneDeckId && d.AccountId == account1.Id);

                var deck2 =  await db.Decks.Include(d => d.Cards).FirstOrDefaultAsync(d => d.Id == data.PlayerTwoDeckId && d.AccountId == account2.Id);

                if (deck1 == null || deck2 == null)
                {
                    return Results.NotFound("Decks could not be retrieved.");
                }


                var deck1Instances =
                    DeckMapper.ToCardInstances(deck1.Cards, cardRepo);

                var deck2Instances =
                    DeckMapper.ToCardInstances(deck2.Cards, cardRepo);

                var context = new Application.Match();
                var playerOne = PlayerFactory.Create( account1.Id, account1.Username, deck1Instances);
                var playerTwo = PlayerFactory.Create(account2.Id, account2.Username, deck2Instances);

                context.StartGame(playerOne, playerTwo);
                context.StartRound();

                var matchId = Guid.NewGuid();
                MatchStore.OngoingMatches[matchId] = context;

                return Results.Ok(
    MatchMapper.ToDto(matchId, context)
);



            });

            app.MapGet("/matches/{matchId}", (Guid matchId) =>
            {
                if (!MatchStore.OngoingMatches.TryGetValue(matchId, out var match))
                {
                    return Results.NotFound("Match not found.");
                }
                return Results.Ok(
                    MatchMapper.ToDto(matchId, match)
                );
            });

            app.MapPut("/matches/{matchId}/players/{playerId}/passTurn", ( Guid matchId, int playerId) =>
            {
                if (!MatchStore.OngoingMatches.TryGetValue(matchId, out var match))
                {
                    return Results.NotFound("Match not found.");
                }
              var ActivePlayer = match.ActivePlayer;
                if (ActivePlayer.Id != playerId)
                {
                    return Results.BadRequest("It's not the player's turn.");
                }
                match.ActivePlayer.HasPassed = true;
                return Results.Ok(
                    MatchMapper.ToDto(matchId, match)
                );

            });
            app.MapPut("/matches/{matchId}/players/{playerId}/startTurn", (Guid matchId, int playerId) =>
            {
                // get match, run validations, start turn fn for player
            });

            app.MapPut("/matches/{matchId}/players/{playerId}/endTurn", () =>
            {

            });

            app.MapPut("/matches/{matchId}/players/{playerId}/playCard", () =>
            {
            });

            app.MapPut("/matches/{matchId}/handleRound", () => {
            
            });





            //});

            ////player passing a turn
            //POST / matches /{ matchId}/ players /{ playerId}/ pass




            app.Run();

        }
    }
}
