using System.Collections.Generic;

namespace DrxTransfer.IntegrationServicesClient
{
  [EntityName("Роль согласования")]
  public class IApprovalRoles
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
