namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Этап согласования.
  /// </summary>
  public class IApprovalStageBase
  {
    public string Name { get; set; }
    public string Note { get; set; }
    public string Status { get; set; }
    public int Id { get; set; }
    public int? DeadlineInDays { get; set; }
    public int? DeadlineInHours { get; set; }

    public override string ToString()
    {
      return Name;
    }
  }
}
