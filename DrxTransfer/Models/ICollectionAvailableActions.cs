namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Коллекция действий по отправке.
  /// </summary>
  public class ICollectionAvailableActions
  {
    public int Id { get; set; }
    public IDocumentSendAction Action { get; set; }
  }
}
