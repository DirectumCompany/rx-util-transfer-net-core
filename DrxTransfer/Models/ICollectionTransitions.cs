namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Коллекция переходов в правилах согласования.
  /// </summary>
  public class ICollectionTransitions
  {
    public int Id { get; set; }
    public int SourceStage { get; set; }
    public int TargetStage { get; set; }
    public bool? ConditionValue { get; set; }
  }
}
