using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace aeon.Integration1C.Structures.Module
{

  #region Результаты успешного создания Финансовых документов.
  
  /// <summary>
  /// Структура сообщения успешного создания документа из 1С.
  /// </summary>
  partial class ResultMesageFinancialDocFromRabbitMQ
  {
    public string message_id { get; set; }
    
    public string message_date { get; set; }
    
    public string correlation_id { get; set; }
    
    public string event_name { get; set; }
    
    public aeon.Integration1C.Structures.Module.ResultUpdateFinancialDocFrom1C body { get; set; }
  }
  
  /// <summary>
  /// Структура Json успешного создания документа из 1С.
  /// </summary>
  partial class ResultUpdateFinancialDocFrom1C
  {
    public int Id { get; set; }
        
    public bool Verification { get; set; }
    
    public string ErrorText { get; set; }
  }
  
  #endregion
  
  #region Получение Неформализованных документов из 1С.
  
  /// <summary>
  /// Структура сообщения получения Неформализованного документа.
  /// </summary>
  partial class NonFormalizedDocMessageGetting
  {
    public string message_id { get; set; }
    
    public string message_date { get; set; }
    
    public string correlation_id { get; set; }
    
    public string event_name { get; set; }
    
    public aeon.Integration1C.Structures.Module.NonFormalizedDocDescriptionGetting body { get; set; }
  }
  
  /// <summary>
  /// Структура Json принимаемых свойств Неформализованного документа.
  /// </summary>
  partial class NonFormalizedDocDescriptionGetting
  {
    public string PacketGUID { get; set; }
    
    public string Name { get; set; }
    
    public string DocumentBody { get; set; }
  }
  
  #endregion

  #region Получение Формализованных документов из 1С.
  
  /// <summary>
  /// Структура сообщения получения Формализованного документа.
  /// </summary>
  partial class FormalizedDocMessageGetting
  {
    public string message_id { get; set; }
    
    public string message_date { get; set; }
    
    public string correlation_id { get; set; }
    
    public string event_name { get; set; }
    
    public aeon.Integration1C.Structures.Module.FormalizedDocDescriptionGetting body { get; set; }
  }
  
  /// <summary>
  /// Структура Json принимаемых свойств Формализованного документа.
  /// </summary>
  partial class FormalizedDocDescriptionGetting
  {
    public string PacketGUID { get; set; }
    
    public string DocumentType { get; set; }
    
    public int? TotalCountDocument { get; set;}
    
    public string GUID { get; set; }
    
    public string Contract { get; set; }
    
    public string BusinessUnitINN { get; set; }
    
    public string BusinessUnitKPP { get; set; }
    
    public double? TotalAmount { get; set; }
    
    public string RegistrationNumber { get; set; }
    
    public string RegistrationDate { get; set; }
    
    public string Currency { get; set; }
    
    public string DocumentBody { get; set; }
  }
  
  #endregion

  #region Результаты интеграции.
  
  /// <summary>
  /// Структура сообщения успешного создания/обновления сущности из 1С.
  /// </summary>
  partial class ResultMesageFromRabbitMQ
  {
    public string message_id { get; set; }
    
    public string message_date { get; set; }
    
    public string correlation_id { get; set; }
    
    public string event_name { get; set; }
    
    public aeon.Integration1C.Structures.Module.ResultUpdateEntityFrom1C body { get; set; }
  }
  
  /// <summary>
  /// Структура Json успешного создания/обновления сущности из 1С.
  /// </summary>
  partial class ResultUpdateEntityFrom1C
  {
    public int Id { get; set; }
    
    public string Guid { get; set; }
    
    public bool Verification { get; set; }
    
    public string ErrorText { get; set; }
  }

  /// <summary>
  /// Структура сообщения ошибки создания/обновления сущности из 1С.
  /// </summary>
  partial class ErrorMesageFromRabbitMQ
  {
    public string message_id { get; set; }
    
    public string message_date { get; set; }
    
    public string correlation_id { get; set; }
    
    public string event_name { get; set; }
    
    public aeon.Integration1C.Structures.Module.ErrorUpdateEntityFrom1C body { get; set; }
  }
  
  /// <summary>
  /// Структура Json ошибки создания/обновления сущности из 1С.
  /// </summary>
  partial class ErrorUpdateEntityFrom1C
  {
    public bool Verification { get; set; }
    
    public string ErrorText { get; set; }
  }
  
  #endregion
  
  #region Отправка Договорных документов в 1С.
  
  /// <summary>
  /// Структура сообщения отправки Договорного документа.
  /// </summary>
  partial class ContractualDocMessageSending
  {
    public string message_id { get; set; }
    
    public string message_date { get; set; }
    
    public string correlation_id { get; set; }
    
    public string event_name { get; set; }
    
    public aeon.Integration1C.Structures.Module.ContractualDocDescriptionSending body { get; set; }
  }
  
  /// <summary>
  /// Структура Json отправляемых свойств Договорного документа.
  /// </summary>
  partial class ContractualDocDescriptionSending
  {
    public int Id { get; set; }
    
    public int? DocumentKind { get; set; }
    
    public int? ContractCategory { get; set; }
    
    public string BusinessUnitINN { get; set; }
    
    public string BusinessUnitKPP { get; set; }
    
    public string Counterparty { get; set; }
    
    public string ResponsibleEmployee { get; set; }
    
    public string ValidFrom { get; set; }
    
    public string ValidTill { get; set; }
    
    public double? TotalAmount { get; set; }
    
    public string Currency { get; set; }
    
    public string RegistrationNumber { get; set; }
    
    public string RegistrationDate { get; set; }
    
    public string LifeCycleState { get; set; }
    
    public string InternalApprovalState { get; set; }
    
    public string ExternalApprovalState { get; set; }
    
    public double? Percentage { get; set; }
    
    public string ContractGuid { get; set; }
    
    public string Guid { get; set; }
  }
  
  #endregion
  
  #region Получение Договорных документов из 1С.
  
  /// <summary>
  /// Структура сообщения получения Договорного документа.
  /// </summary>
  partial class ContractualDocMessageGetting
  {
    public string message_id { get; set; }
    
    public string message_date { get; set; }
    
    public string correlation_id { get; set; }
    
    public string event_name { get; set; }
    
    public aeon.Integration1C.Structures.Module.ContractualDocDescriptionGetting body { get; set; }
  }
  
  /// <summary>
  /// Структура Json принимаемых свойств Договорного документа.
  /// </summary>
  partial class ContractualDocDescriptionGetting
  {
    public int? DocumentKind { get; set; }
    
    public int? ContractCategory { get; set; }
    
    public string BusinessUnitINN { get; set; }
    
    public string BusinessUnitKPP { get; set;}
    
    public string Counterparty { get; set; }
    
    public string ResponsibleEmployee { get; set; }
    
    public string ValidFrom { get; set; }
    
    public string ValidTill { get; set; }
    
    public double? TotalAmount { get; set; }
    
    public string Currency { get; set; }
    
    public string RegistrationNumber { get; set; }
    
    public string RegistrationDate { get; set; }
    
    public string LifeCycleState { get; set; }
    
    public string Status { get; set; }
    
    public double? Percentage { get; set; }
    
    public string Guid { get; set; }
    
    public string Id { get; set; }
  }
  
  /// <summary>
  /// Сопоставление полей Состояние и Согласование для договорного документа.
  /// </summary>
  partial class StateForContractualDoc
  {
    public Sungero.Core.Enumeration? LifeCycleState { get; set; }
    
    public Sungero.Core.Enumeration? InternalApprovalState { get; set; }
    
    public Sungero.Core.Enumeration? ExternalApprovalState { get; set; }
  }
  
  #endregion
  
  #region Отправка Организации в 1С.
  
  /// <summary>
  /// Структура сообщения отправки Организации.
  /// </summary>
  partial class CompanyMessageSending
  {
    public string message_id { get; set; }
    
    public string message_date { get; set; }
    
    public string correlation_id { get; set; }
    
    public string event_name { get; set; }
    
    public aeon.Integration1C.Structures.Module.CompanyDescriptionSending body { get; set; }
  }
  
  /// <summary>
  /// Структура Json отправляемых свойств Организации.
  /// </summary>
  partial class CompanyDescriptionSending
  {
    public string TIN { get; set; }
    
    public string TRRC { get; set; }
    
    public string Id { get; set; }
  }
  
  #endregion

  #region Получение Организации из 1С.
  
  /// <summary>
  /// Структура сообщения получения Организации.
  /// </summary>
  partial class CompanyMessageGetting
  {
    public string message_id { get; set; }
    
    public string message_date { get; set; }
    
    public string correlation_id { get; set; }
    
    public string event_name { get; set; }
    
    public aeon.Integration1C.Structures.Module.CompanyDescriptionGetting body { get; set; }
  }
  
  /// <summary>
  /// Структура Json принимаемых свойств Организации.
  /// </summary>
  partial class CompanyDescriptionGetting
  {
    public string Name { get; set; }
    
    public string LegalName { get; set; }
    
    public string TIN { get; set; }
    
    public bool Nonresident { get; set; }
    
    public string RegNumber { get; set; }
    
    public string LegalAddress { get; set; }
    
    public string PostalAddress { get; set; }
    
    public string Phones { get; set; }
    
    public string Email { get; set; }
    
    public string Note { get; set; }
    
    public string PSRN { get; set; }
    
    public string NCEO { get; set; }
    
    public string NCEA { get; set; }
    
    public string Account { get; set; }
    
    public string Code { get; set; }
    
    public string TRRC { get; set; }
    
    public string City { get; set; }
    
    public string Region { get; set; }
    
    public string Bank { get; set; }
    
    public string Guid { get; set; }
    
    public string Type { get; set; }
    
    public string RegCountry { get; set; }
    
    public string CountryCode { get; set; }
    
    public bool Label { get; set; }
    
    public string Id { get; set; }
  }
  
  #endregion

}