using System.Collections.Generic;

namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Правило согласования.
  /// </summary>
  public class IApprovalRules : IApprovalRuleBases
  {
    public List<ICollectionConditions> Conditions { get; set; }
  }
}
