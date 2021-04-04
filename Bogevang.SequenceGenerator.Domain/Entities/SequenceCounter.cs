namespace Bogevang.SequenceGenerator.Domain.Entities
{
  public class SequenceCounter
  {
    /// <summary>
    /// Counter name - and database primary key.
    /// </summary>
    public string Name { get; set; }


    /// <summary>
    /// Actual counter value.
    /// </summary>
    public int Counter { get; set; }
  }
}
