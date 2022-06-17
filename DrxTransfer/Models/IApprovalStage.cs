using System.Collections.Generic;

namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Этап согласования.
  /// </summary>
  public class IApprovalStage
  {
    public string Name { get; set; }
    public string Note { get; set; }
    public string Status { get; set; }
    public int? Id { get; set; }
    public string StageType { get; set; }
    public string Sequence { get; set; }
    public string ReworkType { get; set; }
    public IRecipients Assignee { get; set; }
    public bool? NeedStrongSign { get; set; }
    public int? StartDelayDays { get; set; }
    public string Subject { get; set; }
    public bool? AllowSendToRework { get; set; }
    public bool? IsConfirmSigning { get; set; }
    public bool? IsResultSubmission { get; set; }
    public int? DeadlineInDays { get; set; }
    public int? DeadlineInHours { get; set; }
    public IApprovalRoleBases ApprovalRole { get; set; }
    public List<ICollectionApprovalRoles> ApprovalRoles { get; set; }
    public string AssigneeType { get; set; }
    public List<ICollectionRecipients> Recipients { get; set; }
    public bool? AllowAdditionalApprovers { get; set; }
    public bool? AllowChangeReworkPerformer { get; set; }
    public string ReworkPerformerType { get; set; }
    public IRecipients ReworkPerformer { get; set; }
    public IApprovalRoleBases ReworkApprovalRole { get; set; }
    public string RightType { get; set; }
    public bool? NeedRestrictPerformerRights { get; set; }

    public override string ToString()
    {
      return Name;
    }
  }
}
