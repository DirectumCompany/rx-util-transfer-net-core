namespace DrxTransfer.IntegrationServicesClient
{
  [EntityName("Коллекция формат номера")]
  public class ICollectionNumberFormatItems
  {
    public int Id { get; set; }
    public int Number { get; set; }
    public string Separator { get; set; }
    public string Element { get; set; }
  }
}
