using System.Collections.Generic;

namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Виды документов.
  /// </summary>
  public class IDocumentKinds
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Note { get; set; }
    public int? DeadlineInDays { get; set; }
    public string ShortName { get; set; }
    public int? DeadlineInHours { get; set; }
    public bool GenerateDocumentName { get; set; }
    public bool AutoNumbering { get; set; }
    public bool ProjectsAccounting { get; set; }
    public bool GrantRightsToProject { get; set; }
    public bool IsDefault { get; set; }
    public string Code { get; set; }
    public string DocumentFlow { get; set; }
    public string NumberingType { get; set; }
    public string Status { get; set; }
    public IDocumentType DocumentType { get; set; }
    public List<ICollectionAvailableActions> AvailableActions { get; set; }
    public override string ToString()
    {
      return Name;
    }
  }
}
