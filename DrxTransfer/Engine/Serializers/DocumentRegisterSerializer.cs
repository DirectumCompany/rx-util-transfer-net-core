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
  class DocumentRegisterSerializer
  {
    #region Singletone Implementation

    private static DocumentRegisterSerializer instance;
    public static DocumentRegisterSerializer Instance
    {
      get
      {
        if (instance == null)
          instance = new DocumentRegisterSerializer();
        return instance;
      }
    }

    #endregion

    public void ImportDocumentRegisters(IDocumentRegisters documentRegisters, NLog.Logger logger)
    {
      var documentRegistersName = documentRegisters.Name;
      var activeDocumentRegisters = BusinessLogic.GetEntitiesWithFilter<IDocumentRegisters>(r => r.Name == documentRegistersName && r.Status == "Active", logger);
      if (activeDocumentRegisters != null)
      {
        logger.Info(string.Format("Журнал регистрации {0} уже существует", documentRegistersName));
        return;
      }
      var numberFormatItems = documentRegisters.NumberFormatItems;
      documentRegisters.NumberFormatItems = null;
      var newDocumentRegisters = BusinessLogic.CreateEntity<IDocumentRegisters>(documentRegisters, logger);
      //TODO ошибка удалении дефолтных, заполняются при создании, удалить не получилось
      //var test = BusinessLogic.InstanceOData().For<IDocumentRegisters>().Key(newDocumentRegisters).NavigateTo(x => x.NumberFormatItems).DeleteEntriesAsync().Result;
      //foreach (var numberFormatItem in numberFormatItems)
      //{
      //  var updatedRecipientsLinks = BusinessLogic.InstanceOData().For<IDocumentRegisters>().Key(newDocumentRegisters).NavigateTo(x => x.NumberFormatItems).Set(numberFormatItem).InsertEntryAsync().Result;
      //}
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
        var jsonBody = JsonConvert.DeserializeObject<IEnumerable<IDocumentRegisters>>(jsonText);

        var index = 1;
        var jsonItemsCount = jsonBody.Count();
        foreach (var jsonItem in jsonBody)
        {
          try
          {
            logger.Info(string.Format("Запись {0} из {1}", index, jsonItemsCount));
            index++;
            ImportDocumentRegisters(jsonItem, logger);
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
          .For<IDocumentRegisters>()
          .Filter(c => c.Status == "Active")
          .Expand(c => c.RegistrationGroup)
          .Expand(c => c.NumberFormatItems)
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
