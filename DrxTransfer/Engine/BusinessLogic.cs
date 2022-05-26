using System;
using System.Linq;
using System.Collections.Generic;
using NLog;
using System.IO;
using DrxTransfer.IntegrationServicesClient;
using Simple.OData.Client;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace DrxTransfer
{
  public class BusinessLogic
  {
    public static IEnumerable<string> ErrorList;

    /// <summary>
    /// Чтение атрибута EntityName.
    /// </summary>
    /// <param name="t">Тип класса.</param>
    /// <returns>Значение атрибута EntityName.</returns>
    private static string PrintInfo(Type t)
    {
      Attribute[] attrs = Attribute.GetCustomAttributes(t);

      foreach (Attribute attr in attrs)
      {
        if (attr is EntityName)
        {
          EntityName a = (EntityName)attr;

          return a.GetName();
        }
      }

      return string.Empty;
    }

    /// <summary>
    /// Получение экземпляра клиента OData.
    /// </summary>
    /// <returns>ODataClient.</returns>
    /// <remarks></remarks>
    public static ODataClient InstanceOData()
    {
      return Client.Instance();
    }

    /// <summary>
    /// Получение сущности по фильтру.
    /// </summary>
    /// <typeparam name="T">Тип сущности.</typeparam>
    /// <param name="expression">Условие фильтрации.</param>
    /// <param name="exceptionList">Список ошибок.</param>
    /// <param name="logger">Логгер</param>
    /// <returns>Сущность.</returns>
    public static IEnumerable<T> GetEntitiesWithFilter<T>(Expression<Func<T, bool>> expression, Logger logger) where T : class
    {
      Expression<Func<T, bool>> condition = expression;
      var filter = new ODataExpression(condition);

      logger.Info(string.Format("Получение сущности {0}", PrintInfo(typeof(T))));

      try
      {
        var entities = Client.GetEntitiesByFilter<T>(filter);
        return entities;
      }
      catch (Exception ex)
      {
        logger.Error(ex.Message);
      }
      return null;
    }

    /// <summary>
    /// Создать сущность.
    /// </summary>
    /// <typeparam name="T">Тип сущности.</typeparam>
    /// <param name="entity">Экземпляр сущности.</param>
    /// <returns>Созданная сущность.</returns>
    public static T CreateEntity<T>(T entity, Logger logger) where T : class
    {
      logger.Info(string.Format("Создание сущности {0}", PrintInfo(typeof(T))));
      try
      {
        var entities = Client.CreateEntity<T>(entity);

        return entities;
      }
      catch (Exception ex)
      {
        logger.Error(ex.Message);
      }
      return null;
    }

    /// <summary>
    /// Обновить сущность.
    /// </summary>
    /// <typeparam name="T">Тип сущности.</typeparam>
    /// <param name="entity">Экземпляор сущности.</param>
    /// <returns>Обновленная сущность.</returns>
    public static T UpdateEntity<T>(T entity, Logger logger) where T : class
    {
      var entities = Client.UpdateEntity<T>(entity);

      logger.Info(string.Format("Тип сущности {0} обновлен.", PrintInfo(typeof(T))));

      return entities;
    }

  }
}