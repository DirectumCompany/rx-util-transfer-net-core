namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Контрагент.
  /// </summary>
  public class ICounterparties
  {
    public string Name { get; set; }
    public string TIN { get; set; }
    public string LegalAddress { get; set; }
    public string PostalAddress { get; set; }
    public string Phones { get; set; }
    public string Email { get; set; }
    public string Homepage { get; set; }
    public string Note { get; set; }
    public bool Nonresident { get; set; }
    public string PSRN { get; set; }
    public string NCEO { get; set; }
    public string NCEA { get; set; }
    public string Account { get; set; }
    public string CanExchange { get; set; }
    public string Code { get; set; }
    public string Status { get; set; }
    public int Id { get; set; }
    public ICities City { get; set; }
    public IRegions Region { get; set; }
    public IBanks Bank { get; set; }

    public override string ToString()
    {
      return Name;
    }
  }
}
