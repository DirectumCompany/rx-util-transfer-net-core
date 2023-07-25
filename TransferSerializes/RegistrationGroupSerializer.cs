using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using DrxTransfer;
using DrxTransfer.IntegrationServicesClient;
using Newtonsoft.Json.Linq;

namespace TransferSerializes
{
  [Export(typeof(SungeroSerializer))]
  class RegistrationGroupSerializer : SungeroSerializer
  {
    public RegistrationGroupSerializer() : base()
    {
      this.EntityName = "RegistrationGroup";
      this.EntityType = "IRegistrationGroup";
    }

    public override void Import(object jsonObject)
    {
      var registrationGroup = (jsonObject as JObject).ToObject<IRegistrationGroups>();

      var registrationGroupsName = registrationGroup.Name;

      var activeRegistrationGroups = IntegrationServiceClient.GetEntitiesWithFilter<IRegistrationGroups>(r => r.Name == registrationGroupsName && r.Status == "Active");
      if (activeRegistrationGroups != null)
      {
        Logger.Info(string.Format("Группа регистрации {0} уже существует", registrationGroupsName));
        return;
      }

      var recipients = registrationGroup.RecipientLinks;
      var responsibleItem = recipients.Where(r => r.Member.Name == registrationGroup.ResponsibleEmployee.Name).FirstOrDefault();
      recipients.Remove(responsibleItem);
      var departments = registrationGroup.Departments;

      var responsible = IntegrationServiceClient.GetEntitiesWithFilter<IEmployees>(e => e.Name == responsibleItem.Member.Name && e.Status == "Active").FirstOrDefault();

      registrationGroup.ResponsibleEmployee = responsible;
      registrationGroup.RecipientLinks = null;      
      registrationGroup.Departments = null;

      var newRegistrationGroups = IntegrationServiceClient.CreateEntity<IRegistrationGroups>(registrationGroup);
      foreach (var recipientItem in recipients.Select(r => r.Member))
      {
        var recipient = IntegrationServiceClient.GetEntitiesWithFilter<IRecipients>(r => r.Name == recipientItem.Name && r.Status == "Active").FirstOrDefault();
        IntegrationServiceClient.Instance.For<IRegistrationGroups>().Key(newRegistrationGroups)
          .NavigateTo(x => x.RecipientLinks).Set(new { Member = recipient }).InsertEntryAsync().Wait();
      }

      foreach (var departmentItem in departments.Select(d => d.Department))
      {
        var department = IntegrationServiceClient.GetEntitiesWithFilter<IDepartments>(r => r.Name == departmentItem.Name && r.Status == "Active").FirstOrDefault();
        IntegrationServiceClient.Instance.For<IRegistrationGroups>().Key(newRegistrationGroups)
          .NavigateTo(x => x.Departments).Set(new { Department = department }).InsertEntryAsync().Wait();
      }
    }

    protected override IEnumerable<dynamic> Export()
    {
      return IntegrationServiceClient.Instance.For<IRegistrationGroups>()
          .Filter(c => c.Status == "Active")
          .Expand(c => c.ResponsibleEmployee)
          .Expand(c => c.RecipientLinks.Select(c => c.Member))
          .Expand(c => c.Departments.Select(c => c.Department))
          .FindEntriesAsync()
          .Result;
    }
  }
}
