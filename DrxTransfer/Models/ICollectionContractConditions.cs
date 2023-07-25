namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Коллекция условий в правилах согласования.
  /// </summary>
  public class ICollectionContractConditions
  {
    public int Id { get; set; }
    public int Number { get; set; }
    public IContractConditions Condition { get; set; }
  }
}
