using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.Integration1C.SettingsIntegration;

namespace aeon.Integration1C.Server
{
  partial class SettingsIntegrationFunctions
  {

    /// <summary>
    /// Фильтрация видов документов для финансовых документов.
    /// </summary>
    /// <param name="query">Виды документов.</param>
    /// <returns>Отфильтрованные виды документов.</returns>
    public static IQueryable<Sungero.Docflow.IDocumentKind> FilterFormalizedDocKinds(IQueryable<Sungero.Docflow.IDocumentKind> query)
    {
      return query.Where(k => k.DocumentType != null && (k.DocumentType.DocumentTypeGuid == Constants.Module.FormalizedDocTypeGuids.ContractStatementInvoiceGuid ||
                                                         k.DocumentType.DocumentTypeGuid == Constants.Module.FormalizedDocTypeGuids.UniversalTransferDocumentGuid ||
                                                         k.DocumentType.DocumentTypeGuid == Sungero.Docflow.PublicConstants.AccountingDocumentBase.OutcomingTaxInvoiceGuid));
    }

    /// <summary>
    /// Получить настройки интеграции.
    /// </summary>
    /// <returns>Настройки интеграции.</returns>
    [Public]
    public static ISettingsIntegration GetSettingsIntegration()
    {
      var settingsIntegration = SettingsIntegrations.Null;
      AccessRights.AllowRead(
        () =>
        {
          settingsIntegration = SettingsIntegrations.GetAll().FirstOrDefault();
        });
      
      return settingsIntegration;
    }

  }
}