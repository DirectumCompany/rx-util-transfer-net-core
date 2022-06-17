namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Коллекция ролей согласования.
  /// </summary>
  public class ICollectionApprovalRoles
  {
    public int Id { get; set; }
    public IApprovalRoleBases ApprovalRole { get; set; }
  }
}
