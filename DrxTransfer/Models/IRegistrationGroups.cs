using System.Collections.Generic;

namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Группа регистрации.
  /// </summary>
  public class IRegistrationGroups : IRecipients
  {
    public string Index { get; set; }
    public bool CanRegisterIncoming { get; set; }
    public bool CanRegisterOutgoing { get; set; }
    public bool CanRegisterInternal { get; set; }
    public bool CanRegisterContractual { get; set; }
    public IEmployees ResponsibleEmployee { get; set; }
    public List<IRecipientsLinks> RecipientLinks { get; set; }
    public List<ICollectionDepartments> Departments { get; set; }
  }
}
