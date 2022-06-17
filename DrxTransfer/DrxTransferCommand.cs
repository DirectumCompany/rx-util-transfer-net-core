using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Reflection;
using System.Text;

namespace DrxTransfer
{
  public class DrxTransferCommand : Command
  {
    public DrxTransferCommand(string name, string description)
      : base(name, description)
    {
      // Импортировать сущности.
      this.Add(new BaseCommand("import", "Import entities into Directum RX from a specified location.")
               {
                 new Argument<string>("path", "Full path to the file with entities to import."),
                 new Argument<string>("entity", "entity."),
               }.WithHandler(typeof(DrxTransferCommand), nameof(ImportHandler)));

      // Экспортировать сущности.
      this.Add(new BaseCommand("export", "Export entities from Directum RX to a specified location.")
               {
                 new Argument<string>("path", "Full path to the file to which entities will be exported."),
                 new Argument<string>("entity", "entity."),
               }.WithHandler(typeof(DrxTransferCommand), nameof(ExportHandler)));
    }

    /// <summary>
    /// Обработчик команды импорта сущностей.
    /// </summary>
    /// <param name="username">Имя пользователя.</param>
    /// <param name="password">Пароль.</param>
    /// <param name="service">Адрес сервиса интеграции.</param>
    /// <param name="path">Путь до файла, откуда импортировать.</param>
    /// <returns>Код возврата.</returns>
    public static int ImportHandler(string username, string password, string service, string path, string entity)
    {
      int exitCode = IntegrationServiceClient.Setup(username, password, service);
      return (exitCode == 0) ? TransferEngine.Instance.Import(path, entity) : exitCode;
    }

    /// <summary>
    /// Обработчик команды экспорта сущностей.
    /// </summary>
    /// <param name="username">Имя пользователя.</param>
    /// <param name="password">Пароль.</param>
    /// <param name="service">Адрес сервиса интеграции.</param>
    /// <param name="path">Путь до файла, куда экспортировать.</param>
    /// <returns>Код возврата.</returns>
    public static int ExportHandler(string username, string password, string service, string path, string entity)
    {
      int exitCode = IntegrationServiceClient.Setup(username, password, service);
      return (exitCode == 0) ? TransferEngine.Instance.Export(path, entity) : exitCode;
    }
  }
}
