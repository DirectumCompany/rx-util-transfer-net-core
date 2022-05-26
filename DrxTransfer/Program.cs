using DrxTransfer.IntegrationServicesClient;
using NDesk.Options;
using NLog;
using NLog.Config;
using System;
using System.IO;

namespace DrxTransfer
{
  class Program
  {
    public static Logger logger = LogManager.GetCurrentClassLogger();
    private const string DefaultConfigSettingsName = @"_ConfigSettings.xml";

    /// <summary>
    /// Выполнение переноса в соответствии с типом.
    /// </summary>
    /// <param name="type">Тип объекта.</param>
    /// <param name="file">Файл.</param>
    /// <param name="logger">Логировщик.</param>
    /// <returns>Соответствующий тип сущности.</returns>
    static void ProcessByAction(string type, string file, bool export, NLog.Logger logger)
    {
      switch (type)
      {
        case "Role":
          logger.Info("Перенос роли");
          logger.Info("-------------");
          if (export)
            RoleSerializer.Instance.Export(file, logger);
          else
            RoleSerializer.Instance.Import(file, logger);
          break;
        case "ContractCategory":
          logger.Info("Перенос категорий договоров");
          logger.Info("-------------");
          if (export)
            ContractCategorySerializer.Instance.Export(file, logger);
          else
            ContractCategorySerializer.Instance.Import(file, logger);
          break;
        case "DocumentKind":
          logger.Info("Перенос видов документа");
          logger.Info("-------------");
          if (export)
            DocumentKindSerializer.Instance.Export(file, logger);
          else
            DocumentKindSerializer.Instance.Import(file, logger);
          break;
        case "RegistrationGroup":
          logger.Info("Перенос групп регистрации");
          logger.Info("-------------");
          if (export)
            RegistrationGroupSerializer.Instance.Export(file, logger);
          else
            RegistrationGroupSerializer.Instance.Import(file, logger);
          break;
        case "DocumentRegister":
          logger.Info("Перенос журналов регистрации");
          logger.Info("-------------");
          if (export)
            DocumentRegisterSerializer.Instance.Export(file, logger);
          else
            DocumentRegisterSerializer.Instance.Import(file, logger);
          break;
        case "RegistrationSetting":
          logger.Info("Перенос настроек регистрации");
          logger.Info("-------------");
          if (export)
            RegistrationSettingSerializer.Instance.Export(file, logger);
          else
            RegistrationSettingSerializer.Instance.Import(file, logger);
          break;
        case "ApprovalRule":
          logger.Info("Перенос правил согласования");
          logger.Info("-------------");
          if (export)
            ApprovalRuleSerializer.Instance.Export(file, logger);
          else
            ApprovalRuleSerializer.Instance.Import(file, logger);
          break;
      }
    }
    static void Main(string[] args)
    {
      try
      {
        LogManager.Configuration = new XmlLoggingConfiguration(AppDomain.CurrentDomain.BaseDirectory + "nlog.config");
        logger.Info("=========================== Process Start ===========================");
        #region Обработка параметров.

        var login = string.Empty;
        var password = string.Empty;
        var file = string.Empty;
        var type = string.Empty;
        var action = string.Empty;
        bool isHelp = false;

        var p = new OptionSet() {
                { "n|name=",  "Имя учетной записи DirectumRX.", v => login = v },
                { "p|password=",  "Пароль учетной записи DirectumRX.", v => password = v },
                { "f|inFile=",  "Файл", v => file = v },
                { "a|action=",  "Экспорт или Импорт", v => action = v },
                { "j|type=",  "Тип объекта", v => type = v },
                { "h|help", "Показать справку", v => isHelp = (v != null) },
              };

        try
        {
          p.Parse(args);
        }
        catch (OptionException e)
        {
          Console.WriteLine("Invalid arguments: " + e.Message);
          p.WriteOptionDescriptions(Console.Out);
          return;
        }

        if (isHelp || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(file) 
          || string.IsNullOrEmpty(type) || string.IsNullOrEmpty(action))
        {
          p.WriteOptionDescriptions(Console.Out);
          return;
        }

        #endregion

        if (!Constants.Actions.dictActions.ContainsKey(type.ToLower()))
        {
          var message = $"Не найден тип объекта \"{type}\". Введите действие корректно.";
          throw new Exception(message);
        }

        try
        {
          #region Аутентификация.
          ConfigSettingsService.SetSourcePath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultConfigSettingsName));
          Client.Setup(login, password, logger);
          #endregion

          #region Выполнение переноса настроек.
          if (action == "export")
            ProcessByAction(type, file, true, logger);
          else if (action == "import")
            ProcessByAction(type, file, false, logger);
          else
            logger.Error("Не найдено действие");
          #endregion
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          logger.Error(ex.Message);
        }

      }
      catch (Exception ex)
      {
        logger.Error(ex.Message);
      }

    }
  }
}
