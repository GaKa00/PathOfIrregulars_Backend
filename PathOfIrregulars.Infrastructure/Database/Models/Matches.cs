using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfIrregulars.Infrastructure.Database.Models
{
    public class Matches
    {
        public int Id { get; set; }
        public Guid PlayerOneId { get; set; }
        public Guid PlayerTwoId { get; set; }
        public Guid WinnerId { get; set; }
        public DateTime MatchDate { get; set; }

    }
}
