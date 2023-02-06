using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.AEOHSolution.SupAgreement;

namespace aeon.AEOHSolution
{
  partial class SupAgreementSharedHandlers
  {

    public override void LeadingDocumentChanged(Sungero.Docflow.Shared.OfficialDocumentLeadingDocumentChangedEventArgs e)
    {
      base.LeadingDocumentChanged(e);
      
      if (e.NewValue != e.OldValue)
      {
        _obj.PercentagePerAnnum = null;
        _obj.TransferDeadlineUp = null;
        _obj.ReturnNoLaterThan = null;
      }
    }
  }
}