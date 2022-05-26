using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using DrxTransfer;
using DrxTransfer.IntegrationServicesClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DrxTransfer
{
  class RegistrationSettingSerializer
  {
    #region Singletone Implementation

    private static RegistrationSettingSerializer instance;
    public static RegistrationSettingSerializer Instance
    {
      get
      {
        if (instance == null)
          instance = new RegistrationSettingSerializer();
        return instance;
      }
    }

    #endregion

    public void ImportRegistrationSetting(IRegistrationSettings registrationSettings, NLog.Logger logger)
    {
      var registrationSettingsName = registrationSettings.Name;

      var activeRegistrationSettings = BusinessLogic.GetEntitiesWithFilter<IRegistrationSettings>(r => r.Name == registrationSettingsName && r.Status == "Active", logger);
      if (activeRegistrationSettings != null)
      {
        logger.Info(string.Format("Настройка регистрации {0} уже существует", registrationSettingsName));
        return;
      }

      var documentKinds = registrationSettings.DocumentKinds;
      var departments = registrationSettings.Departments;
      var businessUnits = registrationSettings.BusinessUnits;
      registrationSettings.DocumentKinds = null;
      registrationSettings.Departments = null;
      registrationSettings.BusinessUnits = null;

      var newRegistrationSetting = BusinessLogic.CreateEntity<IRegistrationSettings>(registrationSettings, logger);
      foreach (var documentKind in documentKinds)
      {
        var updatedRecipientsLinks = BusinessLogic.InstanceOData().For<IRegistrationSettings>().Key(newRegistrationSetting).NavigateTo(x => x.DocumentKinds).Set(documentKind).InsertEntryAsync().Result;
      }
      foreach (var department in departments)
      {
        var updatedRecipientsLinks = BusinessLogic.InstanceOData().For<IRegistrationSettings>().Key(newRegistrationSetting).NavigateTo(x => x.Departments).Set(department).InsertEntryAsync().Result;
      }
      foreach (var businessUnit in businessUnits)
      {
        var updatedRecipientsLinks = BusinessLogic.InstanceOData().For<IRegistrationSettings>().Key(newRegistrationSetting).NavigateTo(x => x.BusinessUnits).Set(businessUnit).InsertEntryAsync().Result;
      }
    }

    public void Import(string filePath, NLog.Logger logger)
    {
      try
      {
        string jsonText = string.Empty;
        using (StreamReader sr = new StreamReader(filePath, System.Text.Encoding.Default))
        {
          logger.Info("Чтение файла");
          jsonText = sr.ReadToEnd();
        }
        var jsonBody = JsonConvert.DeserializeObject<IEnumerable<IRegistrationSettings>>(jsonText);

        var index = 1;
        var jsonItemsCount = jsonBody.Count();
        foreach (var jsonItem in jsonBody)
        {
          try
          {
            logger.Info(string.Format("Запись {0} из {1}", index, jsonItemsCount));
            index++;
            ImportRegistrationSetting(jsonItem, logger);
          }
          catch (Exception ex)
          {
            logger.Error(ex);
            logger.Error("Запись не создана");
          }
        }
      }
      catch (Exception ex)
      {
        logger.Error(ex);
      }
    }

    public void Export(string filePath, NLog.Logger logger)
    {
      try
      {
        var registrationSettings = BusinessLogic.InstanceOData()
          .For<IRegistrationSettings>()
          .Filter(c => c.Status == "Active")
          .Expand(c => c.DocumentRegister)
          .Expand(c => c.DocumentKinds.Select(c => c.DocumentKind))
          .Expand(c => c.BusinessUnits.Select(c => c.BusinessUnit))
          .Expand(c => c.Departments.Select(c => c.Department))
          .FindEntriesAsync()
          .Result;
        var registrationSettingsCount = registrationSettings.Count();
        logger.Info(string.Format("Найдено {0} объектов", registrationSettingsCount));
        string JSONBody = JsonConvert.SerializeObject(registrationSettings);

        using (StreamWriter sw = new StreamWriter(filePath, false, System.Text.Encoding.Default))
        {
          logger.Info("Запись результата в файл");
          sw.WriteLine(JSONBody);
        }
      }
      catch (Exception ex)
      {
        logger.Error(ex);
      }
    }
  }
}
