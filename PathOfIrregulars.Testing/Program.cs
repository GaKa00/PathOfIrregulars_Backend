
using PathOfIrregulars.Application;
using PathOfIrregulars.Application.Services;
using PathOfIrregulars.Domain.Entities;
using PathOfIrregulars.Infrastructure.Database.Models;
using PathOfIrregulars.Infrastructure.Persistence;

Console.WriteLine(File.ReadAllText("cards.json"));
var repo = new CardRepository("cards.json");

// Fake user with 1 deck
//var user = new UserProfile
//{

//    Username = "BamMain",
//    SavedDecks = new Dictionary<string, List<string>>
//    {
//        { "Starter", new List<string> { "bam_irregular",
//    "khun_strategist",
//    "rak_spear_bearer",
//    "anak_jahad",
//    "anak_jahad",
//    "endorse_kick",
//    "shinsu_barrier",
//    "lighthouse_scan",
//    "black_march",
//    "green_april",
//    "quant_ambush"  } }
//    }
//};

// Convert deck names → actual cards
//List<Card> deck = user.SavedDecks["Starter"]
//    .Select(id => repo.GetCardById(id))
//    .ToList();
//List<Card> deck2 = user.SavedDecks["Starter"]
//    .Select(id => repo.GetCardById(id))
//    .ToList();

//Console.WriteLine("deck claimed!");
//Thread.Sleep(500);
//// Build player
//var p1 = OldPlayerFactory.Create(user.Username, deck);
//var p2 = OldPlayerFactory.Create("KhunAI", deck2);
//Console.WriteLine("players created!");
//Thread.Sleep(500);

//// Create game context
//var context = new GameContext();
//context.StartGame(p1, p2);
//Console.WriteLine("game started!");












