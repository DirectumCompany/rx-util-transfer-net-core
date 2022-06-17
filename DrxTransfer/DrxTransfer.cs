using Sungero.Logging;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.ComponentModel.Composition.Hosting;

namespace DrxTransfer
{
  class DrxTransfer
  {
    private const string DefaultConfigSettingsName = @"_ConfigSettings.xml";

    internal static ILog Logger => Logs.GetLogger<DrxTransfer>();

    static int Main(string[] args)
    {
      var exitCode = 0;
      try
      {
        Logs.Сonfiguration.Configure();
        ConfigSettingsService.SetSourcePath(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, DefaultConfigSettingsName));

        var rootCommand = new RootCommand();
        rootCommand.Add(new DrxTransferCommand("transfer", "info"));

        exitCode = rootCommand.Invoke(args);
      }
      catch (Exception ex)
      {
        Logger.Log(LogLevel.Error, ex, ex.Message);
        exitCode = -1;
      }

      return exitCode;
    }
  }
}
