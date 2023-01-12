using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.AEOHSolution.SupAgreement;

namespace aeon.AEOHSolution
{
  partial class SupAgreementClientHandlers
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

  }
}