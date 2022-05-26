namespace DrxTransfer.IntegrationServicesClient
{
  [EntityName("Коллекция адресатов")]
  public class ICollectionAddressees
  {
    public int Id { get; set; }
    public IEmployees Addressee { get; set; }
  }
}
