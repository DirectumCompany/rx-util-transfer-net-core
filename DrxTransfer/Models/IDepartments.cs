namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Подразделение.
  /// </summary>
  public class IDepartments : IRecipients
  {
    public string Phone { get; set; }
    public string ShortName { get; set; }
    public string Note { get; set; }
    public string Code { get; set; }
    public IBusinessUnits BusinessUnit { get; set; }
    public IDepartments HeadOffice { get; set; }
    public IEmployees Manager { get; set; }
  }
}
