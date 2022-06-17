namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Коллекция подразделений.
  /// </summary>
  public class ICollectionDepartments
  {
    public int Id { get; set; }
    public IDepartments Department { get; set; }
  }
}
