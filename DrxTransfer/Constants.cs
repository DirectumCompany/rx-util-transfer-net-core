using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrxTransfer
{
  class Constants
  {
    public class Actions
    {
      public const string ApprovalRule = "approvalrule";
      public const string ContractCategory = "contractcategory";
      public const string DocumentKind = "documentkind";
      public const string DocumentRegister = "documentregister";
      public const string RegistrationGroup = "registrationgroup";
      public const string RegistrationSetting = "registrationsetting";
      public const string Role = "role";
      public static Dictionary<string, string> dictActions = new Dictionary<string, string>
      {
        {ApprovalRule, ApprovalRule},
        {ContractCategory, ContractCategory},
        {DocumentKind, DocumentKind},
        {DocumentRegister, DocumentRegister},
        {RegistrationGroup, RegistrationGroup},
        {RegistrationSetting, RegistrationSetting},
        {Role, Role},
      };
    }
    public class ConfigServices
    {
      public const string IntegrationServiceUrlParamName = "INTEGRATION_SERVICE_URL";
      public const string RequestTimeoutParamName = "INTEGRATION_SERVICE_REQUEST_TIMEOUT";
    }

  }
}