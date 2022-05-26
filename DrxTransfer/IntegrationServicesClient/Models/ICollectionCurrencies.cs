namespace DrxTransfer.IntegrationServicesClient
{
  [EntityName("Коллекция валют")]
  public class ICollectionCurrencies
  {
    public int Id { get; set; }
    public ICurrencies Currency { get; set; }
  }
}
