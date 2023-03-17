using System;
using Sungero.Core;

namespace aeon.Integration1C.Constants
{
  public static class Module
  {

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