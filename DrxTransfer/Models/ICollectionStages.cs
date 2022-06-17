namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Коллекция этапов.
  /// </summary>
  public class ICollectionStages
  {
    public int Id { get; set; }
    public IApprovalStage Stage { get; set; }
    public int Number { get; set; }
    public string StageType { get; set; }
    public IApprovalStageBase StageBase { get; set; }
  }
}
