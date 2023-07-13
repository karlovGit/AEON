using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.Integration1C.SettingsIntegration;

namespace aeon.Integration1C
{
  partial class SettingsIntegrationExternalWithNonResidentDocumentsDocumentKindPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ExternalWithNonResidentDocumentsDocumentKindFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return Functions.SettingsIntegration.FilterFormalizedDocKinds(query).Cast<T>();
    }
  }

  partial class SettingsIntegrationExternalWithResidentDocumentsDocumentKindPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ExternalWithResidentDocumentsDocumentKindFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return Functions.SettingsIntegration.FilterFormalizedDocKinds(query).Cast<T>();
    }
  }

  partial class SettingsIntegrationIntragroupDocumentsDocumentKindPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> IntragroupDocumentsDocumentKindFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return Functions.SettingsIntegration.FilterFormalizedDocKinds(query).Cast<T>();
    }
  }

  partial class SettingsIntegrationContractsAccountDocumentKindPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ContractsAccountDocumentKindFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(k => k.DocumentFlow == AEOHSolution.DocumentKind.DocumentFlow.Contracts);
    }
  }

}