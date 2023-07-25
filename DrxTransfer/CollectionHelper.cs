using Newtonsoft.Json;
using Sungero.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace DrxTransfer
{
  public class CollectionHelper : HttpClient
  {
    #region Client Singletone impl

    private static CollectionHelper instance;

    /// <summary>
    /// Экземпляр клиента.
    /// </summary>
    public static CollectionHelper Instance
    {
      get
      {
        if (instance == null)
          throw new ArgumentException("At first perform Setup() to create an instance.");

        return instance;
      }
    }

    #endregion

    #region Fields and Properties

    internal static ILog Logger => Logs.GetLogger<CollectionHelper>();

    /// <summary>
    /// URL сервиса.
    /// </summary>
    public string ServerUrl { get; private set; }

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    public string UserName { get; private set; }

    /// <summary>
    /// Пароль.
    /// </summary>
    public string Password { get; private set; }

    #endregion

    /// <summary>
    /// Установить параметры подключения к сервису.
    /// </summary>
    /// <param name="userName">Имя пользователя.</param>
    /// <param name="password">Пароль.</param>
    /// <param name="serviceUrl">Адрес сервиса интеграции.</param>
    public static int Setup(string userName, string password, string serviceUrl)
    {
      try
      {
        if (instance == null)
          instance = GetNewClient(userName, password, serviceUrl);

        Logs.Сonfiguration.RegisterExtension(new UserCredentialLoggerExtension(instance.ServerUrl, instance.UserName));
      }
      catch (Exception ex)
      {
        Logger.Log(LogLevel.Error, ex, ex.Message);

        return -1;
      }

      return 0;
    }

    /// <summary>
    /// Обновить сущность.
    /// </summary>
    /// <typeparam name="T">Тип сущности.</typeparam>
    /// <param name="entity">Экзмемпляр сущности.</param>
    /// <returns>Обновленная сущность.</returns>
    public static void UpdateCellectionItem(string entityName, string entityId, string collectionName, string collectionItemId, object collection)
    {
      var json = JsonConvert.SerializeObject(collection);
      var content = new StringContent(json, Encoding.UTF8, "application/json");
      var uri = string.Format("{0}({1})/{2}({3})", entityName, entityId, collectionName, collectionItemId);
      var response = Instance.PatchAsync(uri, content).Result;
    }

    /// <summary>
    /// Обновить сущность.
    /// </summary>
    /// <typeparam name="T">Тип сущности.</typeparam>
    /// <param name="entity">Экзмемпляр сущности.</param>
    /// <returns>Обновленная сущность.</returns>
    public static void DeleteCellectionItem(string entityName, string entityId, string collectionName, string collectionItemId)
    {
      var uri = string.Format("{0}({1})/{2}({3})", entityName, entityId, collectionName, collectionItemId);
      var response = Instance.DeleteAsync(uri).Result;
    }

    /// <summary>
    /// Обновить сущность.
    /// </summary>
    /// <typeparam name="T">Тип сущности.</typeparam>
    /// <param name="entity">Экзмемпляр сущности.</param>
    /// <returns>Обновленная сущность.</returns>
    public static void CellectionItemsClear(string entityName, string entityId, string collectionName)
    {
      var uri = string.Format("{0}({1})/{2}", entityName, entityId, collectionName);
      var response = Instance.DeleteAsync(uri).Result;
    }

    #region Constructors
    private CollectionHelper() : base()
    { }

    /// <summary>
    /// Получить новый экземпляр клиента для работы с сервисом интеграции.
    /// </summary>
    /// <param name="userName">Имя пользователя.</param>
    /// <param name="password">Пароль.</param>
    /// <param name="serverUrl">Адрес сервиса интеграции.</param>
    /// <returns>Клиент.</returns>
    private static CollectionHelper GetNewClient(string userName, string password, string serverUrl)
    {
      var client = new CollectionHelper();
      client.DefaultRequestHeaders.Clear();
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
    "Basic", Convert.ToBase64String(
        Encoding.ASCII.GetBytes($"{userName}:{password}")));

      client.BaseAddress = new Uri(serverUrl);
      return client;
    }
    #endregion
  }
}
