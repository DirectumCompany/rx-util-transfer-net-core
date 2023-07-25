using System.Collections.Generic;

namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Правило согласования.
  /// </summary>
  public class IApprovalRuleBases
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Note { get; set; }
    public string Status { get; set; }
    public int Priority { get; set; }
    public List<ICollectionDocumentKinds> DocumentKinds { get; set; }
    public List<ICollectionDepartments> Departments { get; set; }
    public List<ICollectionStages> Stages { get; set; }
    public List<ICollectionBusinessUnits> BusinessUnits { get; set; }
    public List<ICollectionDocumentGroups> DocumentGroups { get; set; }
    public string DocumentFlow { get; set; }
    public bool IsDefaultRule { get; set; }
    public List<ICollectionTransitions> Transitions { get; set; }
    public List<ICollectionConditionBases> Conditions { get; set; }
    public int VersionNumber { get; set; }
    public IApprovalRuleBases ParentRule { get; set; }
    public bool IsSmallApprovalAllowed { get; set; }
    public string ReworkPerformerType { get; set; }
    public IRecipients ReworkPerformer { get; set; }
    public int ReworkDeadline { get; set; }
    public IApprovalRoleBases ReworkApprovalRole { get; set; }
    public bool NeedRestrictInitiatorRights { get; set; }
    public override string ToString()
    {
      return Name;
    }
  }
}
