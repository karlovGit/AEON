using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Sungero.Domain.Shared;
using Sungero.Metadata;
using System.Xml;

namespace aeon.Integration1C.Server
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Получить Контрагента по Guid.
    /// </summary>
    /// <param name="tin">ИНН.</param>
    /// <param name="trrc">КПП.</param>
    /// <returns>Контрагент.</returns>
    public static aeon.AEOHSolution.ICompany GetCompanyFromTinAndTrrc(string tin, string trrc)
    {
      if (string.IsNullOrEmpty(trrc))
        return AEOHSolution.Companies.GetAll(c => c.TIN == tin).FirstOrDefault();
      else
        return AEOHSolution.Companies.GetAll(c => c.TIN == tin && c.TRRC == trrc).FirstOrDefault();
    }

    /// <summary>
    /// Заменить параметры в XML-файле.
    /// </summary>
    /// <param name="document">Документ.</param>
    public void ReplaceXmlParams(Sungero.Docflow.IOfficialDocument document)
    {
      using (var memory = new System.IO.MemoryStream())
      {
        using (var bodyStream = document.LastVersion.Body.Read())
        {
          XmlDocument xmlDoc = new XmlDocument();
          xmlDoc.Load(bodyStream);
          
          var root = xmlDoc.DocumentElement;
          var node = root.SelectSingleNode(Constants.Module.NodeNamesFromXml.SvUchDokOborGroup);
          var nodeFile = root.SelectSingleNode(Constants.Module.NodeNamesFromXml.FileGroup);
          
          if (node == null || nodeFile == null)
            return;
          
          var attributeBusinessUnit = node.Attributes[Constants.Module.NodeNamesFromXml.SenderId];
          var attributeCompany = node.Attributes[Constants.Module.NodeNamesFromXml.RecipientId];
          
          if (attributeBusinessUnit == null || attributeCompany == null)
            return;
          
          var idBusinessUnit = attributeBusinessUnit.Value;
          var idCompany = attributeCompany.Value;
          
          if (idBusinessUnit == null || idCompany == null)
            return;
          
          var tinBU = idBusinessUnit.Contains(Constants.Module.SeparationSign) ? idBusinessUnit.Split('_')[0] : idBusinessUnit;
          var trrcBU = idBusinessUnit.Contains(Constants.Module.SeparationSign) ? idBusinessUnit.Split('_')[1] : string.Empty;
          
          if (string.IsNullOrEmpty(tinBU))
            return;
          
          var businessUnit = Functions.Module.GetBusinessUnitFromINNAndKPP(tinBU, trrcBU);
          if (businessUnit == null)
            return;
          
          var boxBusinessUnit = Sungero.ExchangeCore.BusinessUnitBoxes.GetAll(b => Equals(b.BusinessUnit, businessUnit) && b.Status == Sungero.ExchangeCore.BusinessUnitBox.Status.Active).FirstOrDefault();
          if (boxBusinessUnit == null || string.IsNullOrEmpty(boxBusinessUnit.OrganizationId))
            return;
          
          var tinCompany = idCompany.Contains(Constants.Module.SeparationSign) ? idCompany.Split('_')[0] : idCompany;
          var trrcCompany = idCompany.Contains(Constants.Module.SeparationSign) ? idCompany.Split('_')[1] : string.Empty;
          
          if (string.IsNullOrEmpty(tinCompany))
            return;
          
          var company = Functions.Module.GetCompanyFromTinAndTrrc(tinCompany, trrcCompany);
          if (company == null || !company.ExchangeBoxes.Any())
            return;
          
          var ftsIdBusinessUnit = boxBusinessUnit.FtsId;
          var ftsIdCompany = company.ExchangeBoxes.FirstOrDefault().FtsId;
          node.Attributes[Constants.Module.NodeNamesFromXml.SenderId].Value = ftsIdBusinessUnit;
          node.Attributes[Constants.Module.NodeNamesFromXml.RecipientId].Value = ftsIdCompany;
          
          var idFile = nodeFile.Attributes[Constants.Module.NodeNamesFromXml.FileId].Value;
          
          if (idFile.Contains(idBusinessUnit))
            idFile = idFile.Replace(idBusinessUnit, ftsIdBusinessUnit);
          
          if (idFile.Contains(idCompany))
            idFile = idFile.Replace(idCompany, ftsIdCompany);
          
          nodeFile.Attributes[Constants.Module.NodeNamesFromXml.FileId].Value = idFile;
          
          #region Создание нового элемента СвОЭДОтпр.
          
          var sVOEDOsendElement = xmlDoc.CreateElement(Constants.Module.NodeNamesFromXml.SVOEDOsendGroup);
          
          var innUlAttribute = xmlDoc.CreateAttribute(Constants.Module.NodeNamesFromXml.INNUL);
          innUlAttribute.Value = tinBU;
          sVOEDOsendElement.Attributes.Append(innUlAttribute);
          
          var idEDOAttribute = xmlDoc.CreateAttribute(Constants.Module.NodeNamesFromXml.IDEDO);
          var gtsIdBusinessUnit = boxBusinessUnit.FtsId;
          idEDOAttribute.Value = gtsIdBusinessUnit.Substring(0, 3);
          sVOEDOsendElement.Attributes.Append(idEDOAttribute);
          
          var naimOrgAttribute = xmlDoc.CreateAttribute(Constants.Module.NodeNamesFromXml.NaimOrg);
          naimOrgAttribute.Value = businessUnit.Name;
          sVOEDOsendElement.Attributes.Append(naimOrgAttribute);

          node.AppendChild(sVOEDOsendElement);
          
          #endregion
          
          xmlDoc.Save(memory);
          
          document.LastVersion.Body.Write(memory);
          document.Save();
        }
      }
    }

    /// <summary>
    /// Получить действующее право подписи по виду документа и нашей организации.
    /// </summary>
    /// <param name="businessUnit">Наша организация.</param>
    /// <param name="kind">Вид документа.</param>
    /// <returns>Действующее право подписи.</returns>
    public static Sungero.Docflow.ISignatureSetting GetSignatureSettings(Sungero.Company.IBusinessUnit businessUnit, Sungero.Docflow.IDocumentKind kind)
    {
      var today = Calendar.UserToday;
      return Sungero.Docflow.SignatureSettings.GetAll()
        .Where(s => s.Status == Sungero.CoreEntities.DatabookEntry.Status.Active)
        .Where(s => s.BusinessUnits.Any(b => Equals(b.BusinessUnit, businessUnit)))
        .Where(s => s.DocumentKinds.Any(k => Equals(k.DocumentKind, kind)) ||
               (!s.DocumentKinds.Any() && (s.DocumentFlow == Sungero.Docflow.SignatureSetting.DocumentFlow.All || s.DocumentFlow == kind.DocumentFlow)))
        .Where(s => (!s.ValidFrom.HasValue || s.ValidFrom.Value <= today) && (!s.ValidTill.HasValue || s.ValidTill.Value >= today))
        .Where(s => Sungero.Company.Employees.Is(s.Recipient))
        .OrderByDescending(s => s.Priority)
        .FirstOrDefault();
    }

    /// <summary>
    /// Прекратить задачи на согласование.
    /// </summary>
    /// <param name="document">Документ для которого нужно прекратить задачи на согласование.</param>
    public void AbortApprovalTasks(AEOHSolution.IAccountingDocumentBase document)
    {
      var approvalTaskDocumentGroupGuid = Sungero.Docflow.PublicConstants.Module.TaskMainGroup.ApprovalTask;
      var docGuid = document.GetEntityMetadata().GetOriginal().NameGuid;
      var approvalTasks = Sungero.Docflow.ApprovalTasks.GetAll(t => (t.Status == Sungero.Workflow.Task.Status.InProcess || t.Status == Sungero.Workflow.Task.Status.Suspended) &&
                                                               t.AttachmentDetails.Any(att => att.AttachmentId == document.Id && att.EntityTypeGuid == docGuid &&
                                                                                       att.GroupId == approvalTaskDocumentGroupGuid));
      foreach (var task in approvalTasks)
      {
        if (Locks.GetLockInfo(task).IsLocked)
          Locks.Unlock(task);
        
        task.AbortingReason = aeon.Integration1C.Resources.AbortingReasonApprovalFinancialDocs;
        task.Abort();
      }
    }

    /// <summary>
    /// Создать версию документа.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="resultBody">Тело документа в формате base64.</param>
    /// <param name="extension">Расширение файла.</param>
    public static void CreateVersionFromBase64String(Sungero.Docflow.IOfficialDocument document, string resultBody, string extension)
    {
      if (!string.IsNullOrEmpty(resultBody))
      {
        var body = Convert.FromBase64String(resultBody);
        using (var bodyStream = new System.IO.MemoryStream(body))
        {
          document.CreateVersionFrom(bodyStream, extension);
          document.Save();
        }
      }
    }
    
    /// <summary>
    /// Создать версию формализованногодокумента.
    /// </summary>
    /// <param name="formalizedDoc">Документ.</param>
    /// <param name="resultBody">Тело документа в формате base64.</param>
    /// <param name="isContractStatementOrUtd">Акт или УПД.</param>
    public static void CreateVersionFormalizedDocFromBase64String(Sungero.Docflow.IAccountingDocumentBase formalizedDoc, string resultBody, bool isContractStatementOrUtd)
    {
      byte[] bytes = Convert.FromBase64String(resultBody);
      var sellerTitle = Sungero.FormalizeDocumentsParser.Extension.GetDocument<Sungero.FormalizeDocumentsParser.ISellerTitle>(bytes);
      if (sellerTitle.Function.HasValue)
      {
        switch (sellerTitle.Function.Value)
        {
          case Sungero.FormalizeDocumentsParser.UniversalDocumentFunction.Schf:
            formalizedDoc.FormalizedFunction = Sungero.Docflow.AccountingDocumentBase.FormalizedFunction.Schf;
            break;
          case Sungero.FormalizeDocumentsParser.UniversalDocumentFunction.SchfDop:
            formalizedDoc.FormalizedFunction = Sungero.Docflow.AccountingDocumentBase.FormalizedFunction.SchfDop;
            break;
          case Sungero.FormalizeDocumentsParser.UniversalDocumentFunction.Dop:
            formalizedDoc.FormalizedFunction = Sungero.Docflow.AccountingDocumentBase.FormalizedFunction.Dop;
            break;
          default:
            throw new Exception("Invalid value for UniversalDocumentFunction");
        }
      }
      
      var version = formalizedDoc.Versions.AddNew();
      version.AssociatedApplication = Sungero.Content.AssociatedApplications.GetByExtension(Constants.Module.PdfExtension);
      version.BodyAssociatedApplication = Sungero.Content.AssociatedApplications.GetByExtension(Constants.Module.XmlExtension);
      formalizedDoc.SellerTitleId = version.Id;
      
      if (isContractStatementOrUtd)
        version.Note = Sungero.FinancialArchive.Resources.SellerTitleVersionNote;
      
      formalizedDoc.IsFormalized = true;
      
      var importResultBody = sellerTitle.Body;
      var importResultPublicBody = Sungero.Docflow.PublicFunctions.Module.Remote.GeneratePublicBodyForFormalizedXml(Sungero.Docflow.Structures.Module.ByteArray.Create(sellerTitle.Body)).Bytes;
      using (var memory = new System.IO.MemoryStream(importResultBody))
      {
        using (var publicMemory = new System.IO.MemoryStream(importResultPublicBody))
        {
          version.Body.Write(memory);
          version.PublicBody.Write(publicMemory);
          formalizedDoc.Save();
        }
      }
    }

    /// <summary>
    ///  Получить ведущий документ из пакета.
    /// </summary>
    /// <param name="packageGuid">Guid пакета документов.</param>
    /// <returns>Ведущий документ.</returns>
    public static AEOHSolution.IAccountingDocumentBase GetLeadingDocument(string packageGuid)
    {
      return AEOHSolution.AccountingDocumentBases
        .GetAll(d => d.PackageGuid == packageGuid && d.LifeCycleState != AEOHSolution.AccountingDocumentBase.LifeCycleState.Obsolete &&
                (Sungero.FinancialArchive.ContractStatements.Is(d) || Sungero.FinancialArchive.UniversalTransferDocuments.Is(d)))
        .FirstOrDefault();
    }

    /// <summary>
    /// Аннулировать текущий документ и связанные с ним документы.
    /// </summary>
    /// <param name="document">Документ для аннулирования.</param>
    public void CancelesOldFormalizedDoc(AEOHSolution.IAccountingDocumentBase document)
    {
      document.LifeCycleState = AEOHSolution.AccountingDocumentBase.LifeCycleState.Obsolete;
      document.Guid1C = null;
      document.Save();
      
      var relations = document.Relations.GetRelated(Sungero.Docflow.PublicConstants.Module.AddendumRelationName).Where(r => AEOHSolution.AccountingDocumentBases.Is(r) || Sungero.Docflow.SimpleDocuments.Is(r));
      foreach (var relatedDocument in relations)
      {
        var accountingDocument = AEOHSolution.AccountingDocumentBases.As(relatedDocument);
        var simpleDocument = Sungero.Docflow.SimpleDocuments.As(relatedDocument);
        if (accountingDocument != null)
        {
          Functions.Module.AbortApprovalTasks(accountingDocument);
          
          accountingDocument.LifeCycleState = AEOHSolution.AccountingDocumentBase.LifeCycleState.Obsolete;
          accountingDocument.Guid1C = null;
          accountingDocument.Save();
        }
        else if (simpleDocument != null)
        {
          simpleDocument.LifeCycleState = Sungero.Docflow.SimpleDocument.LifeCycleState.Obsolete;
          simpleDocument.Save();
        }
      }
    }

    /// <summary>
    /// Проверить корректность даты.
    /// </summary>
    /// <param name="stringDate">Дата.</param>
    /// <returns>Результат проверки.</returns>
    public static string ValidateDate(string stringDate)
    {
      var result = string.Empty;
      
      if (string.IsNullOrEmpty(stringDate))
        return result;
      
      var date = DateTime.Parse(stringDate);
      if (date == null || !Calendar.Between(date, Calendar.SqlMinValue, Calendar.SqlMaxValue))
        result = aeon.Integration1C.Resources.SqlDateTimeException;
      
      return result;
    }

    /// <summary>
    /// Получить роль "Ответственные за интеграцию с 1С".
    /// </summary>
    /// <returns>Роль "Ответственные за интеграцию с 1С".</returns>
    public static IRole GetResponsibleForIntegration1CRole()
    {
      return Roles.GetAll(r => r.Sid == Constants.Module.ResponsibleForIntegration1CRoleGuid).SingleOrDefault();
    }

    /// <summary>
    /// Получить значения полей Состояние и Согласование для договорного документа.
    /// </summary>
    /// <param name="lifeCycleState">Состояние документа из 1С.</param>
    /// <param name="status">Статус документа из 1С.</param>
    /// <returns>Состояние и Согласование в Directum RX.</returns>
    public static Structures.Module.StateForContractualDoc GetStateForContractualDoc(string lifeCycleState, string status, AEOHSolution.IContractualDocument document)
    {
      var state = Structures.Module.StateForContractualDoc.Create();
      
      // Черновик.
      if (lifeCycleState == Constants.Module.StateContractualDoc.Draft)
      {
        state.LifeCycleState = AEOHSolution.ContractualDocument.LifeCycleState.Draft;
        
        // На утверждении.
        if (status == Constants.Module.StateContractualDoc.OnSigning)
          state.InternalApprovalState = AEOHSolution.ContractualDocument.InternalApprovalState.OnApproval;
      }
      // Отправлен на уточнение и Отправлен на уточнение.
      else if (lifeCycleState == Constants.Module.StateContractualDoc.OnRework &&
               status == Constants.Module.StateContractualDoc.OnRework)
      {
        state.LifeCycleState = AEOHSolution.ContractualDocument.LifeCycleState.Draft;
        state.InternalApprovalState = AEOHSolution.ContractualDocument.InternalApprovalState.OnRework;
      }
      // Передан на подпись организации и На утверждении.
      else if (lifeCycleState == Constants.Module.StateContractualDoc.OnSigningCompany &&
               status == Constants.Module.StateContractualDoc.OnSigning)
      {
        state.LifeCycleState = AEOHSolution.ContractualDocument.LifeCycleState.Draft;
        state.InternalApprovalState = AEOHSolution.ContractualDocument.InternalApprovalState.PendingSign;
      }
      // Передан на подпись контрагенту и На утверждении.
      else if (lifeCycleState == Constants.Module.StateContractualDoc.OnSigningCounterparty &&
               status == Constants.Module.StateContractualDoc.OnSigning)
      {
        state.LifeCycleState = AEOHSolution.ContractualDocument.LifeCycleState.Draft;
        state.InternalApprovalState = AEOHSolution.ContractualDocument.InternalApprovalState.Signed;
        state.ExternalApprovalState = AEOHSolution.ContractualDocument.ExternalApprovalState.OnApproval;
      }
      // Подписан и Утвержден.
      else if (lifeCycleState == Constants.Module.StateContractualDoc.Signing &&
               status == Constants.Module.StateContractualDoc.Approved)
      {
        state.LifeCycleState = AEOHSolution.ContractualDocument.LifeCycleState.Draft;
        state.InternalApprovalState = AEOHSolution.ContractualDocument.InternalApprovalState.Signed;
        state.ExternalApprovalState = AEOHSolution.ContractualDocument.ExternalApprovalState.Signed;
      }
      // Исполняется и Утвержден.
      else if (lifeCycleState == Constants.Module.StateContractualDoc.Active &&
               status == Constants.Module.StateContractualDoc.Approved)
      {
        state.LifeCycleState = AEOHSolution.ContractualDocument.LifeCycleState.Active;
        state.InternalApprovalState = AEOHSolution.ContractualDocument.InternalApprovalState.Signed;
        state.ExternalApprovalState = AEOHSolution.ContractualDocument.ExternalApprovalState.Signed;
      }
      // Исполнен и Утвержден.
      else if (lifeCycleState == Constants.Module.StateContractualDoc.Closed &&
               status == Constants.Module.StateContractualDoc.Approved)
      {
        state.LifeCycleState = AEOHSolution.Contracts.Is(document)
          ? AEOHSolution.Contract.LifeCycleState.Closed
          : AEOHSolution.SupAgreement.LifeCycleState.Closed;
        state.InternalApprovalState = AEOHSolution.ContractualDocument.InternalApprovalState.Signed;
        state.ExternalApprovalState = AEOHSolution.ContractualDocument.ExternalApprovalState.Signed;
      }
      // Расторгнут и Последний статус.
      else if (lifeCycleState == Constants.Module.StateContractualDoc.Terminated)
      {
        state.LifeCycleState = AEOHSolution.Contract.LifeCycleState.Terminated;
      }
      
      if (document.State.IsInserted && document.DocumentKind != null &&
          aeon.AEOHSolution.DocumentKinds.As(document.DocumentKind).LoanAgreement.GetValueOrDefault())
        
      {
        state.InternalApprovalState = AEOHSolution.ContractualDocument.InternalApprovalState.PendingSign;
        state.ExternalApprovalState = AEOHSolution.ContractualDocument.ExternalApprovalState.OnApproval;
      }
      
      return state;
    }
    
    #region Отправка результатов создания/обновления сущностей из 1С.
    
    /// <summary>
    /// Отправить уведомление об ошибке ответственному за интеграцию.
    /// </summary>
    /// <param name="error">Ошибка.</param>
    /// <param name="name">Имя сущности.</param>
    /// <param name="hyperlinks">Гиперссылка на сущность.</param>
    public static void SendNoticeFromResponsibleForIntegration(string error, string name, string hyperlinks)
    {
      var responsibleForIntegration = Functions.Module.GetResponsibleForIntegration1CRole();
      var simpleTasks = Sungero.Workflow.SimpleTasks.GetAll(t => t.Author.IsSystem.GetValueOrDefault());
      var subject = aeon.Integration1C.Resources.SubjectIntegrationErrorFormat(name, error);
      if (!simpleTasks.Where(t => t.Subject == subject).Any())
      {
        var notice = Sungero.Workflow.SimpleTasks.CreateWithNotices(subject, responsibleForIntegration);
        notice.ActiveText = aeon.Integration1C.Resources.SubjectIntegrationErrorFormat(hyperlinks, error);
        notice.Start();
      }
    }
    
    /// <summary>
    /// Сериализовать сообщение об успешном создании/обновлении.
    /// </summary>
    /// <param name="id">ИД сущности.</param>
    /// <param name="guid">Guid сущности.</param>
    /// <param name="correlation_id">ИД события.</param>
    /// <returns>Сообщение об успешном создании/обновлении.</returns>
    public static string SerializeMessageFromResult(int id, string guid, string correlation_id)
    {
      var setting = Functions.SettingsIntegration.GetSettingsIntegration();
      var queueName = setting.QueueSendForVerificationHM;
      var message = string.Empty;
      
      var json = Structures.Module.ResultMesageFromRabbitMQ.Create();
      json.message_id = Guid.NewGuid().ToString();
      json.message_date = Calendar.Now.ToString();
      json.correlation_id = correlation_id;
      json.event_name = queueName;
      
      var description = Structures.Module.ResultUpdateEntityFrom1C.Create();
      description.Id = id;
      description.Guid = guid;
      description.Verification = true;
      description.ErrorText = string.Empty;
      json.body = description;
      message = JsonConvert.SerializeObject(json);
      
      return message;
    }
    
    /// <summary>
    /// Сериализовать сообщение об успешном создании финансового документа.
    /// </summary>
    /// <param name="id">ИД сущности.</param>
    /// <param name="correlation_id">ИД события.</param>
    /// <returns>Сообщение об успешном создании.</returns>
    public static string SerializeMessageFinancialDocFromResult(int id, string correlation_id)
    {
      var setting = Functions.SettingsIntegration.GetSettingsIntegration();
      var queueName = setting.QueueSendForVerificationHM;
      var message = string.Empty;
      
      var json = Structures.Module.ResultMesageFinancialDocFromRabbitMQ.Create();
      json.message_id = Guid.NewGuid().ToString();
      json.message_date = Calendar.Now.ToString();
      json.correlation_id = correlation_id;
      json.event_name = queueName;
      
      var description = Structures.Module.ResultUpdateFinancialDocFrom1C.Create();
      description.Id = id;
      description.Verification = true;
      description.ErrorText = string.Empty;
      json.body = description;
      
      message = JsonConvert.SerializeObject(json);
      
      return message;
    }

    /// <summary>
    /// Сериализовать сообщение об ошибке.
    /// </summary>
    /// <param name="error">Ошибка.</param>
    /// <param name="correlation_id">ИД события.</param>
    /// <param name="queueErrors">Очередь ошибки.</param>
    /// <returns>Сообщение об ошибке.</returns>
    public static string SerializeMessageFromError(string error, string correlation_id, string queueErrors)
    {
      var setting = Functions.SettingsIntegration.GetSettingsIntegration();
      var message = string.Empty;
      
      var json = Structures.Module.ErrorMesageFromRabbitMQ.Create();
      json.message_id = Guid.NewGuid().ToString();
      json.message_date = Calendar.Now.ToString();
      json.correlation_id = correlation_id;
      json.event_name = queueErrors;
      
      var description = Structures.Module.ErrorUpdateEntityFrom1C.Create();
      description.Verification = false;
      description.ErrorText = error;
      json.body = description;
      message = JsonConvert.SerializeObject(json);
      
      return message;
    }
    
    #endregion

    #region Получить данные для обновления сущности.
    
    /// <summary>
    /// Получить Валюту по коду.
    /// </summary>
    /// <param name="code">Код.</param>
    /// <returns>Валюта.</returns>
    public static Sungero.Commons.ICurrency GetCurrencyFromCode(string code)
    {
      return Sungero.Commons.Currencies.GetAll(c => c.NumericCode == code).FirstOrDefault();
    }
    
    /// <summary>
    /// Получить Сотрудника по ФИО.
    /// </summary>
    /// <param name="name">ФИО.</param>
    /// <returns>Сотрудник.</returns>
    public static Sungero.Company.IEmployee GetEmployeeFromName(string name)
    {
      return Sungero.Company.Employees.GetAll(c => c.Name == name).FirstOrDefault();
    }
    
    /// <summary>
    /// Получить Контрагента по Guid.
    /// </summary>
    /// <param name="guid">Guid.</param>
    /// <returns>Контрагент.</returns>
    public static aeon.AEOHSolution.ICompany GetCompanyFromGuid(string guid)
    {
      return AEOHSolution.Companies.GetAll(c => c.Guid1C == guid).FirstOrDefault();
    }
    
    /// <summary>
    /// Получить Нашу организацию по ИНН и КПП.
    /// </summary>
    /// <param name="tin">ИНН.</param>
    /// <param name="trrc">КПП.</param>
    /// <returns>Наша организация.</returns>
    public static Sungero.Company.IBusinessUnit GetBusinessUnitFromINNAndKPP(string tin, string trrc)
    {
      if (string.IsNullOrEmpty(trrc) || string.IsNullOrWhiteSpace(trrc))
        return Sungero.Company.BusinessUnits.GetAll(c => c.TIN == tin).FirstOrDefault();
      else
        return Sungero.Company.BusinessUnits.GetAll(c => c.TIN == tin && c.TRRC == trrc).FirstOrDefault();
    }
    
    /// <summary>
    /// Получить Категорию договора по ИД.
    /// </summary>
    /// <param name="categoryId">ИД категории.</param>
    /// <returns>Категория договора.</returns>
    public static Sungero.Contracts.IContractCategory GetCategoryFromId(int categoryId)
    {
      return Sungero.Contracts.ContractCategories.GetAll(c => c.Id == categoryId).FirstOrDefault();
    }
    
    /// <summary>
    /// Получить Вид документа по ИД.
    /// </summary>
    /// <param name="kindId">ИД вида документа.</param>
    /// <returns>Вид документа.</returns>
    public static Sungero.Docflow.IDocumentKind GetDocumentKindFromId(int kindId)
    {
      return Sungero.Docflow.DocumentKinds.GetAll(c => c.Id == kindId).FirstOrDefault();
    }

    /// <summary>
    /// Получить Страну по коду. Если страны нет, то создать новую.
    /// </summary>
    /// <param name="code">Код.</param>
    /// <param name="name">Наименование.</param>
    /// <returns>Страна.</returns>
    public static Sungero.Commons.ICountry GetCountryFromCode(string code, string name)
    {
      if (string.IsNullOrEmpty(code))
        return Sungero.Commons.Countries.GetAll(c => c.Code == Constants.Module.RussiaCountryCode).FirstOrDefault();
      
      var oldCountry = Sungero.Commons.Countries.GetAll(c => c.Code == code).FirstOrDefault();
      if (oldCountry != null)
        return oldCountry;
      
      var country = Sungero.Commons.Countries.Create();
      country.Name = name;
      country.Code = code;
      country.Save();
      
      return country;
    }
    
    /// <summary>
    /// Получить Вид контрагента по наименованию.
    /// </summary>
    /// <param name="name">Наименование.</param>
    /// <returns>Вид контрагента.</returns>
    public static ICounterpartyKind GetCounterpartyKindFromName(string name)
    {
      return CounterpartyKinds.GetAll(c => c.Name == name).FirstOrDefault();
    }
    
    /// <summary>
    /// Получить Банк по БИК.
    /// </summary>
    /// <param name="bic">БИК.</param>
    /// <returns>Банк.</returns>
    public static Sungero.Parties.IBank GetBankFromBIC(string bic)
    {
      return Sungero.Parties.Banks.GetAll(c => c.BIC == bic).FirstOrDefault();
    }
    
    /// <summary>
    /// Получить Регион по наименованию.
    /// </summary>
    /// <param name="name">Наименование.</param>
    /// <returns>Регион.</returns>
    public static Sungero.Commons.IRegion GetRegionFromName(string name)
    {
      return Sungero.Commons.Regions.GetAll(c => c.Name == name).FirstOrDefault();
    }
    
    /// <summary>
    /// Получить Населенный пункт по наименованию.
    /// </summary>
    /// <param name="name">Наименование.</param>
    /// <returns>Населенный пункт.</returns>
    public static Sungero.Commons.ICity GetCityFromName(string name)
    {
      return Sungero.Commons.Cities.GetAll(c => c.Name == name).FirstOrDefault();
    }

    #endregion
    
    #region Отправка и получение сообщений.
    
    /// <summary>
    /// Получить сообщения из RabbitMQ.
    /// </summary>
    /// <param name="queue">Имя очереди.</param>
    /// <returns>Сообщения из RabbitMQ.</returns>
    public static List<string> GetMessagesFromRabbitMQ(string queue)
    {
      var setting = Functions.SettingsIntegration.GetSettingsIntegration();
      var messages = new List<string>();
      var factory = new ConnectionFactory()
      {
        HostName = setting.ServerName,
        Port = Constants.SettingsIntegration.PortRabbitMQ,
        VirtualHost = setting.VirtualHost,
        UserName = setting.UserName,
        Password = setting.Password
      };
      
      using (var connection = factory.CreateConnection())
      {
        using (var channel = connection.CreateModel())
        {
          var c = channel.MessageCount(queue);
          while (c != 0)
          {
            bool noAck = true;
            BasicGetResult result = channel.BasicGet(queue, noAck);
            if (result != null)
            {
              IBasicProperties props = result.BasicProperties;
              messages.Add(Encoding.UTF8.GetString(result.Body.ToArray()));
            }
            --c;
          }
        }
      }
      return messages;
    }

    /// <summary>
    /// Отправить сообщение в RabbitMQ.
    /// </summary>
    /// <param name="json">Json.</param>
    /// <param name="routingKey">Ключ очереди.</param>
    /// <param name="exchangePoint">Точка обмена.</param>
    public void SendMessageFromRabbitMQ(string json, string routingKey, string exchangePoint)
    {
      var setting = Functions.SettingsIntegration.GetSettingsIntegration();
      var hostName = setting.ServerName;
      var virtualHost = setting.VirtualHost;
      var userName = setting.UserName;
      var password = setting.Password;
      
      var result = new List<string>();
      var factory = new ConnectionFactory()
      {
        HostName = hostName,
        Port = Constants.SettingsIntegration.PortRabbitMQ,
        VirtualHost = virtualHost,
        UserName = userName,
        Password = password
      };
      using (var connection = factory.CreateConnection())
      {
        using (var channel = connection.CreateModel())
        {
          var body = Encoding.UTF8.GetBytes(json);

          channel.BasicPublish(exchange: exchangePoint,
                               routingKey: routingKey,
                               basicProperties: null,
                               body: body);
        }
      }
    }

    #endregion
    
    #region Десериализация сообщений из RabbitMQ.
    
    /// <summary>
    /// Десериализовать сообщение результата из RabbitMQ.
    /// </summary>
    /// <param name="json">Полученный Json.</param>
    /// <returns>Сообщение из RabbitMQ.</returns>
    public static Structures.Module.ResultMesageFromRabbitMQ DeserializeMessageFromVerification(string json)
    {
      return JsonConvert.DeserializeObject<Structures.Module.ResultMesageFromRabbitMQ>(json);
    }
    
    /// <summary>
    /// Десериализовать сообщение ошибки из RabbitMQ.
    /// </summary>
    /// <param name="json">Полученный Json.</param>
    /// <returns>Сообщение из RabbitMQ.</returns>
    public static Structures.Module.ErrorMesageFromRabbitMQ DeserializeMessageFromError(string json)
    {
      return JsonConvert.DeserializeObject<Structures.Module.ErrorMesageFromRabbitMQ>(json);
    }
    
    /// <summary>
    /// Десериализовать сообщение из RabbitMQ для Договорного документа.
    /// </summary>
    /// <param name="json">Полученный Json.</param>
    /// <returns>Сообщение из RabbitMQ.</returns>
    public static Structures.Module.ContractualDocMessageGetting DeserializeMessageFromContractualDoc(string json)
    {
      return JsonConvert.DeserializeObject<Structures.Module.ContractualDocMessageGetting>(json);
    }
    
    /// <summary>
    /// Десериализовать сообщение из RabbitMQ для Организации.
    /// </summary>
    /// <param name="json">Полученный Json.</param>
    /// <returns>Сообщение из RabbitMQ.</returns>
    public static Structures.Module.CompanyMessageGetting DeserializeMessageFromCompany(string json)
    {
      return JsonConvert.DeserializeObject<Structures.Module.CompanyMessageGetting>(json);
    }
    
    /// <summary>
    /// Десериализовать сообщение из RabbitMQ для Формализованного документа.
    /// </summary>
    /// <param name="json">Полученный Json.</param>
    /// <returns>Сообщение из RabbitMQ.</returns>
    public static Structures.Module.FormalizedDocMessageGetting DeserializeMessageFromFormalizedDoc(string json)
    {
      return JsonConvert.DeserializeObject<Structures.Module.FormalizedDocMessageGetting>(json);
    }
    
    /// <summary>
    /// Десериализовать сообщение из RabbitMQ для Неформализованного документа.
    /// </summary>
    /// <param name="json">Полученный Json.</param>
    /// <returns>Сообщение из RabbitMQ.</returns>
    public static Structures.Module.NonFormalizedDocMessageGetting DeserializeMessageFromNonFormalizedDoc(string json)
    {
      return JsonConvert.DeserializeObject<Structures.Module.NonFormalizedDocMessageGetting>(json);
    }
    
    #endregion
    
    #region Сериализация сообщений из RabbitMQ.
    
    /// <summary>
    /// Сериализовать сообщение для отправки Договорного документа в RabbitMQ.
    /// </summary>
    /// <param name="document">Договорной документ.</param>
    /// <param name="correlationID">ИД отправляемого запроса.</param>
    /// <param name="queueName">Имя очереди.</param>
    /// <returns>Сформированный Json.</returns>
    public static string SerializeMessageFromContractualDoc(aeon.AEOHSolution.IContractualDocument document, string correlationID, string queueName)
    {
      var message = string.Empty;
      
      var json = Structures.Module.ContractualDocMessageSending.Create();
      json.message_id = Guid.NewGuid().ToString();
      json.message_date = Calendar.Now.ToString();
      json.correlation_id = correlationID;
      json.event_name = queueName;
      
      var description = Structures.Module.ContractualDocDescriptionSending.Create();
      description.Id = document.Id;
      
      if (document.DocumentKind != null)
        description.DocumentKind = document.DocumentKind.Id;
      
      if (document.DocumentGroup != null)
        description.ContractCategory = document.DocumentGroup.Id;
      
      if (document.BusinessUnit != null)
      {
        description.BusinessUnitINN = document.BusinessUnit.TIN;
        description.BusinessUnitKPP = document.BusinessUnit.TRRC;
      }
      description.Counterparty = aeon.AEOHSolution.Companies.As(document.Counterparty).Guid1C;
      
      if (document.ResponsibleEmployee != null)
        description.ResponsibleEmployee = document.ResponsibleEmployee.Name;
      
      description.ValidFrom = document.ValidFrom.HasValue ? document.ValidFrom.Value.ToShortDateString() : null;
      description.ValidTill = document.ValidTill.HasValue ? document.ValidTill.Value.ToShortDateString() : null;
      description.TotalAmount = document.TotalAmount;
      
      if (document.Currency != null)
        description.Currency = document.Currency.NumericCode;
      
      description.RegistrationNumber = document.RegistrationNumber;
      description.RegistrationDate = document.RegistrationDate.HasValue ? document.RegistrationDate.Value.ToShortDateString() : null;
      description.LifeCycleState = document.LifeCycleState.HasValue ? document.LifeCycleState.Value.ToString() : string.Empty;
      description.InternalApprovalState = document.InternalApprovalState.HasValue ? document.InternalApprovalState.Value.ToString() : string.Empty;
      description.ExternalApprovalState = document.ExternalApprovalState.HasValue ? document.ExternalApprovalState.Value.ToString() : string.Empty;
      description.Percentage = document.PercentagePerAnnum;
      
      var contractGuid = string.Empty;
      var supAgreement = aeon.AEOHSolution.SupAgreements.As(document);
      
      if (supAgreement != null && supAgreement.LeadingDocument != null)
        contractGuid = supAgreement.LeadingDocument.Guid1C;
      
      description.ContractGuid = contractGuid;
      description.Guid = !string.IsNullOrEmpty(document.Guid1C) ? document.Guid1C : string.Empty;
      json.body = description;
      message = JsonConvert.SerializeObject(json);
      
      return message;
    }
    
    /// <summary>
    /// Сериализовать сообщение для отправки Организации в RabbitMQ.
    /// </summary>
    /// <param name="company">Организация.</param>
    /// <param name="correlationID">ИД отправляемого запроса.</param>
    /// <param name="queueName">Имя очереди.</param>
    /// <returns>Сформированный Json.</returns>
    public static string SerializeMessageFromCompany(aeon.AEOHSolution.ICompany company, string correlationID, string queueName)
    {
      var message = string.Empty;
      
      var json = Structures.Module.CompanyMessageSending.Create();
      json.message_id = Guid.NewGuid().ToString();
      json.message_date = Calendar.Now.ToString();
      json.correlation_id = correlationID;
      json.event_name = queueName;
      
      var description = Structures.Module.CompanyDescriptionSending.Create();
      description.TIN = !string.IsNullOrEmpty(company.TIN) ? company.TIN : string.Empty;
      description.TRRC = !string.IsNullOrEmpty(company.TRRC) ? company.TRRC : string.Empty;
      description.Id = company.Id.ToString();
      json.body = description;
      message = JsonConvert.SerializeObject(json);
      
      return message;
    }
    
    #endregion

  }
}