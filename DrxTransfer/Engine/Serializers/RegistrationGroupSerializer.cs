using DrxTransfer;
using DrxTransfer.IntegrationServicesClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrxTransfer
{
  class RegistrationGroupSerializer
  {
    #region Singletone Implementation

    private static RegistrationGroupSerializer instance;
    public static RegistrationGroupSerializer Instance
    {
      get
      {
        if (instance == null)
          instance = new RegistrationGroupSerializer();
        return instance;
      }
    }

    #endregion

    public void ImportRegistrationGroups(IRegistrationGroups registrationGroups, NLog.Logger logger)
    {
      var registrationGroupsName = registrationGroups.Name;

      var activeRegistrationGroups = BusinessLogic.GetEntitiesWithFilter<IRegistrationGroups>(r => r.Name == registrationGroupsName && r.Status == "Active", logger);
      if (activeRegistrationGroups != null)
      {
        logger.Info(string.Format("Группа регистрации {0} уже существует", registrationGroupsName));
        return;
      }

      var recipients = registrationGroups.RecipientLinks;
      var responsible = recipients.Where(r => r.Member.Name == registrationGroups.ResponsibleEmployee.Name).FirstOrDefault();
      recipients.Remove(responsible);
      registrationGroups.RecipientLinks = null;
      var departments = registrationGroups.Departments;
      registrationGroups.Departments = null;
      var newRegistrationGroups = BusinessLogic.CreateEntity<IRegistrationGroups>(registrationGroups, logger);
      foreach (var recipient in recipients)
      {
        var updatedRecipientsLinks = BusinessLogic.InstanceOData().For<IRegistrationGroups>().Key(newRegistrationGroups).NavigateTo(x => x.RecipientLinks).Set(recipient).InsertEntryAsync().Result;
      }
      foreach (var department in departments)
      {
        var updatedRecipientsLinks = BusinessLogic.InstanceOData().For<IRegistrationGroups>().Key(newRegistrationGroups).NavigateTo(x => x.Departments).Set(department).InsertEntryAsync().Result;
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
        var jsonBody = JsonConvert.DeserializeObject<IEnumerable<IRegistrationGroups>>(jsonText);

        var index = 1;
        var jsonItemsCount = jsonBody.Count();
        foreach (var jsonItem in jsonBody)
        {
          try
          {
            logger.Info(string.Format("Запись {0} из {1}", index, jsonItemsCount));
            index++;
            ImportRegistrationGroups(jsonItem, logger);
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
        var registrationGroups = BusinessLogic.InstanceOData()
          .For<IRegistrationGroups>()
          .Filter(c => c.Status == "Active")
          .Expand(c => c.ResponsibleEmployee)
          .Expand(c => c.RecipientLinks.Select(c => c.Member))
          .Expand(c => c.Departments.Select(c => c.Department))
          .FindEntriesAsync()
          .Result;
        var registrationGroupsCount = registrationGroups.Count();
        logger.Info(string.Format("Найдено {0} объектов", registrationGroupsCount));
        string JSONBody = JsonConvert.SerializeObject(registrationGroups);

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
