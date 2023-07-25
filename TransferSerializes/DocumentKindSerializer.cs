using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using DrxTransfer;
using DrxTransfer.IntegrationServicesClient;
using Newtonsoft.Json.Linq;

namespace TransferSerializes
{
  [Export(typeof(SungeroSerializer))]
  class DocumentKindSerializer : SungeroSerializer
  {
    public DocumentKindSerializer() : base()
    {
      this.EntityName = "DocumentKind";
      this.EntityType = "IDocumentKind";
    }

    public override void Import(object jsonObject)
    {
      var documentKind = (jsonObject as JObject).ToObject<IDocumentKinds>();

      var documentKindName = documentKind.Name;
      var activeDocumentKind = IntegrationServiceClient.GetEntitiesWithFilter<IDocumentKinds>(r => r.Name == documentKindName && r.Status == "Active");
      if (activeDocumentKind != null)
      {
        Logger.Info(string.Format("Вид документа {0} уже существует", documentKindName));
        return;
      }

      var documentType = IntegrationServiceClient
        .GetEntitiesWithFilter<IDocumentType>(t => t.Name == documentKind.DocumentType.Name && t.DocumentFlow == documentKind.DocumentType.DocumentFlow && t.Status == "Active")
        .FirstOrDefault();
      documentKind.DocumentType = documentType;

      var availableActions = documentKind.AvailableActions;
      documentKind.AvailableActions = null;
      var newDocumentKind = IntegrationServiceClient.CreateEntity<IDocumentKinds>(documentKind);

      CollectionHelper.CellectionItemsClear("IDocumentKinds", newDocumentKind.Id.ToString(), "AvailableActions");

      foreach (var availableActionItem in availableActions.Select(a => a.Action))
      {
        var availableAction = IntegrationServiceClient.GetEntitiesWithFilter<IDocumentSendAction>(a => a.Name == availableActionItem.Name && a.Status == "Active").FirstOrDefault();
        IntegrationServiceClient.Instance.For<IDocumentKinds>().Key(newDocumentKind)
          .NavigateTo(x => x.AvailableActions).Set(new { Action = availableAction }).InsertEntryAsync().Wait();
      }
    }

    protected override IEnumerable<dynamic> Export()
    {
      return IntegrationServiceClient.Instance.For<IDocumentKinds>()
          .Filter(c => c.Status == "Active")
          .Expand(c => c.DocumentType)
          .Expand(c => c.AvailableActions.Select(c => c.Action))
          .FindEntriesAsync()
          .Result;
    }
  }
}
