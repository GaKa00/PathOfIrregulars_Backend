using PathOfIrregulars.Domain.Enums;

namespace PathOfIrregulars.API2.Contracts
{
    public class CardDto(string id, string name, CardType type, int power)
    {
     
        public string Id { get; set; } = id;
        public string Name { get; set; } = name;
        public  CardType Type { get; set; }
        public int Power { get; set; } = power;
    }
}
