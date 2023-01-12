using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.AEOHSolution.Contract;

namespace aeon.AEOHSolution
{
  partial class ContractClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      if (_obj.DocumentKind != null)
      {
        var isLoanAgreement = aeon.AEOHSolution.DocumentKinds.As(_obj.DocumentKind).LoanAgreement.GetValueOrDefault();
        
        _obj.State.Properties.PercentagePerAnnum.IsVisible = isLoanAgreement;
        _obj.State.Properties.TransferDeadlineUp.IsVisible = isLoanAgreement;
        _obj.State.Properties.ReturnNoLaterThan.IsVisible = isLoanAgreement;
      }
    }

    public override void DocumentKindValueInput(Sungero.Docflow.Client.OfficialDocumentDocumentKindValueInputEventArgs e)
    {
      base.DocumentKindValueInput(e);
      
      if (e.NewValue != null && !aeon.AEOHSolution.DocumentKinds.As(e.NewValue).LoanAgreement.GetValueOrDefault())
      {
        _obj.PercentagePerAnnum = null;
        _obj.TransferDeadlineUp = null;
        _obj.ReturnNoLaterThan = null;
      }
    }

  }
}