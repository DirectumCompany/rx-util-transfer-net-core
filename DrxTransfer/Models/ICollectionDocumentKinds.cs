namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Коллекция видов документов.
  /// </summary>
  public class ICollectionDocumentKinds
  {
    public int Id { get; set; }
    public IDocumentKinds DocumentKind { get; set; }
  }
}
