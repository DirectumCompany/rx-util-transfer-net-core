namespace DrxTransfer.IntegrationServicesClient
{
  [EntityName("Коллекция групп документов")]
  public class ICollectionDocumentGroups
  {
    public int Id { get; set; }
    public IDocumentGroups DocumentGroup { get; set; }
  }
}
