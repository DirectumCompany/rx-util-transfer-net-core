using Sungero.Logging;

namespace DrxTransfer
{
  class UserCredentialLoggerExtension : ILoggerExtension
  {
    private string UserName { get; }

    private string Tenant { get; }

    private const string UserNamePropertyName = "userName";
    private const string TenantPropertyName = "tenant";

    public UserCredentialLoggerExtension(string tenant, string userName)
    {
      this.UserName = userName;
      this.Tenant = tenant;
    }

    /// <summary>
    /// Изменить события логирования.
    /// </summary>
    /// <param name="logEvent">Событие логирования.</param>
    public void ModifyLogEvent(LogEvent logEvent)
    {
      logEvent.Properties[UserNamePropertyName] = this.UserName;
      logEvent.Properties[TenantPropertyName] = this.Tenant;
    }
  }
}
