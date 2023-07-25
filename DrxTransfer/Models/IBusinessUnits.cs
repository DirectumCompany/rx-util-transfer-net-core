namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Наша организация.
  /// </summary>
  public class IBusinessUnits : IRecipients
  {
    public string TIN { get; set; }
    public string TRRC { get; set; }
    public string Phones { get; set; }
    public string LegalName { get; set; }
    public string LegalAddress { get; set; }
    public string PostalAddress { get; set; }
    public string Note { get; set; }
    public string Email { get; set; }
    public string Homepage { get; set; }
    public string PSRN { get; set; }
    public string NCEO { get; set; }
    public string NCEA { get; set; }
    public string Account { get; set; }
    public string Code { get; set; }
    public IBusinessUnits HeadCompany { get; set; }
    public IEmployees CEO { get; set; }
    public ICities City { get; set; }
    public IRegions Region { get; set; }
    public IBanks Bank { get; set; }
  }
}
