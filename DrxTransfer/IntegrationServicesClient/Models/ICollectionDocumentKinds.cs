namespace DrxTransfer.IntegrationServicesClient
{
  [EntityName("Коллекция видов документов")]
  public class ICollectionDocumentKinds
  {
    public int Id { get; set; }
    public IDocumentKinds DocumentKind { get; set; }
  }
}
