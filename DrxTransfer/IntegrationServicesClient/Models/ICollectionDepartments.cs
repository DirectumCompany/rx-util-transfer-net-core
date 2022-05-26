namespace DrxTransfer.IntegrationServicesClient
{
  [EntityName("Коллекция подразделений")]
  public class ICollectionDepartments
  {
    public int Id { get; set; }
    public IDepartments Department { get; set; }
  }
}
