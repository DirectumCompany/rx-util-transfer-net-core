using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;

namespace DrxTransfer
{
  /// <summary>
  /// Репозиторий сериализаторов.
  /// </summary>
  class SerializersRepository
  {
    #region Singletone Implementation

    private static SerializersRepository instance;

    public static SerializersRepository Instance
    {
      get
      {
        if (instance == null)
          instance = new SerializersRepository();
        return instance;
      }
    }

    #endregion

    #region Поля и свойства

    /// <summary>
    /// Активные сериализаторы.
    /// </summary>
    private List<SungeroSerializer> activeSerializers;

    #endregion

    /// <summary>
    /// Получить сериализатор по указанному типу.
    /// </summary>
    /// <param name="entityName">Тип.</param>
    /// <returns>Сериализатор.</returns>
    public SungeroSerializer GetSerializerForEntityType(string entityName)
    {
      return activeSerializers.FirstOrDefault(s => s.EntityName == entityName);
    }

    /// <summary>
    /// Загрузить сериализаторы.
    /// </summary>
    /// <returns>Загруженные сериализаторы.</returns>
    private IEnumerable<SungeroSerializer> LoadSerializers()
    {
      try
      {
        var catalog = new AggregateCatalog();
        catalog.Catalogs.Add(new AssemblyCatalog(typeof(SungeroSerializer).Assembly));
        catalog.Catalogs.Add(new DirectoryCatalog(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Entities")));
        var compositionContainer = new CompositionContainer(catalog);

        return compositionContainer.GetExportedValues<SungeroSerializer>();
      }
      catch (Exception ex)
      {
        throw new InvalidDataException("Ошибка при загрузке сериализаторов", ex);
      }
    }

    #region Конструктор

    private SerializersRepository()
    {
      this.activeSerializers = this.LoadSerializers().ToList();
    }

    #endregion
  }
}
