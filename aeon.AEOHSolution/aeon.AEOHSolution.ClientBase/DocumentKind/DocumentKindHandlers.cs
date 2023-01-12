using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.AEOHSolution.DocumentKind;

namespace aeon.AEOHSolution
{
  partial class DocumentKindClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      _obj.State.Properties.LoanAgreement.IsVisible = _obj.DocumentType != null 
        && (_obj.DocumentType.DocumentTypeGuid.Contains("f37c7e63-b134-4446-9b5b-f8811f6c9666") || _obj.DocumentType.DocumentTypeGuid.Contains("265f2c57-6a8a-4a15-833b-ca00e8047fa5"));
    }

  }
}