using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DrxTransfer.IntegrationServicesClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DrxTransfer
{
  class ContractCategorySerializer
  {
    #region Singletone Implementation

    private static ContractCategorySerializer instance;
    public static ContractCategorySerializer Instance
    {
      get
      {
        if (instance == null)
          instance = new ContractCategorySerializer();
        return instance;
      }
    }

    #endregion

    public void ImportContractCategory(IContractCategory contractCategory, NLog.Logger logger)
    {
      var contractCategoryName = contractCategory.Name;

      var activeContractCategory = BusinessLogic.GetEntitiesWithFilter<IContractCategory>(r => r.Name == contractCategoryName && r.Status == "Active", logger);
      if (activeContractCategory != null)
      {
        logger.Info(string.Format("Категория договора {0} уже существует", contractCategoryName));
        return;
      }

      var documentKinds = contractCategory.DocumentKinds;
      contractCategory.DocumentKinds = null;
      
      var newContractCategory = BusinessLogic.CreateEntity<IContractCategory>(contractCategory, logger);
      foreach (var documentKind in documentKinds)
      {
        var updatedDocumentKinds = BusinessLogic.InstanceOData().For<IContractCategory>().Key(newContractCategory).NavigateTo(x => x.DocumentKinds).Set(documentKind).InsertEntryAsync().Result;
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
        var jsonBody = JsonConvert.DeserializeObject<IEnumerable<IContractCategory>>(jsonText);

        var index = 1;
        var jsonItemsCount = jsonBody.Count();
        foreach (var jsonItem in jsonBody)
        {
          try
          {
            logger.Info(string.Format("Запись {0} из {1}", index, jsonItemsCount));
            index++;
            ImportContractCategory(jsonItem, logger);
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
        var contractCategories = BusinessLogic.InstanceOData()
          .For<IContractCategory>()
          .Filter(c => c.Status == "Active")
          .Expand(c => c.DocumentKinds.Select(c => c.DocumentKind))
          .FindEntriesAsync()
          .Result;
        var contractCategoriesCount = contractCategories.Count();
        logger.Info(string.Format("Найдено {0} объектов", contractCategoriesCount));
        string JSONBody = JsonConvert.SerializeObject(contractCategories);

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
