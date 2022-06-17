using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using DrxTransfer;
using DrxTransfer.IntegrationServicesClient;
using Newtonsoft.Json.Linq;

namespace TransferSerializes
{
  [Export(typeof(SungeroSerializer))]
  class ContractCategorySerializer : SungeroSerializer
  {
    public ContractCategorySerializer() : base()
    {
      this.EntityName = "ContractCategory";
      this.EntityType = "IContractCategory";
    }

    public override void Import(object jsonObject)
    {
      var contractCategory = (jsonObject as JObject).ToObject<IContractCategory>();

      var contractCategoryName = contractCategory.Name;

      var activeContractCategory = IntegrationServiceClient.GetEntitiesWithFilter<IContractCategory>(r => r.Name == contractCategoryName && r.Status == "Active");
      if (activeContractCategory != null)
      {
        Logger.Info(string.Format("Категория договора {0} уже существует", contractCategoryName));
        return;
      }

      var documentKinds = contractCategory.DocumentKinds;
      contractCategory.DocumentKinds = null;

      var newContractCategory = IntegrationServiceClient.CreateEntity<IContractCategory>(contractCategory);

      foreach (var documentKindItem in documentKinds.Select(k => k.DocumentKind))
      {
        var documentKind = IntegrationServiceClient.GetEntitiesWithFilter<IDocumentKinds>(r => r.Name == documentKindItem.Name && r.DocumentFlow == documentKindItem.DocumentFlow && r.Status == "Active").FirstOrDefault();
        IntegrationServiceClient.Instance.For<IContractCategory>().Key(newContractCategory)
          .NavigateTo(x => x.DocumentKinds).Set(new { DocumentKind = documentKind }).InsertEntryAsync().Wait();
      }
    }

    protected override IEnumerable<dynamic> Export()
    {
      return IntegrationServiceClient.Instance
          .For<IContractCategory>()
          .Filter(c => c.Status == "Active")
          .Expand(c => c.DocumentKinds.Select(c => c.DocumentKind))
          .FindEntriesAsync()
          .Result;
    }
  }
}
