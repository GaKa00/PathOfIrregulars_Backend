using PathOfIrregulars.Domain.Enums;

namespace PathOfIrregulars.API.Contracts
{
public class CardDto
    {
public CardDto() { }

    public CardDto(string id, string name, string description, CardType type, int power)
    {
        Id = id ?? string.Empty;
        Name = name ?? string.Empty;
        Description = description;
        Type = type;
        Power = power;
    }

    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public CardType Type { get; set; }
    public int Power { get; set; }

        }
    
}
