using System;
using ConfigSettings.Patch;

namespace DrxTransfer
{
  public class ConfigSettingsService
  {
    #region Fields and Properties

    private string sourcePath;

    #endregion

    #region Singletone impl

    private static ConfigSettingsService instance;

    /// <summary>
    /// Экземпляр класса с настройками.
    /// </summary>
    public static ConfigSettingsService Instance
    {
      get
      {
        if (instance == null)
          instance = new ConfigSettingsService();
        return instance;
      }
    }

    #endregion

    /// <summary>
    /// Задать путь до файла с настройками.
    /// </summary>
    /// <param name="path">Путь до файла с настройками.</param>
    public static void SetSourcePath(string path)
    {
      Instance.sourcePath = path;
    }

    /// <summary>
    /// Получить значение int-параметра.
    /// </summary>
    /// <param name="name">Имя параметра.</param>
    /// <param name="defaultValue">Значение по умолчанию.</param>
    /// <returns>Значение параметра из _ConfigSettings RxCmd.</returns>
    /// <remarks>Если нет настроек или нет параметра, то вернуть значение по умолчанию.</remarks>
    public static int GetIntParamValue(string name, string defaultValue = "")
    {
      var paramValue = GetConfigSettingsValueByName(name, defaultValue);
      int value;
      int.TryParse(paramValue, out value);
      return value;
    }

    /// <summary>
    /// Получить значение параметра из _ConfigSettings RxCmd.
    /// </summary>
    /// <param name="name">Имя параметра.</param>
    /// <param name="defaultValue">Значение по умолчанию.</param>
    /// <returns>Значение параметра из _ConfigSettings RxCmd.</returns>
    /// <remarks>Если нет настроек или нет параметра, то вернуть значение по умолчанию.</remarks>
    public static string GetConfigSettingsValueByName(string name, string defaultValue = "")
    {
      if (string.IsNullOrWhiteSpace(Instance.sourcePath))
        throw new ArgumentNullException(nameof(Instance.sourcePath));

      var config = new ConfigSettingsParser(Instance.sourcePath);
      if (config != null && config.HasVariable(name))
        return config.GetVariableValue(name);

      if (!string.IsNullOrWhiteSpace(defaultValue))
        return defaultValue;

      // Параметр не указан и нет значения по умолчанию.
      if (config == null || !System.IO.File.Exists(Instance.sourcePath))
        throw new System.IO.FileNotFoundException($"File not found: {Instance.sourcePath}");
      else
        throw new ArgumentException("The parameter must be specified.", name);
    }

    #region ctors
    private ConfigSettingsService()
    {
    }

    #endregion
  }
}
