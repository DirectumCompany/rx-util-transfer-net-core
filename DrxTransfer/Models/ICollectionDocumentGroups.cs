namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Коллекция групп документов.
  /// </summary>
  public class ICollectionDocumentGroups
  {
    public int Id { get; set; }
    public IDocumentGroupBases DocumentGroup { get; set; }
  }
}
