namespace DrxTransfer.IntegrationServicesClient
{
  [EntityName("Коллекция ролей согласования")]
  public class ICollectionApprovalRoles
  {
    public int Id { get; set; }
    public IApprovalRoles ApprovalRole { get; set; }
  }
}
