using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace aeon.Integration1C.Server
{
  public class ModuleJobs
  {

    /// <summary>
    /// Отправка финансовых документов на согласование по регламенту.
    /// </summary>
    public virtual void SendingFinancialDocumentsForApproval()
    {
      var documents = AEOHSolution.AccountingDocumentBases.GetAll(d => d.Author.IsSystem.GetValueOrDefault() && d.LifeCycleState == AEOHSolution.AccountingDocumentBase.LifeCycleState.Draft &&
                                                                  !d.InternalApprovalState.HasValue && d.NumberOfDocumentsInPackage.HasValue && d.HasRelations &&
                                                                  (Sungero.FinancialArchive.ContractStatements.Is(d) || Sungero.FinancialArchive.UniversalTransferDocuments.Is(d)));
      
      var integrationRole = Functions.Module.GetResponsibleForIntegration1CRole();
      var subjectSimpleTask = aeon.Integration1C.Resources.TaskSubjectForManualSendingDocument;
      var simpleTasks = Sungero.Workflow.SimpleTasks.GetAll(t => t.Author.IsSystem.GetValueOrDefault() && t.Status == Sungero.Workflow.SimpleTask.Status.InProcess && t.Subject == subjectSimpleTask);
      foreach (var document in documents)
      {
        var relations = document.Relations.GetRelated(Sungero.Docflow.PublicConstants.Module.AddendumRelationName);
        if ((relations.Count() + 1) == document.NumberOfDocumentsInPackage)
        {
          var signatorySetting = Functions.Module.GetSignatureSettings(document.BusinessUnit, document.DocumentKind);
          if (signatorySetting != null)
          {
            var approvalTask = Sungero.Docflow.ApprovalTasks.Create();
            approvalTask.DocumentGroup.OfficialDocuments.Add(document);
            approvalTask.Author = document.ResponsibleEmployee;
            approvalTask.Signatory = Sungero.Company.Employees.As(signatorySetting.Recipient);
            approvalTask.Start();
          }
          else
          {
            if (simpleTasks.Any(t => t.AttachmentDetails.Any(a => a.EntityId == document.Id)))
              continue;
            
            var simpleTask = Sungero.Workflow.SimpleTasks.Create(subjectSimpleTask, integrationRole);
            simpleTask.Attachments.Add(document);
            simpleTask.NeedsReview = false;
            simpleTask.Start();
          }
        }
      }
    }

    /// <summary>
    /// Получить неформализованные документы из 1С.
    /// </summary>
    public virtual void GetNonFormalizedDocsIn1C()
    {
      var setting = Functions.SettingsIntegration.GetSettingsIntegration();
      var queueName = setting.QueueGetForNonFormalizedDocs;
      
      var messages = Functions.Module.GetMessagesFromRabbitMQ(queueName);
      foreach (var message in messages)
      {
        Logger.Debug(aeon.Integration1C.Resources.LoggingMessageFromRabbitMQFormat(message));
        
        var async = AsyncHandlers.UpdateNonFormalizedDoc.Create();
        async.Message = message;
        async.ExecuteAsync();
      }
    }

    /// <summary>
    /// Получить формализованные документы из 1С.
    /// </summary>
    public virtual void GetFormalizedDocsIn1C()
    {
      var setting = Functions.SettingsIntegration.GetSettingsIntegration();
      if (!setting.IntragroupDocuments.Any() || !setting.ExternalWithResidentDocuments.Any() || !setting.ExternalWithNonResidentDocuments.Any())
        return;
      
      var queueName = setting.QueueGetForFormalizedDocs;
      
      var messages = Functions.Module.GetMessagesFromRabbitMQ(queueName);
      foreach (var message in messages)
      {
        Logger.Debug(aeon.Integration1C.Resources.LoggingMessageFromRabbitMQFormat(message));
        
        var async = AsyncHandlers.UpdateFormalizedDoc.Create();
        async.Message = message;
        async.ExecuteAsync();
      }
    }

    /// <summary>
    /// Получить организации из 1С.
    /// </summary>
    public virtual void GetCompanyIn1C()
    {
      var setting = Functions.SettingsIntegration.GetSettingsIntegration();
      var queueName = setting.QueueGetForCounterparties;
      var routingKeyVerification = setting.RoutingKeySendVerificationHM;
      var routingKeyError = setting.RoutingKeyErrorsHM;
      var exchangePoint = setting.ExchangePointHM;
      var queueErrors = setting.QueueErrorsHM;
        
      var messages = Functions.Module.GetMessagesFromRabbitMQ(queueName);
      foreach (var message in messages)
      {
        Logger.Debug(aeon.Integration1C.Resources.LoggingMessageFromRabbitMQFormat(message));
        
        var async = AsyncHandlers.UpdateCompany.Create();
        async.Message = message;
        async.RoutingKeyVerification = routingKeyVerification;
        async.RoutingKeyError = routingKeyError;
        async.ExchangePoint = exchangePoint;
        async.QueueErrors = queueErrors;
        async.ExecuteAsync();
      }
    }

    /// <summary>
    /// Отправить Договорные документы в 1С.
    /// </summary>
    public virtual void SendContractualDocsIn1C()
    {
      var async = AsyncHandlers.SendContractualDocsIn1CAsync.Create();
      async.ExecuteAsync();
    }

    /// <summary>
    /// Получить результаты интеграции из 1С.
    /// </summary>
    public virtual void GetResultsIn1C()
    {
      var async = AsyncHandlers.GetIntegrationsResultsAsync.Create();
      async.ExecuteAsync();
    }

    /// <summary>
    /// Получить договорные документы из 1С.
    /// </summary>
    public virtual void GetContractualDocsIn1C()
    {
      var setting = Functions.SettingsIntegration.GetSettingsIntegration();
      
      var queueName = setting.QueueGetForContracts;
      var errorQueue = setting.QueueErrorsHM;
      var routingKeyVerification = setting.RoutingKeySendVerificationHM;
      var routingKeyError = setting.RoutingKeyErrorsHM;
      var exchangePoint = setting.ExchangePointHM;

      var messages = Functions.Module.GetMessagesFromRabbitMQ(queueName);
      foreach (var message in messages)
      {
        Logger.Debug(aeon.Integration1C.Resources.LoggingMessageFromRabbitMQFormat(message));
        
        var async = AsyncHandlers.UpdateContractualDoc.Create();
        async.Message = message;
        async.RoutingKeyVerification = routingKeyVerification;
        async.RoutingKeyError = routingKeyError;
        async.IdContractualDocResponsible = setting.ContractResponsible.Id;
        async.ExchangePoint = exchangePoint;
        async.QueueErrors = errorQueue;
        async.ExecuteAsync();
      }
    }

    /// <summary>
    /// Отправить Организации в 1С.
    /// </summary>
    public virtual void SendCompanyIn1C()
    {
      var async = AsyncHandlers.SendCompanyIn1CAsync.Create();
      async.ExecuteAsync();
    }

  }
}