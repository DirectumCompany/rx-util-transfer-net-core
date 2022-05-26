namespace DrxTransfer.IntegrationServicesClient
{
  [EntityName("Коллекция субъектов")]
  public class ICollectionRecipients
  {
    public int Id { get; set; }
    public IRecipients Recipient { get; set; }
  }
}
