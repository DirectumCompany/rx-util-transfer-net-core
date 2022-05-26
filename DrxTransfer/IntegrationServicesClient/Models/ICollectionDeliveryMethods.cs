namespace DrxTransfer.IntegrationServicesClient
{
  [EntityName("Коллекция способов доставки")]
  public class ICollectionDeliveryMethods
  {
    public int Id { get; set; }
    public IMailDeliveryMethods DeliveryMethod { get; set; }
  }
}
