using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Reflection;
using System.Text;

namespace DrxTransfer
{
  public class BaseCommand : Command
  {
    /// <summary>
    /// Базовая команда управления.
    /// </summary>
    public Command WithHandler(Type type, string name)
    {
      var method = type.GetMethod(name);
      this.Handler = CommandHandler.Create(method);
      return this;
    }

    public BaseCommand(string name, string description)
      : base(name, description)
    {
      this.AddOption(new Option<string>(new[] { "--username", "-n" }, "Directum RX user name."));
      this.AddOption(new Option<string>(new[] { "--password", "-p" }, "Directum RX password."));
      this.AddOption(new Option<string>(new[] { "--service", "-s" }, "Integration service URL."));
    }
  }
}
