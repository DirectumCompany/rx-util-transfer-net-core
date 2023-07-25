namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Коллекция способов доставки.
  /// </summary>
  public class ICollectionDeliveryMethods
  {
    public int Id { get; set; }
    public IMailDeliveryMethods DeliveryMethod { get; set; }
  }
}
