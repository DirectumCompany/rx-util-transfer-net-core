namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Коллекция валют.
  /// </summary>
  public class ICollectionCurrencies
  {
    public int Id { get; set; }
    public ICurrencies Currency { get; set; }
  }
}
