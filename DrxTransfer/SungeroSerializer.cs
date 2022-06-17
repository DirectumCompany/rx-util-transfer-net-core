using System;
using System.Collections.Generic;
using Sungero.Logging;
using System.Text;
using DrxTransfer.IntegrationServicesClient;
using Newtonsoft.Json;
using System.IO;

namespace DrxTransfer
{
  /// <summary>
  /// Сериализатор объекта.
  /// </summary>
  public class SungeroSerializer
  {
    public ILog Logger => Logs.GetLogger<SungeroSerializer>();

    // TODO: убрать сеттер, передовать в конструкторе.
    public string EntityName { get; set; }
    public string EntityType { get; set; }

    public Dictionary<string, object> content;

    /// <summary>
    /// Описание экспорта сущности.
    /// </summary>
    /// <param name="entity">Объект.</param>
    /// <returns>Словарь с описанием реквизитов сущности.</returns>
    protected virtual IEnumerable<dynamic> Export()
    {
      return null;
    }

    /// <summary>
    /// Создание, заполнение реквизитов и сохранение сущности.
    /// </summary>
    /// <param name="jsonBody"></param>
    public virtual void Import(object jsonObject)
    {

    }

    /// <summary>
    /// Фильтрация выгружаемых записей.
    /// </summary>
    /// <param name="entities">Все сущности выгружаемого типа.</param>
    /// <returns>Список отфильтрованных сущностей.</returns>
    public virtual IEnumerable<object> Filter(IEnumerable<object> entities)
    {
      return entities;
    }

    /// <summary>
    /// Сериализация объектов в json.
    /// </summary>
    /// <param name="filePath">Путь к файлу для записи.</param>
    internal void Serialize(string filePath)
    {
      Logger.Debug(string.Format("Сериализация объектов типа {0}", this.EntityType));

      var entities = Export();

      List<string> result = new List<string>();
      result.Add(JsonConvert.SerializeObject(this.EntityType, Formatting.Indented));
      //Logger.Info(string.Format("Найдено {0} объектов", rolesCount));
      string JSONBody = JsonConvert.SerializeObject(entities, Formatting.Indented);
      result.Add(JSONBody);

      using (StreamWriter sw = new StreamWriter(filePath, false, System.Text.Encoding.Default))
      {
        Logger.Info("Запись результата в файл");
        sw.WriteLine(JSONBody);
      }
    }
  }
}
