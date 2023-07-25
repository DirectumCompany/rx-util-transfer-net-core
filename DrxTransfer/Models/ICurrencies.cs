namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Валюта.
  /// </summary>
  public class ICurrencies
  {
    public string Name { get; set; }
    public string AlphaCode { get; set; }
    public string Status { get; set; }
    public string ShortName { get; set; }
    public string FractionName { get; set; }
    public bool IsDefault { get; set; }
    public string NumericCode { get; set; }
    public int Id { get; set; }

    public override string ToString()
    {
      return Name;
    }
  }
}
