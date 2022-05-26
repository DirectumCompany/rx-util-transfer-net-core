﻿using System.Collections.Generic;

namespace DrxTransfer.IntegrationServicesClient
{
  [EntityName("Условие")]
  public class IConditions
  {
    public string Name { get; set; }
    public string Status { get; set; }
    public int Id { get; set; }
    public string ConditionType { get; set; }
    public int Amount { get; set; }
    public string AmountOperator { get; set; }
    public List<ICollectionCurrencies> Currencies { get; set; }
    public List<ICollectionDocumentKinds> DocumentKinds { get; set; }
    public string Note { get; set; }
    public List<ICollectionDocumentKinds> ConditionDocumentKinds { get; set; }
    public IApprovalRoles ApprovalRole { get; set; }
    public IApprovalRoles ApprovalRoleForComparison { get; set; }
    public IRecipients RecipientForComparison { get; set; }
    public List<ICollectionDeliveryMethods> DeliveryMethods { get; set; }
    public IDocumentKinds AddendaDocumentKind { get; set; }
    public List<ICollectionAddressees> Addressees { get; set; }

    public override string ToString()
    {
      return Name;
    }
  }
}
