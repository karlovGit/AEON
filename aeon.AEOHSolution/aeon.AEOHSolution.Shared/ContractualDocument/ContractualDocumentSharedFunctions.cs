using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.AEOHSolution.ContractualDocument;

namespace aeon.AEOHSolution.Shared
{
  partial class ContractualDocumentFunctions
  {
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();
      
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