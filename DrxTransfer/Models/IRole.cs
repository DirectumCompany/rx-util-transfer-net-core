using System.Collections.Generic;

namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Роль.
  /// </summary>
  public class IRole
  {
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsSystem { get; set; }
    public bool IsSingleUser { get; set; }
    public string Status { get; set; }
    public int Id { get; set; }
    public List<IRecipientsLinks> RecipientLinks { get; set; }
    public override string ToString()
    {
      return Name;
    }
  }
}
