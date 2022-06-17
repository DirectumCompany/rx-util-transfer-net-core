namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Коллекция условий в правилах согласования.
  /// </summary>
  public class ICollectionConditionBases
  {
    public int Id { get; set; }
    public int Number { get; set; }
    public IConditionBases Condition { get; set; }
  }
}
