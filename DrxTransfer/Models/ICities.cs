namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Город.
  /// </summary>
  public class ICities
  {
    public string Name { get; set; }
    public string Status { get; set; }
    public int Id { get; set; }

    public override string ToString()
    {
      return Name;
    }
  }
}
