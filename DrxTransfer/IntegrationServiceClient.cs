using Simple.OData.Client;
using Sungero.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;

namespace DrxTransfer
{
  public class IntegrationServiceClient : Simple.OData.Client.ODataClient
  {
    #region Constants

    private const string IntegrationServiceUrlParamName = "INTEGRATION_SERVICE_URL";
    private const string RequestTimeoutParamName = "INTEGRATION_SERVICE_REQUEST_TIMEOUT";

    #endregion

    #region Client Singletone impl

    private static IntegrationServiceClient instance;

    /// <summary>
    /// Экземпляр клиента.
    /// </summary>
    public static IntegrationServiceClient Instance
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

    internal static ILog Logger => Logs.GetLogger<IntegrationServiceClient>();

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

    #region Methods

    /// <summary>
    /// Получение сущности по фильтру.
    /// </summary>
    /// <typeparam name="T">Тип сущности.</typeparam>
    /// <param name="expression">Условие фильтрации.</param>
    /// <param name="exceptionList">Список ошибок.</param>
    /// <param name="logger">Логгер</param>
    /// <returns>Сущность.</returns>
    public static IEnumerable<T> GetEntitiesWithFilter<T>(Expression<Func<T, bool>> expression) where T : class
    {
      Expression<Func<T, bool>> condition = expression;
      var filter = new ODataExpression(condition);

      Logger.Info(string.Format("Получение сущности {0}", (typeof(T))));

      try
      {
        var entities = GetEntitiesByFilter<T>(filter);
        return entities;
      }
      catch (Exception ex)
      {
        Logger.Error(ex.Message);
      }
      return null;
    }

    /// <summary>
    /// Получение сущностей по фильтру.
    /// </summary>
    /// <typeparam name="T">Тип сущности.</typeparam>
    /// <param name="expression">Условие фильтрации.</param>
    /// <returns>Сущность.</returns>
    public static IEnumerable<T> GetEntitiesByFilter<T>(ODataExpression expression) where T : class
    {
      var data = Instance.For<T>().Filter(expression).FindEntriesAsync().Result;

      return data;
    }

    /// <summary>
    /// Создать сущность.
    /// </summary>
    /// <typeparam name="T">Тип сущности.</typeparam>
    /// <param name="entity">Экзмемпляр сущности.</param>
    /// <returns>Созданна сущность.</returns>
    public static T CreateEntity<T>(T entity) where T : class
    {
      var data = Instance.For<T>().Set(entity).InsertEntryAsync().Result;

      return data;
    }

    /// <summary>
    /// Обновить сущность.
    /// </summary>
    /// <typeparam name="T">Тип сущности.</typeparam>
    /// <param name="entity">Экзмемпляр сущности.</param>
    /// <returns>Обновленная сущность.</returns>
    public static T UpdateEntity<T>(T entity) where T : class
    {
      var data = Instance.For<T>().Key(entity).Set(entity).UpdateEntryAsync().Result;

      return data;
    }

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
        // Имя и пароль - обязательные параметры для подключения к сервису.
        if (string.IsNullOrWhiteSpace(userName))
          throw new ArgumentException("Username is null or empty.", nameof(userName));
        if (string.IsNullOrWhiteSpace(password))
          throw new ArgumentException("Password is null or empty.", nameof(password));

        // Адрес сервиса взять либо из командной строки, либо из конфига.
        if (string.IsNullOrWhiteSpace(serviceUrl))
        {
          serviceUrl = ConfigSettingsService.GetConfigSettingsValueByName(IntegrationServiceUrlParamName);
          if (string.IsNullOrWhiteSpace(serviceUrl))
            throw new Exception("Integration service URL not found.");
        }

        CollectionHelper.Setup(userName, password, serviceUrl);
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
    /// Вызвать прикладную GET-функцию.
    /// </summary>
    /// <param name="moduleName">Имя модуля.</param>
    /// <param name="functionName">Имя функции.</param>
    /// <returns>Результат выполнения функции.</returns>
    public static dynamic RunFunction(string moduleName, string functionName)
    {
      try
      {
        var request = Instance
          .For(moduleName)
          .Function(functionName)
          .ExecuteAsSingleAsync();

        // Результат выполнения функции содержится в элементе словаря с ключом "__result".
        // Если пришел null, возвращаем null, так как словаря в этом случае нет.
        return request.Result?["__result"];
      }
      catch (Exception ex)
      {
        throw GetSimpleODataInnerException(ex);
      }
    }

    /// <summary>
    /// Вызвать прикладную GET-функцию с параметрами.
    /// </summary>
    /// <param name="moduleName">Имя модуля.</param>
    /// <param name="functionName">Имя функции.</param>
    /// <param name="parameters">Список параметров.</param>
    /// <returns>Результат выполнения функции.</returns>
    public static dynamic RunFunction(string moduleName, string functionName, object parameters)
    {
      try
      {
        var request = Instance
          .For(moduleName)
          .Function(functionName)
          .Set(parameters)
          .ExecuteAsSingleAsync();

        // Результат выполнения функции содержится в элементе словаря с ключом "__result".
        // Если пришел null, возвращаем null, так как словаря в этом случае нет.
        return request.Result?["__result"];
      }
      catch (Exception ex)
      {
        throw GetSimpleODataInnerException(ex);
      }
    }

    /// <summary>
    /// Вызвать прикладную GET-функцию с параметрами.
    /// </summary>
    /// <typeparam name="T">Тип возвращаемого значения.</typeparam>
    /// <param name="moduleName">Имя модуля.</param>
    /// <param name="functionName">Имя функции.</param>
    /// <param name="parameters">Список параметров.</param>
    /// <returns>Результат выполнения функции.</returns>
    public static T RunFunction<T>(string moduleName, string functionName, object parameters)
    {
      try
      {
        return Instance
        .For(moduleName)
        .Function(functionName)
        .Set(parameters)
        .ExecuteAsSingleAsync<T>().Result;
      }
      catch (Exception ex)
      {
        throw GetSimpleODataInnerException(ex);
      }
    }

    /// <summary>
    /// Вызвать прикладную GET-функцию с параметрами.
    /// </summary>
    /// <param name="moduleName">Имя модуля.</param>
    /// <param name="functionName">Имя функции.</param>
    /// <param name="parameters">Список параметров.</param>
    /// <returns>Результат выполнения функции.</returns>
    public static IDictionary<string, object> RunFunctionWithStructureResult(string moduleName, string functionName, object parameters)
    {
      try
      {
        return Instance
          .For(moduleName)
          .Function(functionName)
          .Set(parameters)
          .ExecuteAsSingleAsync().Result;
      }
      catch (Exception ex)
      {
        throw GetSimpleODataInnerException(ex);
      }
    }

    /// <summary>
    /// Вызвать прикладную POST-функцию.
    /// </summary>
    /// <param name="moduleName">Имя модуля.</param>
    /// <param name="functionName">Имя функции.</param>
    public static void RunAction(string moduleName, string functionName)
    {
      try
      {
        Instance
          .For(moduleName)
          .Action(functionName)
          .ExecuteAsync()
          .Wait();
      }
      catch (Exception ex)
      {
        throw GetSimpleODataInnerException(ex);
      }
    }

    /// <summary>
    /// Вызвать прикладную POST-функцию с параметрами.
    /// </summary>
    /// <param name="moduleName">Имя модуля.</param>
    /// <param name="functionName">Имя функции.</param>
    /// <param name="parameters">Список параметров.</param>
    public static void RunAction(string moduleName, string functionName, object parameters)
    {
      try
      {
        Instance
          .For(moduleName)
          .Action(functionName)
          .Set(parameters)
          .ExecuteAsync()
          .Wait();
      }
      catch (Exception ex)
      {
        throw GetSimpleODataInnerException(ex);
      }
    }

    public static Exception GetSimpleODataInnerException(Exception ex)
    {
      if (ex.InnerException is Simple.OData.Client.WebRequestException)
      {
        var webRequestException = ex.InnerException as Simple.OData.Client.WebRequestException;
        Logger.Trace(string.Format("Integration service response: {0}", webRequestException.Response));
        return new Exception($"{ex.Message}. See log for details", ex);
      }
      return ex;
    }

    #endregion

    #region Constructors
    private IntegrationServiceClient(Simple.OData.Client.ODataClientSettings settings) : base(settings)
    { }

    /// <summary>
    /// Получить новый экземпляр клиента для работы с сервисом интеграции.
    /// </summary>
    /// <param name="userName">Имя пользователя.</param>
    /// <param name="password">Пароль.</param>
    /// <param name="serverUrl">Адрес сервиса интеграции.</param>
    /// <returns>Клиент.</returns>
    private static IntegrationServiceClient GetNewClient(string userName, string password, string serverUrl)
    {
      var timeout = ConfigSettingsService.GetIntParamValue(RequestTimeoutParamName, "600");
      var settings = new Simple.OData.Client.ODataClientSettings(new Uri(serverUrl));
      settings.RequestTimeout = new TimeSpan(0, 0, timeout);
      settings.BeforeRequest += delegate (HttpRequestMessage message)
      {
        var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", userName, password)));
        message.Headers.Add("Authorization", "Basic " + authHeaderValue);
      };
      return new IntegrationServiceClient(settings);
    }

    #endregion

  }
}
