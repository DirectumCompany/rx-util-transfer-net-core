namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Коллекция субъектов.
  /// </summary>
  public class ICollectionRecipients
  {
    public int Id { get; set; }
    public IRecipients Recipient { get; set; }
  }
}
