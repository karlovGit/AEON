using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.AEOHSolution.DocumentKind;

namespace aeon.AEOHSolution
{
  partial class DocumentKindServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.LoanAgreement = false;
    }
  }

}