namespace Bogevang.SequenceGenerator.Domain.Entities
{
  public class SequenceCounter
  {
    /// <summary>
    /// Counter name.
    /// </summary>
    public string Name { get; set; }


    /// <summary>
    /// Actual counter value.
    /// </summary>
    public int Counter { get; set; }
  }
}
