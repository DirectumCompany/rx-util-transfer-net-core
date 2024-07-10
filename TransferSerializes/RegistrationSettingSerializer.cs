using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using DrxTransfer;
using DrxTransfer.IntegrationServicesClient;
using Newtonsoft.Json.Linq;

namespace TransferSerializes
{
  [Export(typeof(SungeroSerializer))]
  class RegistrationSettingSerializer : SungeroSerializer
  {
    public RegistrationSettingSerializer() : base()
    {
      this.EntityName = "RegistrationSetting";
      this.EntityType = "IRegistrationSetting";
    }

    public override void Import(object jsonObject)
    {
      var registrationSetting = (jsonObject as JObject).ToObject<IRegistrationSettings>();

      var registrationSettingsName = registrationSetting.Name;

      var activeRegistrationSettings = IntegrationServiceClient.GetEntitiesWithFilter<IRegistrationSettings>(r => r.Name == registrationSettingsName && r.Status == "Active");
      if (activeRegistrationSettings != null)
      {
        Logger.Info(string.Format("Настройка регистрации {0} уже существует", registrationSettingsName));
        return;
      }

      // TODO : Увеличить критерии поиска журнала.
      var documentRegister = IntegrationServiceClient.GetEntitiesWithFilter<IDocumentRegisters>(r => r.Name == registrationSetting.DocumentRegister.Name && r.Status == "Active").FirstOrDefault();
      var documentKinds = registrationSetting.DocumentKinds;
      var departments = registrationSetting.Departments;
      var businessUnits = registrationSetting.BusinessUnits;
      registrationSetting.DocumentRegister = documentRegister;
      registrationSetting.DocumentKinds = null;
      registrationSetting.Departments = null;
      registrationSetting.BusinessUnits = null;

      var newRegistrationSetting = IntegrationServiceClient.CreateEntity<IRegistrationSettings>(registrationSetting);
      foreach (var documentKindItem in documentKinds.Select(k => k.DocumentKind))
      {
        var documentKind = IntegrationServiceClient.GetEntitiesWithFilter<IDocumentKinds>(r => r.Name == documentKindItem.Name && r.DocumentFlow == documentKindItem.DocumentFlow && r.Status == "Active").FirstOrDefault();
        IntegrationServiceClient.Instance.For<IRegistrationSettings>().Key(newRegistrationSetting)
          .NavigateTo(x => x.DocumentKinds).Set(new { DocumentKind = documentKind }).InsertEntryAsync().Wait();
      }
      foreach (var departmentItem in departments.Select(d => d.Department))
      {
        var department = IntegrationServiceClient.GetEntitiesWithFilter<IDepartments>(r => r.Name == departmentItem.Name && r.Status == "Active").FirstOrDefault();
        IntegrationServiceClient.Instance.For<IRegistrationSettings>().Key(newRegistrationSetting)
          .NavigateTo(x => x.Departments).Set(new { Department = department }).InsertEntryAsync().Wait();
      }
      foreach (var businessUnitItem in businessUnits.Select(u => u.BusinessUnit))
      {
        var businessUnit = IntegrationServiceClient.GetEntitiesWithFilter<IBusinessUnits>(r => r.Name == businessUnitItem.Name && r.Status == "Active").FirstOrDefault();
        IntegrationServiceClient.Instance.For<IRegistrationSettings>().Key(newRegistrationSetting)
          .NavigateTo(x => x.BusinessUnits).Set(new { BusinessUnit = businessUnit }).InsertEntryAsync().Wait();
      }

      // Обновляем сущность, т.к. коробочная логика затирает поле журнал регистрации, если он не регистрируемый.
      if (newRegistrationSetting.SettingType != "Registration")
      {
        newRegistrationSetting.DocumentRegister = documentRegister;
        newRegistrationSetting = IntegrationServiceClient.UpdateEntity<IRegistrationSettings>(newRegistrationSetting);
      }
    }

    protected override IEnumerable<dynamic> Export()
    {
      return IntegrationServiceClient.Instance.For<IRegistrationSettings>()
          .Filter(c => c.Status == "Active")
          .Expand(c => c.DocumentRegister)
          .Expand(c => c.DocumentKinds.Select(c => c.DocumentKind))
          .Expand(c => c.BusinessUnits.Select(c => c.BusinessUnit))
          .Expand(c => c.Departments.Select(c => c.Department))
          .FindEntriesAsync()
          .Result;
    }
  }
}
