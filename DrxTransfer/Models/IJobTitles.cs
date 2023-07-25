namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Должность.
  /// </summary>
  public class IJobTitles
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
