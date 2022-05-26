namespace DrxTransfer.IntegrationServicesClient
{
  [EntityName("Коллекция условий в правилах согласования")]
  public class ICollectionConditions
  {
    public int Id { get; set; }
    public int Number { get; set; }
    public IConditions Condition { get; set; }
  }
}
