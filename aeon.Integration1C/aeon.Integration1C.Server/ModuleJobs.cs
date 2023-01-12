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
    /// Получить организации из 1С.
    /// </summary>
    public virtual void GetCompanyIn1C()
    {
      var setting = Functions.SettingsIntegration.GetSettingsIntegration();
      var queueName = setting.QueueGetForCounterparties;
      var routingKeyVerification = setting.RoutingKeySendVerification;
      var routingKeyError = setting.RoutingKeyErrors;
      
      var messages = Functions.Module.GetMessagesFromRabbitMQ(queueName);
      foreach (var message in messages)
      {
        Logger.Debug(aeon.Integration1C.Resources.LoggingMessageFromRabbitMQFormat(message));
        
        var async = AsyncHandlers.UpdateCompany.Create();
        async.Message = message;
        async.RoutingKeyVerification = routingKeyVerification;
        async.RoutingKeyError = routingKeyError;
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
      var errorQueue = setting.QueueErrors;
      var routingKeyVerification = setting.RoutingKeySendVerification;
      var routingKeyError = setting.RoutingKeyErrors;
      
      var messages = Functions.Module.GetMessagesFromRabbitMQ(queueName);
      var contractualDocs = AEOHSolution.ContractualDocuments.GetAll();
      foreach (var message in messages)
      {
        Logger.Debug(aeon.Integration1C.Resources.LoggingMessageFromRabbitMQFormat(message));
        
        var async = AsyncHandlers.UpdateContractualDoc.Create();
        async.Message = message;
        async.RoutingKeyVerification = routingKeyVerification;
        async.RoutingKeyError = routingKeyError;
        async.IdContractualDocResponsible = setting.ContractResponsible.Id;
        
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