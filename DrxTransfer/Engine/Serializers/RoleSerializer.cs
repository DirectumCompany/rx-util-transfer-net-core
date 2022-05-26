using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DrxTransfer.IntegrationServicesClient;
using Newtonsoft.Json;

namespace DrxTransfer
{
  class RoleSerializer
  {
    #region Singletone Implementation

    private static RoleSerializer instance;
    public static RoleSerializer Instance
    {
      get
      {
        if (instance == null)
          instance = new RoleSerializer();
        return instance;
      }
    }

    #endregion
    public void ImportRole(IRole role, NLog.Logger logger)
    {
      var roleName = role.Name;

      var activeRole = BusinessLogic.GetEntitiesWithFilter<IRole>(r => r.Name == roleName && r.Status == "Active", logger);
      if (activeRole != null)
      {
        logger.Info(string.Format("Роль {0} уже существует", roleName));
        return;
      }

      var recipients = role.RecipientLinks;
      var isSingle = role.IsSingleUser;
      role.RecipientLinks = null;
      role.IsSingleUser = false;
      var newRole = BusinessLogic.CreateEntity<IRole>(role, logger);
      foreach (var recipient in recipients)
      {
        var updatedRecipientsLinks = BusinessLogic.InstanceOData().For<IRole>().Key(newRole).NavigateTo(x => x.RecipientLinks).Set(recipient).InsertEntryAsync().Result;
      }
      if (isSingle)
      {
        newRole.IsSingleUser = true;
        newRole = BusinessLogic.UpdateEntity<IRole>(newRole, logger);
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
        var jsonBody = JsonConvert.DeserializeObject<IEnumerable<IRole>>(jsonText);

        var index = 1;
        var jsonItemsCount = jsonBody.Count();
        foreach (var jsonItem in jsonBody)
        {
          try
          {
            logger.Info(string.Format("Запись {0} из {1}", index, jsonItemsCount));
            index++;
            ImportRole(jsonItem, logger);
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
        var roles = BusinessLogic.InstanceOData()
          .For<IRole>()
          .Filter(c => c.Status == "Active")
          .Expand(c => c.RecipientLinks.Select(c => c.Member))
          .FindEntriesAsync()
          .Result;
        var rolesCount = roles.Count();
        logger.Info(string.Format("Найдено {0} объектов", rolesCount));
        string JSONBody = JsonConvert.SerializeObject(roles);

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
