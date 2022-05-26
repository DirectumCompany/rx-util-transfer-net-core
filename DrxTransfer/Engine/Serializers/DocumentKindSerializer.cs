using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DrxTransfer.IntegrationServicesClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DrxTransfer
{
  class DocumentKindSerializer
  {
    #region Singletone Implementation

    private static DocumentKindSerializer instance;
    public static DocumentKindSerializer Instance
    {
      get
      {
        if (instance == null)
          instance = new DocumentKindSerializer();
        return instance;
      }
    }

    #endregion

    public void ImportDocumentKinds(IDocumentKinds documentKinds, NLog.Logger logger)
    {
      var documentKindsName = documentKinds.Name;

      var activeDocumentKinds = BusinessLogic.GetEntitiesWithFilter<IDocumentKinds>(r => r.Name == documentKindsName && r.Status == "Active", logger);
      if (activeDocumentKinds != null)
      {
        logger.Info(string.Format("Вид документа {0} уже существует", documentKindsName));
        return;
      }

      var availableActions = documentKinds.AvailableActions;
      documentKinds.AvailableActions = null;

      var newDocumentKinds = BusinessLogic.CreateEntity<IDocumentKinds>(documentKinds, logger);
      // TODO ошибка с дублями действий, заполняются при создании, удалить не получилось
      //var deleteActions = BusinessLogic.InstanceOData().For<IDocumentKinds>().Key(newDocumentKinds).NavigateTo(x => x.AvailableActions).DeleteEntriesAsync().Result;
      foreach (var availableAction in availableActions)
      {
        var updatedDocumentKinds = BusinessLogic.InstanceOData().For<IDocumentKinds>().Key(newDocumentKinds).NavigateTo(x => x.AvailableActions).Set(availableAction).InsertEntryAsync().Result;
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
        var jsonBody = JsonConvert.DeserializeObject<IEnumerable<IDocumentKinds>>(jsonText);

        var index = 1;
        var jsonItemsCount = jsonBody.Count();
        foreach (var jsonItem in jsonBody)
        {
          try
          {
            logger.Info(string.Format("Запись {0} из {1}", index, jsonItemsCount));
            index++;
            ImportDocumentKinds(jsonItem, logger);
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
        var documentKinds = BusinessLogic.InstanceOData()
          .For<IDocumentKinds>()
          .Filter(c => c.Status == "Active")
          .Expand(c => c.DocumentType)
          .Expand(c => c.AvailableActions.Select(c => c.Action))
          .FindEntriesAsync()
          .Result;
        var documentKindsCount = documentKinds.Count();
        logger.Info(string.Format("Найдено {0} объектов", documentKindsCount));
        string JSONBody = JsonConvert.SerializeObject(documentKinds);

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
