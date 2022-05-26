namespace DrxTransfer.IntegrationServicesClient
{
  [EntityName("Коллекция действий по отправке")]
  public class ICollectionAvailableActions
  {
    public int Id { get; set; }
    public IDocumentSendAction Action { get; set; }
  }
}
