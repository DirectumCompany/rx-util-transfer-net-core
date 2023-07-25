using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DrxTransfer;
using DrxTransfer.IntegrationServicesClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simple.OData.Client;

namespace TransferSerializes
{
  [Export(typeof(SungeroSerializer))]
  class ApprovalRuleSerializer : SungeroSerializer
  {
    public ApprovalRuleSerializer() : base()
    {
      this.EntityName = "ApprovalRule";
      this.EntityType = "IApprovalRuleBase";
    }

    public override void Import(object jsonObject)
    {
      var rule = (jsonObject as JObject).ToObject<IApprovalRuleBases>();

      var ruleName = rule.Name;
      var documentFlow = rule.DocumentFlow;
      var activeApprovalRule = IntegrationServiceClient.GetEntitiesWithFilter<IApprovalRuleBases>(r => r.Name == rule.Name && r.DocumentFlow == documentFlow && r.Status == "Active");
      if (activeApprovalRule != null)
      {
        Logger.Info(string.Format("Правило согласования {0} уже существует", ruleName));
        return;
      }

      if (rule.DocumentFlow != "Contracts")
        rule = (jsonObject as JObject).ToObject<IApprovalRules>();

      ImportAsync(rule).Wait();
    }

    public async Task ImportAsync(IApprovalRuleBases rule)
    {
      var ruleName = rule.Name;
      var documentFlow = rule.DocumentFlow;

      var isContractsRule = documentFlow == "Contracts";
      string ruleODataType = isContractsRule ? "IContractsApprovalRules" : "IApprovalRules";
      string conditionOdataType = isContractsRule ? "IContractConditions" : "IConditions";

      #region Поиск НОР, подразделений, видов документов, категорий договоров.

      var documentKinds = new List<IDocumentKinds>();
      foreach (var documentKindItem in rule.DocumentKinds.Select(k => k.DocumentKind))
      {
        var documentKind = IntegrationServiceClient.GetEntitiesWithFilter<IDocumentKinds>(k => k.Name == documentKindItem.Name && k.DocumentFlow == documentKindItem.DocumentFlow && k.Status == "Active").FirstOrDefault();
        if (documentKind != null)
          documentKinds.Add(documentKind);
      }
      rule.DocumentKinds = null;

      var businessUnits = new List<IBusinessUnits>();
      foreach (var businessUnitItem in rule.BusinessUnits.Select(u => u.BusinessUnit))
      {
        var businessUnit = IntegrationServiceClient.GetEntitiesWithFilter<IBusinessUnits>(u => u.Name == businessUnitItem.Name && u.Status == "Active").FirstOrDefault();
        if (businessUnit != null)
          businessUnits.Add(businessUnit);
      }
      rule.BusinessUnits = null;

      var departments = new List<IDepartments>();
      foreach (var departmentItem in rule.Departments.Select(d => d.Department))
      {
        var department = IntegrationServiceClient.GetEntitiesWithFilter<IDepartments>(d => d.Name == departmentItem.Name && d.Status == "Active").FirstOrDefault();
        if (department != null)
          departments.Add(department);
      }
      rule.Departments = null;

      var contractCategories = new List<IDocumentGroupBases>();
      if (isContractsRule)
      {
        foreach (var documentGroupItem in rule.DocumentGroups.Select(g => g.DocumentGroup))
        {
          var contractCategory = IntegrationServiceClient.GetEntitiesWithFilter<IDocumentGroupBases>(g => g.Name == documentGroupItem.Name && g.Status == "Active").FirstOrDefault();
          if (contractCategory != null)
            contractCategories.Add(contractCategory);
        }
      }
      rule.DocumentGroups = null;

      #endregion

      #region Поиск и заполнение ответственных за доработку.

      if (rule.ReworkApprovalRole != null)
      {
        var reworkApprovalRole = IntegrationServiceClient.GetEntitiesWithFilter<IApprovalRoleBases>(r => r.Name == rule.ReworkApprovalRole.Name && r.Status == "Active").FirstOrDefault();
        rule.ReworkApprovalRole = reworkApprovalRole;
      }

      if (rule.ReworkPerformer != null)
      {
        var reworkPerformer = IntegrationServiceClient.GetEntitiesWithFilter<IRecipients>(r => r.Name == rule.ReworkPerformer.Name && r.Status == "Active").FirstOrDefault();
        rule.ReworkPerformer = reworkPerformer;
      }

      #endregion

      #region Cоздание условий.

      var conditionsArray = new List<ICollectionConditionBases>();

      if (isContractsRule)
      {
        foreach (var collectionConditionItem in rule.Conditions)
        {
          #region function.

          //var condition = collectionConditionItem.Condition;

          //if (condition.ApprovalRole != null)
          //{
          //  var approvalRole = IntegrationServiceClient.GetEntitiesWithFilter<IApprovalRoleBases>(r => r.Name == condition.ApprovalRole.Name && r.Status == "Active").FirstOrDefault();
          //  condition.ApprovalRole = approvalRole;
          //}

          //if (condition.RecipientForComparison != null)
          //{
          //  var recipientForComparison = IntegrationServiceClient.GetEntitiesWithFilter<IRecipients>(r => r.Name == condition.RecipientForComparison.Name && r.Status == "Active").FirstOrDefault();
          //  condition.RecipientForComparison = recipientForComparison;
          //}

          //if (condition.ApprovalRoleForComparison != null)
          //{
          //  var approvalRoleForComparison = IntegrationServiceClient.GetEntitiesWithFilter<IApprovalRoleBases>(r => r.Name == condition.ApprovalRoleForComparison.Name && r.Status == "Active").FirstOrDefault();
          //  condition.ApprovalRoleForComparison = approvalRoleForComparison;
          //}

          //if (condition.AddendaDocumentKind != null)
          //{
          //  var addendaDocumentKind = IntegrationServiceClient
          //    .GetEntitiesWithFilter<IDocumentKinds>(k => k.Name == condition.AddendaDocumentKind.Name && k.DocumentFlow == condition.AddendaDocumentKind.DocumentFlow && k.Status == "Active").FirstOrDefault();
          //  condition.AddendaDocumentKind = addendaDocumentKind;
          //}

          //var conditionDocumentKinds = new List<IDocumentKinds>();
          //foreach (var documentKindItem in condition.ConditionDocumentKinds.Select(k => k.DocumentKind))
          //{
          //  var documentKind = IntegrationServiceClient
          //    .GetEntitiesWithFilter<IDocumentKinds>(k => k.Name == documentKindItem.Name && k.DocumentFlow == documentKindItem.DocumentFlow && k.Status == "Active").FirstOrDefault();
          //  if (documentKind != null)
          //    conditionDocumentKinds.Add(documentKind);
          //}

          //var conditionCurrencies = new List<ICurrencies>();
          //foreach (var currencyItem in condition.Currencies.Select(c => c.Currency))
          //{
          //  var currency = IntegrationServiceClient.GetEntitiesWithFilter<ICurrencies>(c => c.Name == currencyItem.Name && c.Status == "Active").FirstOrDefault();
          //  if (currency != null)
          //    conditionCurrencies.Add(currency);
          //}

          //var conditionDeliveryMethods = new List<IMailDeliveryMethods>();
          //foreach (var deliveryMethodItem in condition.DeliveryMethods.Select(d => d.DeliveryMethod))
          //{
          //  var deliveryMethod = IntegrationServiceClient.GetEntitiesWithFilter<IMailDeliveryMethods>(d => d.Name == deliveryMethodItem.Name && d.Status == "Active").FirstOrDefault();
          //  if (deliveryMethod != null)
          //    conditionDeliveryMethods.Add(deliveryMethod);
          //}

          //condition.ConditionDocumentKinds = null;
          //condition.DocumentKinds = null;
          //condition.Currencies = null;
          //condition.DeliveryMethods = null;

          //var conditionName = condition.Name;
          //condition.Name = string.Empty;

          //var batchCondition = new ODataBatch(IntegrationServiceClient.Instance);
          //object resultCondition;
          //var conditionId = (-1) * condition.Id;
          //condition.Id = conditionId;

          //batchCondition += async c => resultCondition = await c
          //  .For(conditionOdataType)
          //  .Set(condition)
          //  .InsertEntryAsync();

          //var itemId = -1;

          //foreach (var conditionDocumentKind in conditionDocumentKinds)
          //{
          //  batchCondition += async c => await c
          //   .For(conditionOdataType)
          //   .Key(conditionId)
          //   .NavigateTo("ConditionDocumentKinds")
          //   .Set(new { id = itemId, DocumentKind = conditionDocumentKind })
          //   .InsertEntryAsync();
          //  itemId--;
          //}

          //foreach (var conditionCurrency in conditionCurrencies)
          //{
          //  batchCondition += async c => await c
          //   .For(conditionOdataType)
          //   .Key(conditionId)
          //   .NavigateTo("Currencies")
          //   .Set(new { id = itemId, Currency = conditionCurrency })
          //   .InsertEntryAsync();
          //  itemId--;
          //}

          //foreach (var conditionDeliveryMethod in conditionDeliveryMethods)
          //{
          //  batchCondition += async c => await c
          //   .For(conditionOdataType)
          //   .Key(conditionId)
          //   .NavigateTo("DeliveryMethods")
          //   .Set(new { id = itemId, DeliveryMethod = conditionDeliveryMethod })
          //   .InsertEntryAsync();
          //  itemId--;
          //}

          //batchCondition.ExecuteAsync().Wait();

          #endregion

          var condition = collectionConditionItem.Condition;
          var conditionName = condition.Name;
          condition.Name = string.Empty;

          CreateConditionAsync(collectionConditionItem.Condition, conditionOdataType).Wait();

          var newCondition = IntegrationServiceClient.GetEntitiesWithFilter<IConditionBases>(c => c.ConditionType == condition.ConditionType && c.Name == conditionName)
            .OrderByDescending(c => c.Id).FirstOrDefault();
          conditionsArray.Add(new ICollectionConditionBases { Number = collectionConditionItem.Number, Condition = newCondition });
        }
      }
      else
      {
        foreach (var collectionConditionItem in (rule as IApprovalRules).Conditions)
        {
          var condition = collectionConditionItem.Condition;
          var conditionName = condition.Name;
          condition.Name = string.Empty;
          CreateConditionAsync(collectionConditionItem.Condition, conditionOdataType).Wait();

          var newCondition = IntegrationServiceClient.GetEntitiesWithFilter<IConditionBases>(c => c.ConditionType == condition.ConditionType && c.Name == conditionName)
            .OrderByDescending(c => c.Id).FirstOrDefault();
          conditionsArray.Add(new ICollectionConditionBases { Number = collectionConditionItem.Number, Condition = newCondition });
        }
      }

      rule.Conditions = null;
      var approvalRule = rule as IApprovalRules;
      if (approvalRule != null)
        approvalRule.Conditions = null;

      #endregion

      #region Поиск и создание этапов.

      var stagesArray = new List<ICollectionStages>();
      foreach (var collectionStageItem in rule.Stages.Where(s => s.StageType != "Function"))
      {
        var stage = collectionStageItem.Stage;
        var activeStage = IntegrationServiceClient.GetEntitiesWithFilter<IApprovalStage>(s => s.Name == stage.Name);
        //TODO : GetEntitiesWithFilter должна возращать пустую коллекцию, а не null.
        if (activeStage != null)
        {
          stagesArray.Add(new ICollectionStages { Number = collectionStageItem.Number, StageType = collectionStageItem.StageType, Stage = activeStage.FirstOrDefault() });
        }
        else
        {
          if (stage.Assignee != null)
          {
            var assignee = IntegrationServiceClient.GetEntitiesWithFilter<IRecipients>(r => r.Name == stage.Assignee.Name && r.Status == "Active").FirstOrDefault();
            stage.Assignee = assignee;
          }

          if (stage.ApprovalRole != null)
          {
            var approvalRole = IntegrationServiceClient.GetEntitiesWithFilter<IApprovalRoleBases>(r => r.Name == stage.ApprovalRole.Name && r.Status == "Active").FirstOrDefault();
            stage.ApprovalRole = approvalRole;
          }

          if (stage.ReworkPerformer != null)
          {
            var reworkPerformer = IntegrationServiceClient.GetEntitiesWithFilter<IRecipients>(r => r.Name == stage.ReworkPerformer.Name && r.Status == "Active").FirstOrDefault();
            stage.ReworkPerformer = reworkPerformer;
          }

          if (stage.ReworkApprovalRole != null)
          {
            var reworkApprovalRole = IntegrationServiceClient.GetEntitiesWithFilter<IApprovalRoleBases>(r => r.Name == stage.ReworkApprovalRole.Name && r.Status == "Active").FirstOrDefault();
            stage.ReworkApprovalRole = reworkApprovalRole;
          }

          var approvalRoles = new List<IApprovalRoleBases>();
          foreach (var approvalRoleItem in stage.ApprovalRoles.Select(r => r.ApprovalRole))
          {
            var approvalRole = IntegrationServiceClient.GetEntitiesWithFilter<IApprovalRoleBases>(r => r.Name == approvalRoleItem.Name && r.Status == "Active").FirstOrDefault();
            if (approvalRole != null)
              approvalRoles.Add(approvalRole);
          }
          stage.ApprovalRoles = null;

          var recipients = new List<IRecipients>();
          foreach (var recipientItem in stage.Recipients.Select(r => r.Recipient))
          {
            var recipient = IntegrationServiceClient.GetEntitiesWithFilter<IRecipients>(r => r.Name == recipientItem.Name && r.Status == "Active").FirstOrDefault();
            if (recipient != null)
              recipients.Add(recipient);
          }
          stage.Recipients = null;

          if (stage.StageType == "Notice" && stage.DeadlineInDays == null)
            stage.DeadlineInDays = 1;

          var batchStage = new ODataBatch(IntegrationServiceClient.Instance);
          object resultStage;
          var stageId = (-1) * stage.Id;
          stage.Id = stageId;

          batchStage += async c => resultStage = await c
            .For("IApprovalStage")
            .Set(stage)
            .InsertEntryAsync();

          var itemId = -1;

          foreach (var approvalRole in approvalRoles)
          {
            batchStage += async c => await c
             .For("IApprovalStage")
             .Key(stageId)
             .NavigateTo("ApprovalRoles")
             .Set(new { id = itemId, ApprovalRole = approvalRole })
             .InsertEntryAsync();
            itemId--;
          }

          foreach (var recipient in recipients)
          {
            batchStage += async c => await c
             .For("IApprovalStage")
             .Key(stageId)
             .NavigateTo("Recipients")
             .Set(new { id = itemId, Recipient = recipient })
             .InsertEntryAsync();
            itemId--;
          }

          batchStage.ExecuteAsync().Wait();

          var newStage = IntegrationServiceClient.GetEntitiesWithFilter<IApprovalStage>(s => s.Name == stage.Name).FirstOrDefault();
          stagesArray.Add(new ICollectionStages { Number = collectionStageItem.Number, StageType = collectionStageItem.StageType, Stage = newStage });
        }
      }

      foreach (var collectionStageItem in rule.Stages.Where(s => s.StageType == "Function"))
      {
        var stage = collectionStageItem.StageBase;
        var activeStage = IntegrationServiceClient.GetEntitiesWithFilter<IApprovalStageBase>(s => s.Name == stage.Name);
        if (activeStage != null)
        {
          stagesArray.Add(new ICollectionStages { Number = collectionStageItem.Number, StageType = collectionStageItem.StageType, StageBase = activeStage.FirstOrDefault() });
        }
      }
      rule.Stages = null;

      #endregion

      #region Пакетный запрос на создание правила.

      var transitionsArray = rule.Transitions;
      rule.Transitions = null;

      var batch = new ODataBatch(IntegrationServiceClient.Instance);
      object result;
      var ruleId = (-1) * rule.Id;
      rule.Id = ruleId;

      batch += async c => result = await c
        .For(ruleODataType)
        .Set(rule)
        .InsertEntryAsync();

      var i = -1;

      foreach (var businessUnit in businessUnits)
      {
        batch += async c => await c
         .For(ruleODataType)
         .Key(ruleId)
         .NavigateTo("BusinessUnits")
         .Set(new { id = i, BusinessUnit = businessUnit })
         .InsertEntryAsync();
        i--;
      }

      foreach (var department in departments)
      {
        batch += async c => await c
         .For(ruleODataType)
         .Key(ruleId)
         .NavigateTo("Departments")
         .Set(new { id = i, Department = department })
         .InsertEntryAsync();
        i--;
      }

      foreach (var documentKind in documentKinds)
      {
        batch += async c => await c
         .For(ruleODataType)
         .Key(ruleId)
         .NavigateTo("DocumentKinds")
         .Set(new { id = i, DocumentKind = documentKind })
         .InsertEntryAsync();
        i--;
      }

      foreach (var contractCategory in contractCategories)
      {
        batch += async c => await c
         .For(ruleODataType)
         .Key(ruleId)
         .NavigateTo("DocumentGroups")
         .Set(new { id = i, DocumentGroup = contractCategory })
         .InsertEntryAsync();
        i--;
      }

      foreach (var condition in conditionsArray)
      {
        if (condition.Condition == null)
          continue;

        batch += async c => await c
         .For(ruleODataType)
         .Key(ruleId)
         .NavigateTo("Conditions")
         .Set(new
         {
           Id = i,
           Number = condition.Number,
           Condition = condition.Condition
         })
         .InsertEntryAsync();

        i--;
      }

      foreach (var stage in stagesArray)
      {
        if (stage.Stage == null && stage.StageBase == null)
          continue;

        batch += async c => await c
         .For(ruleODataType)
         .Key(ruleId)
         .NavigateTo("Stages")
         .Set(new
         {
           id = i,
           Number = stage.Number,
           Stage = stage.Stage,
           StageType = stage.StageType,
           StageBase = stage.StageBase,
         })
         .InsertEntryAsync();
        i--;
      }

      foreach (var transition in transitionsArray)
      {

        batch += async c => await c
           .For(ruleODataType)
           .Key(ruleId)
           .NavigateTo("Transitions")
           .Set(new
           {
             id = i,
             SourceStage = transition.SourceStage,
             TargetStage = transition.TargetStage,
             ConditionValue = transition.ConditionValue
           })
           .InsertEntryAsync();
        i--;
      }

      await batch.ExecuteAsync();

      #endregion
    }

    public async Task CreateConditionAsync(IConditionBases condition, string conditionOdataType)
    {
      if (condition.ApprovalRole != null)
      {
        var approvalRole = IntegrationServiceClient.GetEntitiesWithFilter<IApprovalRoleBases>(r => r.Name == condition.ApprovalRole.Name && r.Status == "Active").FirstOrDefault();
        condition.ApprovalRole = approvalRole;
      }

      if (condition.RecipientForComparison != null)
      {
        var recipientForComparison = IntegrationServiceClient.GetEntitiesWithFilter<IRecipients>(r => r.Name == condition.RecipientForComparison.Name && r.Status == "Active").FirstOrDefault();
        condition.RecipientForComparison = recipientForComparison;
      }

      if (condition.ApprovalRoleForComparison != null)
      {
        var approvalRoleForComparison = IntegrationServiceClient.GetEntitiesWithFilter<IApprovalRoleBases>(r => r.Name == condition.ApprovalRoleForComparison.Name && r.Status == "Active").FirstOrDefault();
        condition.ApprovalRoleForComparison = approvalRoleForComparison;
      }

      if (condition.AddendaDocumentKind != null)
      {
        var addendaDocumentKind = IntegrationServiceClient
          .GetEntitiesWithFilter<IDocumentKinds>(k => k.Name == condition.AddendaDocumentKind.Name && k.DocumentFlow == condition.AddendaDocumentKind.DocumentFlow && k.Status == "Active").FirstOrDefault();
        condition.AddendaDocumentKind = addendaDocumentKind;
      }

      var conditionDocumentKinds = new List<IDocumentKinds>();
      foreach (var documentKindItem in condition.ConditionDocumentKinds.Select(k => k.DocumentKind))
      {
        var documentKind = IntegrationServiceClient
          .GetEntitiesWithFilter<IDocumentKinds>(k => k.Name == documentKindItem.Name && k.DocumentFlow == documentKindItem.DocumentFlow && k.Status == "Active").FirstOrDefault();
        if (documentKind != null)
          conditionDocumentKinds.Add(documentKind);
      }

      var conditionCurrencies = new List<ICurrencies>();
      foreach (var currencyItem in condition.Currencies.Select(c => c.Currency))
      {
        var currency = IntegrationServiceClient.GetEntitiesWithFilter<ICurrencies>(c => c.Name == currencyItem.Name && c.Status == "Active").FirstOrDefault();
        if (currency != null)
          conditionCurrencies.Add(currency);
      }

      var conditionDeliveryMethods = new List<IMailDeliveryMethods>();
      foreach (var deliveryMethodItem in condition.DeliveryMethods.Select(d => d.DeliveryMethod))
      {
        var deliveryMethod = IntegrationServiceClient.GetEntitiesWithFilter<IMailDeliveryMethods>(d => d.Name == deliveryMethodItem.Name && d.Status == "Active").FirstOrDefault();
        if (deliveryMethod != null)
          conditionDeliveryMethods.Add(deliveryMethod);
      }

      var conditionAddressees = new List<IEmployees>();
      var conditionApproval = condition as IConditions;
      if (conditionApproval != null && conditionApproval.Addressees != null)
      {
        foreach (var addresseeItem in conditionApproval.Addressees.Select(a => a.Addressee))
        {
          var addressee = IntegrationServiceClient.GetEntitiesWithFilter<IEmployees>(e => e.Name == addresseeItem.Name && e.Status == "Active").FirstOrDefault();
          if (addressee != null)
            conditionAddressees.Add(addressee);
        }
        conditionApproval.Addressees = null;
      }

      condition.ConditionDocumentKinds = null;
      condition.DocumentKinds = null;
      condition.Currencies = null;
      condition.DeliveryMethods = null;

      var batchCondition = new ODataBatch(IntegrationServiceClient.Instance);
      object resultCondition;
      var conditionId = (-1) * condition.Id;
      condition.Id = conditionId;

      batchCondition += async c => resultCondition = await c
        .For(conditionOdataType)
        .Set(condition)
        .InsertEntryAsync();

      var itemId = -1;

      foreach (var conditionDocumentKind in conditionDocumentKinds)
      {
        batchCondition += async c => await c
         .For(conditionOdataType)
         .Key(conditionId)
         .NavigateTo("ConditionDocumentKinds")
         .Set(new { id = itemId, DocumentKind = conditionDocumentKind })
         .InsertEntryAsync();
        itemId--;
      }

      foreach (var conditionCurrency in conditionCurrencies)
      {
        batchCondition += async c => await c
         .For(conditionOdataType)
         .Key(conditionId)
         .NavigateTo("Currencies")
         .Set(new { id = itemId, Currency = conditionCurrency })
         .InsertEntryAsync();
        itemId--;
      }

      foreach (var conditionDeliveryMethod in conditionDeliveryMethods)
      {
        batchCondition += async c => await c
         .For(conditionOdataType)
         .Key(conditionId)
         .NavigateTo("DeliveryMethods")
         .Set(new { id = itemId, DeliveryMethod = conditionDeliveryMethod })
         .InsertEntryAsync();
        itemId--;
      }

      if (conditionAddressees.Any())
      {
        foreach (var conditionAddressee in conditionAddressees)
        {
          batchCondition += async c => await c
           .For(conditionOdataType)
           .Key(conditionId)
           .NavigateTo("Addressees")
           .Set(new { id = itemId, Addressee = conditionAddressee })
           .InsertEntryAsync();
          itemId--;
        }
      }

      await batchCondition.ExecuteAsync();
    }

    protected override IEnumerable<dynamic> Export()
    {
      return GetRequestAsync().Result;
    }

    public async Task<IEnumerable<object>> GetRequestAsync()
    {
      var request = await IntegrationServiceClient.Instance.For<IApprovalRuleBases>()
            .Filter(c => c.Status == "Active")
            .Expand(c => new { c.ReworkPerformer, c.ReworkApprovalRole, c.ParentRule })
            .Expand(c => c.DocumentKinds.Select(c => c.DocumentKind.DocumentType))
            .Expand(c => c.BusinessUnits.Select(c => c.BusinessUnit))
            .Expand(c => c.Departments.Select(c => c.Department.BusinessUnit))
            .Expand(c => c.DocumentGroups.Select(c => c.DocumentGroup))
            .Expand(c => c.Stages.Select(c =>
            new
            {
              c.Stage.Assignee,
              c.Stage.ApprovalRole,
              ApprovalRoles = c.Stage.ApprovalRoles.Select(r => r.ApprovalRole),
              Recipients = c.Stage.Recipients.Select(r => r.Recipient),
              c.Stage.ReworkPerformer,
              c.Stage.ReworkApprovalRole
            }))
            .Expand(c => c.Stages.Select(c => c.StageBase))
            .Expand(c => c.DocumentGroups.Select(c => c.DocumentGroup))
            .Expand(c => c.Transitions)
            .Expand(c => c.Conditions.Select(c =>
            new
            {
              c.Condition.RecipientForComparison,
              c.Condition.ApprovalRole,
              c.Condition.ApprovalRoleForComparison,
              c.Condition.AddendaDocumentKind,
              c.Condition.ConditionDocumentKinds,
              DocumentKind = c.Condition.ConditionDocumentKinds.Select(k => k.DocumentKind),
              c.Condition.Currencies,
              Currency = c.Condition.Currencies.Select(c => c.Currency),
              c.Condition.DeliveryMethods,
              DeliveryMethod = c.Condition.DeliveryMethods.Select(m => m.DeliveryMethod),
              c.Condition.DocumentKinds
            }))
            .GetCommandTextAsync();

      request = request.Replace(",Condition($expand=DocumentKinds)", ",Condition($expand=DocumentKinds),Condition($expand=Sungero.IntegrationService.Models.Generated.Docflow.IConditionDto/Addressees($expand=Addressee))");
      request = request.Replace("($select=ApprovalRole)", ",ApprovalRoles($expand = ApprovalRole)");

      return (JsonConvert.DeserializeObject(CollectionHelper.Instance.GetStringAsync(request).Result) as JObject).Property("value").Value.ToObject<IEnumerable<object>>();
    }
  }
}
