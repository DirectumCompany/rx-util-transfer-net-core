using System.Collections.Generic;

namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Настройки регистрации.
  /// </summary>
  public class IRegistrationSettings
  {
    public string Name { get; set; }
    public string DocumentFlow { get; set; }
    public string SettingType { get; set; }
    public string Status { get; set; }
    public int Id { get; set; }
    public int Priority { get; set; }
    public IDocumentRegisters DocumentRegister { get; set; }
    public List<ICollectionDocumentKinds> DocumentKinds { get; set; }
    public List<ICollectionDepartments> Departments { get; set; }
    public List<ICollectionBusinessUnits> BusinessUnits { get; set; }
    public override string ToString()
    {
      return Name;
    }
  }
}
