namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Типы документов.
  /// </summary>
  public class IDocumentType
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string DocumentTypeGuid { get; set; }
    public string DocumentFlow { get; set; }
    public string Status { get; set; }
    public bool IsRegistrationAllowed { get; set; }

    public override string ToString()
    {
      return Name;
    }
  }
}
