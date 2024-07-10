using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DrxTransfer;
using DrxTransfer.IntegrationServicesClient;
using Newtonsoft.Json.Linq;
using Simple.OData.Client;

namespace TransferSerializes
{
  [Export(typeof(SungeroSerializer))]
  class DocumentRegisterSerializer : SungeroSerializer
  {
    public DocumentRegisterSerializer() : base()
    {
      this.EntityName = "DocumentRegister";
      this.EntityType = "IDocumentRegister";
    }

    public override void Import(object jsonObject)
    {
      var documentRegister = (jsonObject as JObject).ToObject<IDocumentRegisters>();

      var documentRegistersName = documentRegister.Name;
      var activeDocumentRegisters = IntegrationServiceClient.GetEntitiesWithFilter<IDocumentRegisters>(r => r.Name == documentRegistersName && r.Status == "Active");
      if (activeDocumentRegisters != null)
      {
        Logger.Info(string.Format("Журнал регистрации {0} уже существует", documentRegistersName));
        return;
      }

      if (documentRegister.RegistrationGroup != null)
      {
        var registrationGroup = IntegrationServiceClient
          .GetEntitiesWithFilter<IRegistrationGroups>(r => r.Name == documentRegister.RegistrationGroup.Name && r.Index == documentRegister.RegistrationGroup.Index && r.Status == "Active")
          .FirstOrDefault();
        documentRegister.RegistrationGroup = registrationGroup;
      }

      var numberFormatItems = documentRegister.NumberFormatItems;
      documentRegister.NumberFormatItems = null;
      var newRegistrationGroups = IntegrationServiceClient.CreateEntity<IDocumentRegisters>(documentRegister);

      newRegistrationGroups = IntegrationServiceClient.Instance.For<IDocumentRegisters>()
       .Key(newRegistrationGroups)
       .Expand(x => x.NumberFormatItems)
       .FindEntryAsync().Result;

      var newBegginingOfLineItem = newRegistrationGroups.NumberFormatItems.Where(i => i.Element == "BegginingOfLine").FirstOrDefault();
      if (newBegginingOfLineItem != null)
      {
        CollectionHelper.DeleteCellectionItem("IDocumentRegisters",
        newRegistrationGroups.Id.ToString(),
        "NumberFormatItems",
        newBegginingOfLineItem.Id.ToString());
      }

      var newNumberItem = newRegistrationGroups.NumberFormatItems.Where(i => i.Element == "Number").FirstOrDefault();
      if (newNumberItem != null)
      {
        var numberItem = numberFormatItems.Where(i => i.Element == "Number").FirstOrDefault();
        if (numberItem != null)
        {
          newNumberItem.Number = numberItem.Number;
          newNumberItem.Separator = numberItem.Separator;

          CollectionHelper.UpdateCellectionItem("IDocumentRegisters",
            newRegistrationGroups.Id.ToString(),
            "NumberFormatItems",
            newNumberItem.Id.ToString(),
            newNumberItem);
        }
      }

      foreach (var item in numberFormatItems.Where(i => i.Element != "Number"))
      {
        IntegrationServiceClient.Instance.For<IDocumentRegisters>().Key(newRegistrationGroups).
          NavigateTo(x => x.NumberFormatItems).Set(new { Element = item.Element, Number = item.Number, Separator = item.Separator }).InsertEntryAsync().Wait();
      }
    }

    protected override IEnumerable<dynamic> Export()
    {
      return IntegrationServiceClient.Instance.For<IDocumentRegisters>()
          .Filter(c => c.Status == "Active")
          .Expand(c => c.RegistrationGroup)
          .Expand(c => c.NumberFormatItems)
          .FindEntriesAsync()
          .Result;
    }
  }
}
