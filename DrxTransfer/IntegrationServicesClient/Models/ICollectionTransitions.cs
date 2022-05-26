namespace DrxTransfer.IntegrationServicesClient
{
  [EntityName("Коллекция переходов в правилах согласования")]
  public class ICollectionTransitions
  {
    public int Id { get; set; }
    public int SourceStage { get; set; }
    public int TargetStage { get; set; }
    public bool ConditionValue { get; set; }
  }
}
