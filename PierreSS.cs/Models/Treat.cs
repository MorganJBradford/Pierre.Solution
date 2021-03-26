using System.Collections.Generic;

namespace PierreSS.Models
{
  public class Treat
  {
    public Treat()
    {
      JoinEntities = new HashSet<FlavorTreat>();
    }
    public int TreatId { get; set; }
    public string Name { get; set; }
    public virtual ICollection<FlavorTreat> JoinEntities { get; }
  }
}