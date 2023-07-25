using DrxTransfer.IntegrationServicesClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sungero.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DrxTransfer
{
  /// <summary>
  /// Конвертация объектов.
  /// </summary>
  class TransferEngine
  {
    internal static ILog Logger => Logs.GetLogger<TransferEngine>();

    #region Singletone Implementation

    private static TransferEngine instance;
    public static TransferEngine Instance
    {
      get
      {
        if (instance == null)
          instance = new TransferEngine();
        return instance;
      }
    }

    #endregion

    #region Методы

    /// <summary>
    /// Запуск импорта данных.
    /// </summary>
    /// <param name="filePath">Путь к файлу.</param>
    public int Import(string filePath, string entityType)
    {
      try
      {
        string jsonText = string.Empty;
        using (StreamReader sr = new StreamReader(filePath, System.Text.Encoding.Default))
        {
          Logger.Debug("Чтение файла");
          jsonText = sr.ReadToEnd();
        }

        Logger.Debug("Загрузка сериализатора");
        var serializer = SerializersRepository.Instance.GetSerializerForEntityType(entityType);

        if (serializer != null)
        {

          Logger.Debug(string.Format("Сериализатор {0} успешно загружен", entityType));
          var jsonBody = JsonConvert.DeserializeObject(jsonText, typeof(IEnumerable<object>)) as IEnumerable<object>;
          var index = 1;
          var jsonItemsCount = jsonBody.Count();
          foreach (var jsonItem in jsonBody)
          {
            try
            {
              Logger.Debug(string.Format("Запись {0} из {1}", index, jsonItemsCount));
              index++;
              serializer.Import(jsonItem);
            }
            catch (Exception ex)
            {
              Logger.Error(ex);
              Logger.Error("Запись не создана");
            }
          }
        }
        else
        {
          Logger.Error(string.Format("Сериализатор {0} не найден", entityType));
          return -1;
        }
      }
      catch (Exception ex)
      {
        Logger.Error(ex);
        return -1;
      }

      return 0;
    }

    /// <summary>
    /// Запуск экспорта данных в json файл.
    /// </summary>
    /// <param name="filePath">Путь к файлу.</param>
    public int Export(string filePath, string entityType)
    {
      try
      {
        Logger.Debug("Загрузка сериализатора");
        var serializer = SerializersRepository.Instance.GetSerializerForEntityType(entityType);

        if (serializer != null)
        {
          Logger.Debug(string.Format("Сериализатор {0} успешно загружен", entityType));
          serializer.Serialize(filePath);
        }
        else
        {
          Logger.Error(string.Format("Сериализатор {0} не найден", entityType));
          return -1;
        }
      }
      catch (Exception ex)
      {
        Logger.Error(ex);
        return -1;
      }

      return 0;
    }
    #endregion
  }
}
