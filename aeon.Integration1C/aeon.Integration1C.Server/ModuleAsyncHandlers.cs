using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace aeon.Integration1C.Server
{
  public class ModuleAsyncHandlers
  {

    public virtual void UpdateCompany(aeon.Integration1C.Server.AsyncHandlerInvokeArgs.UpdateCompanyInvokeArgs args)
    {
      var companies = AEOHSolution.Companies.GetAll();
      var result = Functions.Module.DeserializeMessageFromCompany(args.Message);
      var resultBody = result.body;
      
      var oldCompany = aeon.AEOHSolution.Companies.Null;
      if (!string.IsNullOrEmpty(resultBody.Id))
      {
        oldCompany = companies.Where(c => c.Id.ToString() == resultBody.Id).FirstOrDefault();
      }
      else
      {
        if (resultBody.Type == aeon.Integration1C.Resources.NaturalPersonCounterpartyKind
            || resultBody.Type == aeon.Integration1C.Resources.SelfEmployedCounterpartyKind)
        {
          if (!string.IsNullOrEmpty(resultBody.TIN))
            oldCompany = companies.Where(c => c.TIN == resultBody.TIN).FirstOrDefault();
          else
            oldCompany = companies.Where(c => c.Name == resultBody.Name).FirstOrDefault();
        }
        else if (!string.IsNullOrEmpty(resultBody.Guid))
        {
          oldCompany = companies.Where(c => c.Guid1C == resultBody.Guid).FirstOrDefault();
          if (oldCompany == null)
          {
            if (resultBody.Nonresident == false)
              oldCompany = companies.Where(c => c.TIN == resultBody.TIN && c.TRRC == resultBody.TRRC).FirstOrDefault();
            else
              oldCompany = companies.Where(c => c.RegNumber == resultBody.RegNumber).FirstOrDefault();
          }
        }
      }
      var company = oldCompany != null ? oldCompany : aeon.AEOHSolution.Companies.Create();
      try
      {
        
        #region Обновление свойств Организации.
        
        company.Name = company.Name != resultBody.Name ? resultBody.Name : company.Name;
        company.LegalName = company.LegalName != resultBody.LegalName ? resultBody.LegalName : company.LegalName;
        company.TIN = company.TIN != resultBody.TIN ? resultBody.TIN : company.TIN;
        company.Nonresident = company.Nonresident != resultBody.Nonresident ? resultBody.Nonresident : company.Nonresident;
        company.RegNumber = company.RegNumber != resultBody.RegNumber ? resultBody.RegNumber : company.RegNumber;
        company.LegalAddress = company.LegalAddress != resultBody.LegalAddress ? resultBody.LegalAddress : company.LegalAddress;
        company.PostalAddress = company.PostalAddress != resultBody.PostalAddress ? resultBody.PostalAddress : company.PostalAddress;
        company.Phones = company.Phones != resultBody.Phones ? resultBody.Phones : company.Phones;
        company.Email = company.Email != resultBody.Email ? resultBody.Email : company.Email;
        company.Note = company.Note != resultBody.Note ? resultBody.Note : company.Note;
        company.PSRN = company.PSRN != resultBody.PSRN ? resultBody.PSRN : company.PSRN;
        company.NCEO = company.NCEO != resultBody.NCEO ? resultBody.NCEO : company.NCEO;
        company.NCEA = company.NCEA != resultBody.NCEA ? resultBody.NCEA : company.NCEA;
        company.Account = company.Account != resultBody.Account ? resultBody.Account : company.Account;
        company.Code = company.Code != resultBody.Code ? resultBody.Code : company.Code;
        company.TRRC = company.TRRC != resultBody.TRRC ? resultBody.TRRC : company.TRRC;
        
        var city = Functions.Module.GetCityFromName(resultBody.City);
        company.City = (city != null && !Equals(city, company.City)) ? city : company.City;
        
        var region = Functions.Module.GetRegionFromName(resultBody.Region);
        company.Region = (region != null && !Equals(region, company.Region)) ? region : company.Region;
        
        var bank = Functions.Module.GetBankFromBIC(resultBody.Bank);
        company.Bank = (bank != null && !Equals(bank, company.Bank)) ? bank : company.Bank;
        
        company.Guid1C = company.Guid1C != resultBody.Guid ? resultBody.Guid : company.Guid1C;
        
        var kind = Functions.Module.GetCounterpartyKindFromName(resultBody.Type);
        company.CounterpartyKind = (kind != null && !Equals(kind, company.CounterpartyKind)) ? kind : company.CounterpartyKind;
        
        var country = Functions.Module.GetCountryFromCode(resultBody.CountryCode, resultBody.RegCountry);
        company.CountryRegistration = (country != null && !Equals(country, company.CountryRegistration)) ? country : company.CountryRegistration;
        company.Status = resultBody.Label == false ? AEOHSolution.Company.Status.Closed : AEOHSolution.Company.Status.Active;
        
        #endregion
        
        company.Save();
        var messageResult = Functions.Module.SerializeMessageFromResult(company.Id, company.Guid1C, result.correlation_id);
        Functions.Module.SendMessageFromRabbitMQ(messageResult, args.RoutingKeyVerification);
      }
      catch (Exception ex)
      {
        Logger.Error(ex.Message, ex);
        var messageError = Functions.Module.SerializeMessageFromError(ex.Message, result.correlation_id);
        Functions.Module.SendMessageFromRabbitMQ(messageError, args.RoutingKeyError);
      }
    }

    public virtual void UpdateContractualDoc(aeon.Integration1C.Server.AsyncHandlerInvokeArgs.UpdateContractualDocInvokeArgs args)
    {
      var result = Functions.Module.DeserializeMessageFromContractualDoc(args.Message);
      var resultBody = result.body;
      var oldContractualDoc = AEOHSolution.ContractualDocuments.Null;
      var contractualDocs = AEOHSolution.ContractualDocuments.GetAll();
      
      if (!string.IsNullOrEmpty(resultBody.Id))
        oldContractualDoc = contractualDocs.Where(c => c.Id.ToString() == resultBody.Id).FirstOrDefault();
      else
        oldContractualDoc = contractualDocs.Where(c => c.Guid1C == resultBody.Guid).FirstOrDefault();
      
      var contractualDoc = oldContractualDoc != null ? oldContractualDoc : aeon.AEOHSolution.Contracts.Create();
      
      try
      {
        
        // Проверить корректность дат.
        foreach (var resultDate in new List<string>{ resultBody.RegistrationDate, resultBody.ValidTill, resultBody.ValidFrom })
        {
          var validateResult = Functions.Module.ValidateDate(resultDate);
          if (validateResult != string.Empty)
            throw new Exception(validateResult);
        }
        
        #region Обновление свойств Договорного документа.
        
        contractualDoc.Subject = contractualDoc.Subject != resultBody.Name ? resultBody.Name : contractualDoc.Subject;
        
        if (resultBody.DocumentKind.HasValue)
        {
          var kind = Functions.Module.GetDocumentKindFromId(resultBody.DocumentKind.Value);
          contractualDoc.DocumentKind = (kind != null && !Equals(kind, contractualDoc.DocumentKind)) ? kind : contractualDoc.DocumentKind;
        }
        
        if (aeon.AEOHSolution.Contracts.Is(contractualDoc) && resultBody.ContractCategory.HasValue)
        {
          var category = Functions.Module.GetCategoryFromId(resultBody.ContractCategory.Value);
          contractualDoc.DocumentGroup = (category != null && !Equals(category, contractualDoc.DocumentGroup)) ? category : contractualDoc.DocumentGroup;
        }
        
        var businessUnit = Functions.Module.GetBusinessUnitFromINNAndKPP(resultBody.BusinessUnitINN, resultBody.BusinessUnitKPP);
        contractualDoc.BusinessUnit = (businessUnit != null && !Equals(businessUnit, contractualDoc.BusinessUnit)) ? businessUnit : contractualDoc.BusinessUnit;
        
        var counterparty = Functions.Module.GetCompanyFromGuid(resultBody.Counterparty);
        contractualDoc.Counterparty = (counterparty != null && !Equals(counterparty, contractualDoc.Counterparty)) ? counterparty : contractualDoc.Counterparty;

        var responsible = Functions.Module.GetEmployeeFromName(resultBody.ResponsibleEmployee);
        if (responsible != null)
        {
          contractualDoc.ResponsibleEmployee = (responsible != null && !Equals(responsible, contractualDoc.ResponsibleEmployee)) ? responsible : contractualDoc.ResponsibleEmployee;
          contractualDoc.Department = responsible.Department;
        }
        else
        {
          var contractResponsible = Sungero.Company.Employees.GetAll(s => s.Id == args.IdContractualDocResponsible).FirstOrDefault();
          if (contractResponsible != null && contractResponsible.Department != null)
          {
            contractualDoc.ResponsibleEmployee = contractResponsible;
            contractualDoc.Department = contractResponsible.Department;
          }
        }
        
        if (!string.IsNullOrEmpty(resultBody.ValidFrom))
        {
          var validFrom = DateTime.Parse(resultBody.ValidFrom);
          contractualDoc.ValidFrom = contractualDoc.ValidFrom != validFrom ? validFrom : contractualDoc.ValidFrom;
        }
        
        if (!string.IsNullOrEmpty(resultBody.ValidTill))
        {
          var validTill = DateTime.Parse(resultBody.ValidTill);
          contractualDoc.ValidTill = contractualDoc.ValidTill != validTill ? validTill : contractualDoc.ValidTill;
        }
        
        contractualDoc.TotalAmount = (resultBody.TotalAmount.HasValue && contractualDoc.TotalAmount != resultBody.TotalAmount) ? resultBody.TotalAmount : contractualDoc.TotalAmount;

        var currency = Functions.Module.GetCurrencyFromCode(resultBody.Currency);
        contractualDoc.Currency = (currency != null && !Equals(currency, contractualDoc.Currency)) ? currency : contractualDoc.Currency;

        contractualDoc.RegistrationNumber = contractualDoc.RegistrationNumber != resultBody.RegistrationNumber ? resultBody.RegistrationNumber : contractualDoc.RegistrationNumber;
        
        if (!string.IsNullOrEmpty(resultBody.RegistrationDate))
        {
          var regDate = DateTime.Parse(resultBody.RegistrationDate);
          contractualDoc.RegistrationDate = contractualDoc.RegistrationDate != regDate ? regDate : contractualDoc.RegistrationDate;
        }
        
        var statuses = Functions.Module.GetStateForContractualDoc(resultBody.LifeCycleState, resultBody.Status, contractualDoc);
        if (statuses.InternalApprovalState.HasValue)
          contractualDoc.InternalApprovalState = statuses.InternalApprovalState;
        
        if (statuses.ExternalApprovalState.HasValue)
          contractualDoc.ExternalApprovalState = statuses.ExternalApprovalState;
        
        if (statuses.LifeCycleState.HasValue)
          contractualDoc.LifeCycleState = statuses.LifeCycleState;
        
        contractualDoc.PercentagePerAnnum = (resultBody.Percentage.HasValue && contractualDoc.PercentagePerAnnum != resultBody.Percentage)
          ? resultBody.Percentage
          : contractualDoc.PercentagePerAnnum;
        
        contractualDoc.Guid1C = contractualDoc.Guid1C != resultBody.Guid ? resultBody.Guid : contractualDoc.Guid1C;
        contractualDoc.Name = contractualDoc.Name != resultBody.Name ? resultBody.Name : contractualDoc.Name;
        
        #endregion
        
        contractualDoc.Save();
        
        var messageResult = Functions.Module.SerializeMessageFromResult(contractualDoc.Id, contractualDoc.Guid1C, result.correlation_id);
        Functions.Module.SendMessageFromRabbitMQ(messageResult, args.RoutingKeyVerification);
      }
      catch (Exception ex)
      {
        Logger.Error(ex.Message, ex);
        var messageError = Functions.Module.SerializeMessageFromError(ex.Message, result.correlation_id);
        Functions.Module.SendMessageFromRabbitMQ(messageError, args.RoutingKeyError);
      }
    }

    public virtual void SendContractualDocsIn1CAsync(aeon.Integration1C.Server.AsyncHandlerInvokeArgs.SendContractualDocsIn1CAsyncInvokeArgs args)
    {
      var setting = Functions.SettingsIntegration.GetSettingsIntegration();
      var contractualDocs = aeon.AEOHSolution.ContractualDocuments.GetAll(c => c.IsMustBeSent1C.GetValueOrDefault() && c.Counterparty != null &&
                                                                          aeon.AEOHSolution.Companies.Is(c.Counterparty));
      var exchange = setting.ExchangePoint;
      var routingKey = setting.RoutingKeySendContracts;
      var hostName = setting.ServerName;
      var virtualHost = setting.VirtualHost;
      var userName = setting.UserName;
      var password = setting.Password;
      var queueName = setting.QueueSendForContracts;
      foreach (var document in contractualDocs)
      {
        if (Locks.GetLockInfo(document).IsLocked)
          continue;
        
        try
        {
          var correlationID = Guid.NewGuid().ToString();
          var json = Functions.Module.SerializeMessageFromContractualDoc(document, correlationID, queueName);
          Logger.Error(json);
          Functions.Module.SendMessageFromRabbitMQ(json, routingKey);
          document.IsMustBeSent1C = false;
          document.CorrelationId = correlationID;
          document.Save();
        }
        catch (Exception ex)
        {
          Logger.Error(ex.Message, ex);
          Functions.Module.SendNoticeFromResponsibleForIntegration(ex.Message, document.Name, Hyperlinks.Get(document));
        }
      }
    }

    public virtual void GetIntegrationsResultsAsync(aeon.Integration1C.Server.AsyncHandlerInvokeArgs.GetIntegrationsResultsAsyncInvokeArgs args)
    {
      var setting = Functions.SettingsIntegration.GetSettingsIntegration();
      var exchange = setting.ExchangePoint;
      var hostName = setting.ServerName;
      var virtualHost = setting.VirtualHost;
      var userName = setting.UserName;
      var password = setting.Password;
      
      #region Получить ошибки интеграции.
      
      /*var queueErrors = setting.QueueErrors;
      var messageErrors = Functions.Module.GetMessagesFromRabbitMQ(queueErrors);
      foreach (var message in messageErrors)
      {
        var result = Functions.Module.DeserializeMessageFromError(message);
        var resultBody = result.body;
        var company = aeon.AEOHSolution.Companies.GetAll(c => c.CorrelationId == result.correlation_id).FirstOrDefault();
        var contractualDocs = aeon.AEOHSolution.ContractualDocuments.GetAll(c => c.CorrelationId == result.correlation_id).FirstOrDefault();
        var error = resultBody.ErrorText;
        if (company != null)
        {
          Functions.Module.SendNoticeFromResponsibleForIntegration(error, company.Name, Hyperlinks.Get(company));
        }
        else if (contractualDocs != null)
        {
          Functions.Module.SendNoticeFromResponsibleForIntegration(error, contractualDocs.Name, Hyperlinks.Get(contractualDocs));
        }
      }*/
      
      #endregion
      
      #region Получить результаты интеграции.
      
      var queueVerifications = setting.QueueGetForVerification;
      var messageVerifications = Functions.Module.GetMessagesFromRabbitMQ(queueVerifications);
      foreach (var message in messageVerifications)
      {
        var result = Functions.Module.DeserializeMessageFromVerification(message);
        var resultBody = result.body;
        var company = aeon.AEOHSolution.Companies.GetAll(c => c.CorrelationId == result.correlation_id).FirstOrDefault();
        var contractualDocs = aeon.AEOHSolution.ContractualDocuments.GetAll(c => c.CorrelationId == result.correlation_id).FirstOrDefault();
        var verification = resultBody.ErrorText;
        if (company != null)
        {
          company.IsSuccessfullyCreated1C = true;
          if (string.IsNullOrEmpty(company.Guid1C))
            company.Guid1C = resultBody.Guid;
          
          company.Save();
        }
        else if (contractualDocs != null)
        {
          contractualDocs.IsSuccessfullyCreated1C = true;
          if (string.IsNullOrEmpty(contractualDocs.Guid1C))
            contractualDocs.Guid1C = resultBody.Guid;
          
          contractualDocs.Save();
        }
      }
      
      #endregion
      
    }

    public virtual void SendCompanyIn1CAsync(aeon.Integration1C.Server.AsyncHandlerInvokeArgs.SendCompanyIn1CAsyncInvokeArgs args)
    {
      var setting = Functions.SettingsIntegration.GetSettingsIntegration();
      var companies = aeon.AEOHSolution.Companies.GetAll(c => c.IsMustBeSent1C.GetValueOrDefault() && c.Status == aeon.AEOHSolution.Company.Status.Active &&
                                                         !c.IsForOffice.GetValueOrDefault());
      var exchange = setting.ExchangePoint;
      var routingKey = setting.RoutingKeySendCounterparties;
      var hostName = setting.ServerName;
      var virtualHost = setting.VirtualHost;
      var userName = setting.UserName;
      var password = setting.Password;
      var queueName = setting.QueueSendForCounterparties;
      foreach (var company in companies)
      {
        if (Locks.GetLockInfo(company).IsLocked)
          continue;
        
        try
        {
          var correlationID = Guid.NewGuid().ToString();
          var json = Functions.Module.SerializeMessageFromCompany(company, correlationID, queueName);
          Functions.Module.SendMessageFromRabbitMQ(json, routingKey);
          company.IsMustBeSent1C = false;
          company.CorrelationId = correlationID;
          company.Save();
        }
        catch (Exception ex)
        {
          Logger.Error(ex.Message, ex);
          Functions.Module.SendNoticeFromResponsibleForIntegration(ex.Message, company.Name, Hyperlinks.Get(company));
        }
      }
    }
  }
}