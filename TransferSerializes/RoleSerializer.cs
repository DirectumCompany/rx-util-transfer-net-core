using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using DrxTransfer;
using DrxTransfer.IntegrationServicesClient;
using Newtonsoft.Json.Linq;
using Simple.OData.Client;

namespace TransferSerializes
{
  [Export(typeof(SungeroSerializer))]
  class RoleSerializer : SungeroSerializer
  {    
    public RoleSerializer() : base()
    {
      this.EntityName = "Role";
      this.EntityType = "IRole";
    }

    public override void Import(object jsonObject)
    {
      var role = (jsonObject as JObject).ToObject<IRole>();

      var roleName = role.Name;

      var activeRole = IntegrationServiceClient.GetEntitiesWithFilter<IRole>(r => r.Name == roleName && r.Status == "Active");
      if (activeRole != null)
      {
        Logger.Info(string.Format("Роль {0} уже существует", roleName));
        return;
      }

      var recipients = role.RecipientLinks;
      var isSingle = role.IsSingleUser;
      role.RecipientLinks = null;
      role.IsSingleUser = false;
      var newRole = IntegrationServiceClient.CreateEntity<IRole>(role);
      foreach (var recipientItem in recipients.Select(r => r.Member))
      {
        var recipient = IntegrationServiceClient.GetEntitiesWithFilter<IRecipients>(u => u.Name == recipientItem.Name && u.Status == "Active").FirstOrDefault();
        IntegrationServiceClient.Instance.For<IRole>().Key(newRole).NavigateTo(x => x.RecipientLinks).Set( new { Member = recipient }).InsertEntryAsync().Wait();
      }
      if (isSingle)
      {
        newRole.IsSingleUser = true;
        newRole = IntegrationServiceClient.UpdateEntity<IRole>(newRole);
      }
    }

    protected override IEnumerable<dynamic> Export()
    {
      return IntegrationServiceClient.Instance.For<IRole>()
          .Filter(c => c.Status == "Active")
          .Expand(c => c.RecipientLinks.Select(c => c.Member))
          .FindEntriesAsync()
          .Result;
    }
  }
}
