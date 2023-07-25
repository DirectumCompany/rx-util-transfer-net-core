namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Сотрудник.
  /// </summary>
  public class IEmployees : IRecipients
  {
    public string Phone { get; set; }
    public string Note { get; set; }
    public string Email { get; set; }
    public bool? NeedNotifyExpiredAssignments { get; set; }
    public bool? NeedNotifyNewAssignments { get; set; }
    public string PersonnelNumber { get; set; }
    public IPersons Person { get; set; }
    public IDepartments Department { get; set; }
    public IJobTitles JobTitle { get; set; }
  }
}
