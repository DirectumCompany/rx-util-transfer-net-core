namespace DrxTransfer.IntegrationServicesClient
{
  [EntityName("Коллекция наших оргинизаций")]
  public class ICollectionBusinessUnits
  {
    public int Id { get; set; }
    public IBusinessUnits BusinessUnit { get; set; }
  }
}
