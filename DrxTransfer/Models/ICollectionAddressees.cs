namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Коллекция адресатов.
  /// </summary>
  public class ICollectionAddressees
  {
    public int Id { get; set; }
    public IEmployees Addressee { get; set; }
  }
}
