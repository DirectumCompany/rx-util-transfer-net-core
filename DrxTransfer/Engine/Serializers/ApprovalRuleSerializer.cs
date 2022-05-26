using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DrxTransfer.IntegrationServicesClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simple.OData.Client;

namespace DrxTransfer
{
    class ApprovalRuleSerializer
    {
        #region Singletone Implementation

        private static ApprovalRuleSerializer instance;
        public static ApprovalRuleSerializer Instance
        {
            get
            {
                if (instance == null)
                    instance = new ApprovalRuleSerializer();
                return instance;
            }
        }

        #endregion

        public async Task ImportApprovalRuleAsync(IApprovalRules approvalRule, NLog.Logger logger)
        {
            var approvalRuleSettingsName = approvalRule.Name;

            var activeApprovalRule = BusinessLogic.GetEntitiesWithFilter<IApprovalRules>(r => r.Name == approvalRuleSettingsName && r.Status == "Active", logger);
            if (activeApprovalRule != null)
            {
                logger.Info(string.Format("Правило согласования {0} уже существует", approvalRuleSettingsName));
                return;
            }
            int id = (-1) * approvalRule.Id;
            var documentKinds = approvalRule.DocumentKinds;
            var departments = approvalRule.Departments;
            var stages = approvalRule.Stages;
            var businessUnits = approvalRule.BusinessUnits;
            var documentGroups = approvalRule.DocumentGroups;
            var conditions = approvalRule.Conditions;
            var transitions = approvalRule.Transitions;
            
            object result;

            var batch = new ODataBatch(BusinessLogic.InstanceOData()); // Создать новый batch-запрос.

            batch += async c => result = await c
              .For("IApprovalRules")
              .Set(new
              {
                  Id = id,
                  Name = approvalRule.Name,
                  NeedRestrictInitiatorRights = approvalRule.NeedRestrictInitiatorRights,
                  Note = approvalRule.Note,
                  ParentRule = approvalRule.ParentRule,
                  Priority = approvalRule.Priority,
                  ReworkApprovalRole = approvalRule.ReworkApprovalRole,
                  ReworkDeadline = approvalRule.ReworkDeadline,
                  ReworkPerformer = approvalRule.ReworkPerformer,
                  VersionNumber = approvalRule.VersionNumber,
                  Status = approvalRule.Status,
                  ReworkPerformerType = approvalRule.ReworkPerformerType,
                  DocumentFlow = approvalRule.DocumentFlow,
                  IsSmallApprovalAllowed = approvalRule.IsSmallApprovalAllowed,
                  IsDefaultRule = approvalRule.IsDefaultRule,
              })
              .InsertEntryAsync();

            var i = -1;

            foreach (var documentkind in documentKinds)
            {
                batch += async c => await c
                .For("IApprovalRules")
                .Key(id)
                .NavigateTo("DocumentKinds")
                .Set( new {
                    Id = i,
                    DocumentKind = documentkind.DocumentKind
                })
                .InsertEntryAsync();               
                i--;
            }

            foreach (var department in departments)
            {
                batch += async c => await c
                 .For("IApprovalRules")
                 .Key(id)
                 .NavigateTo("Departments")
                 .Set(new { id = i })
                 .InsertEntryAsync();
                batch += async c => await c
                .For("IApprovalRules")
                .Key(id)
                .NavigateTo("Departments")
                .Key(i)
                .NavigateTo("Department")
                .Set(new
                {
                    BusinessUnit = department.Department.BusinessUnit,
                    Code = department.Department.Code,
                    Description = department.Department.Description,
                    HeadOffice = department.Department.HeadOffice,
                    IsSystem = department.Department.IsSystem,
                    Manager = department.Department.Manager,
                    Name = department.Department.Name,
                    ShortName = department.Department.ShortName,
                    Status = department.Department.Status,
                    id = department.Department.Id,
                })
                .InsertEntryAsync();
                i--;
            }

            foreach (var stage in stages)
            {
                if (stage.Stage == null)
                    continue;
                batch += async c => await c
                 .For("IApprovalRules")
                 .Key(id)
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

            foreach (var businessUnit in businessUnits)
            {
                batch += async c => await c
                 .For("IApprovalRules")
                 .Key(id)  
                 .NavigateTo("BusinessUnits")
                 .Set(new { id = i, BusinessUnit = businessUnit.BusinessUnit })
                 .InsertEntryAsync();
                i--;
            }

            foreach (var documentGroup in documentGroups)
            {
                batch += async c => await c
                  .For("IApprovalRules")
                  .Key(id)
                  .NavigateTo("DocumentGroups")
                  .Set(new { id = i , DocumentGroup = documentGroup.DocumentGroup})
                  .InsertEntryAsync();
                i--;
            }

            foreach (var condition in conditions)
            {
                batch += async c => await c
                   .For("IApprovalRules")
                   .Key(id)
                   .NavigateTo("Conditions")
                   .Set(new { id = i, Number = condition.Number, Condition = condition.Condition })
                   .InsertEntryAsync();
                i--;
            }

            foreach (var transition in transitions)
            {
                batch += async c => await c
                   .For("IApprovalRules")
                   .Key(id) 
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

            try
            {
                await batch.ExecuteAsync();
                logger.Info(string.Format("Правило согласования \"{0}\" перенесено", approvalRuleSettingsName));
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
            }
        }

        public void Import(string filePath, NLog.Logger logger)
        {
            try
            {
                string jsonText = string.Empty;
                using (StreamReader sr = new StreamReader(filePath, System.Text.Encoding.Default))
                {
                    logger.Info("Чтение файла");
                    jsonText = sr.ReadToEnd();
                }
                var jsonBody = JsonConvert.DeserializeObject<IEnumerable<IApprovalRules>>(jsonText);

                var index = 1;
                var jsonItemsCount = jsonBody.Count();
                foreach (var jsonItem in jsonBody)
                {
                    try
                    {
                        logger.Info(string.Format("Запись {0} из {1}", index, jsonItemsCount));
                        index++;
                        _ = ImportApprovalRuleAsync(jsonItem, logger);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        logger.Error("Запись не создана");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void Export(string filePath, NLog.Logger logger)
        {
            try
            {
                var approvalRules = BusinessLogic.InstanceOData()
                  .For<IApprovalRules>()
                  .Filter(c => c.Status == "Active")
                  .Expand(c => new { c.ReworkPerformer, c.ReworkApprovalRole, c.ParentRule })
                  .Expand(c => c.DocumentKinds.Select(c => c.DocumentKind.DocumentType))
                  .Expand(c => c.BusinessUnits.Select(c => c.BusinessUnit))
                  .Expand(c => c.Departments.Select(c => c.Department.BusinessUnit))
                  .Expand(c => c.Stages.Select(c => c.Stage))
                  .Expand(c => c.DocumentGroups.Select(c => c.DocumentGroup))
                  .Expand(c => c.Transitions)
                  .Expand(c => c.Conditions.Select(c => c.Condition))
                  .FindEntriesAsync()
                  .Result;
                var approvalRulesCount = approvalRules.Count();
                logger.Info(string.Format("Найдено {0} объектов", approvalRulesCount));
                string JSONBody = JsonConvert.SerializeObject(approvalRules);

                using (StreamWriter sw = new StreamWriter(filePath, false, System.Text.Encoding.Default))
                {
                    logger.Info("Запись результата в файл");
                    sw.WriteLine(JSONBody);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
