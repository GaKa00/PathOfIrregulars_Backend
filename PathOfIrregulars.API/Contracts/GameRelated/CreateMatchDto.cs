using PathOfIrregulars.Application;

namespace PathOfIrregulars.API.Contracts.GameRelated
{
 public class CreateMatchDto
{
    public int PlayerOneId { get; set; }
    public int PlayerTwoId { get; set; }

    public int PlayerOneDeckId { get; set; }
    public int PlayerTwoDeckId { get; set; }
}

}
