using System;
using Sungero.Core;

namespace aeon.Integration1C.Constants
{
  public static class Module
  {

    public static class NodeNamesFromXml
    {
      /// <summary>
      /// Наименование узла СвОЭДОтпр.
      /// </summary>
      public const string SVOEDOsendGroup = "СвОЭДОтпр";
      
      /// <summary>
      /// Наименование аргумента ИННЮЛ.
      /// </summary>
      public const string INNUL = "ИННЮЛ";
      
      /// <summary>
      /// Наименование аргумента ИдЭДО.
      /// </summary>
      public const string IDEDO = "ИдЭДО";
      
      /// <summary>
      /// Наименование аргумента НаимОрг.
      /// </summary>
      public const string NaimOrg = "НаимОрг";

      /// <summary>
      /// Наименование узла СвУчДокОбор.
      /// </summary>
      public const string SvUchDokOborGroup = "//СвУчДокОбор";
      
      /// <summary>
      /// Наименование узла Файл.
      /// </summary>
      public const string FileGroup = "//Файл";
      
      /// <summary>
      /// Наименование атрибута ИД получателя в XML файле.
      /// </summary>
      public const string RecipientId = "ИдПол";
      
      /// <summary>
      /// Наименование атрибута ИД отправителя в XML файле.
      /// </summary>
      public const string SenderId = "ИдОтпр";
      
      /// <summary>
      /// Наименование атрибута ИД файла в XML файле.
      /// </summary>
      public const string FileId = "ИдФайл";
    }
    
    /// <summary>
    /// Знак разделения ИНН и КПП в XML файле.
    /// </summary>
    public const string SeparationSign = "_";
    
    public static class FormalizedDocTypeGuids
    {
      /// <summary>
      /// GUID типа документа "Универсальный передаточный документ".
      /// </summary>
      public const string UniversalTransferDocumentGuid = "58986e23-2b0a-4082-af37-bd1991bc6f7e";
      
      /// <summary>
      /// GUID типа документа "Акт выполненных работ".
      /// </summary>
      public const string ContractStatementInvoiceGuid = "f2f5774d-5ca3-4725-b31d-ac618f6b8850";
      
      /// <summary>
      /// GUID типа документа "Простой документ".
      /// </summary>
      public static readonly Guid SimpleDocumentTypeGuid = Guid.Parse("09584896-81e2-4c83-8f6c-70eb8321e1d0");
    }

    /// <summary>
    /// Расширение файла Pdf.
    /// </summary>
    public const string PdfExtension = "pdf";
    
    /// <summary>
    /// Расширение файла Xml.
    /// </summary>
    public const string XmlExtension = "xml";


    /// <summary>
    /// GUID вида документа "Неформализованный финансовый документ".
    /// </summary>
    [Public]
    public static readonly Guid NonFormalizedSimpleDocumentKind = Guid.Parse("057B530E-2F1B-4C0D-A5A6-6A2C3A29FCE5");

    public static class FormalizedDocType
    {
      /// <summary>
      /// Тип документа Счет-фактура выставленный.
      /// </summary>
      public const string InvoiceType = "Invoice";
      
      /// <summary>
      /// Тип документа Акт выполненных работ.
      /// </summary>
      public const string ActType = "Act";
      
      /// <summary>
      /// Тип документа Универсальный передаточный документ.
      /// </summary>
      public const string UTDType = "UTD";
    }

    #region Состояния договорного документа в 1С.
    
    /// <summary>
    /// Состояния договорного документа в 1С.
    /// </summary>
    public static class StateContractualDoc
    {
      public const string Draft = "Черновик";
      
      public const string OnSigning = "На утверждении";
      
      public const string OnRework = "Отправлен на уточнение";
      
      public const string OnSigningCompany = "Передан на подпись организации";
      
      public const string LastState = "Последний статус";
      
      public const string OnSigningCounterparty = "Передан на подпись контрагенту";
      
      public const string Signing = "Подписан";
      
      public const string Approved = "Утвержден";
      
      public const string Active = "Исполняется";
      
      public const string Closed = "Исполнен";
      
      public const string Terminated = "Расторгнут";
    }
    
    #endregion

    /// <summary>
    /// Код страны Российская Федерация.
    /// </summary>
    public const string RussiaCountryCode = "643";

    /// <summary>
    /// GUID роли "Ответственные за интеграцию с 1С".
    /// </summary>
    [Public]
    public static readonly Guid ResponsibleForIntegration1CRoleGuid = Guid.Parse("D7E2B562-EF89-45D2-858A-336DE0CB5FF4");

  }
}