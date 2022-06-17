using System.Collections.Generic;

namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Роль согласования.
  /// </summary>
  public class IApprovalRoleBases
  {
    public string Name { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public string Type { get; set; }
    public int Id { get; set; }
    public override string ToString()
    {
      return Name;
    }
  }
}
