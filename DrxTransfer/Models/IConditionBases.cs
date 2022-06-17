using System.Collections.Generic;

namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Условие.
  /// </summary>
  public class IConditionBases
  {
    public string Name { get; set; }
    public string Status { get; set; }
    public int Id { get; set; }
    public string ConditionType { get; set; }
    public int? Amount { get; set; }
    public string AmountOperator { get; set; }
    public List<ICollectionCurrencies> Currencies { get; set; }
    public List<ICollectionDocumentKinds> DocumentKinds { get; set; }
    public string Note { get; set; }
    public List<ICollectionDocumentKinds> ConditionDocumentKinds { get; set; }
    public IApprovalRoleBases ApprovalRole { get; set; }
    public IApprovalRoleBases ApprovalRoleForComparison { get; set; }
    public IRecipients RecipientForComparison { get; set; }
    public List<ICollectionDeliveryMethods> DeliveryMethods { get; set; }
    public IDocumentKinds AddendaDocumentKind { get; set; }


    public override string ToString()
    {
      return Name;
    }
  }
}
