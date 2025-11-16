using PathOfIrregulars.Application;
using PathOfIrregulars.Application.Services;
using PathOfIrregulars.Domain.Entities;
using PathOfIrregulars.Domain.ValueObjects;

// Setup
var context = new GameContext();
var registry = new EffectRegistry();
var cardService = new CardService(registry);

var p1 = new Player { Name = "Bam" };
var p2 = new Player { Name = "Khun" };
context.StartGame(p1, p2);

// Card to test
var card = new Card
{
    Name = "Test Buff",
    Power = new PowerValue(5),
    CardEffects = new()
    {
        new CardEffect { EffectId = "BuffSelf" }
    }
};

// Play the card
var result = cardService.PlayCard(card, context);

Console.WriteLine(result.Message);
Console.WriteLine($"After play: {card.Power.Value}");
