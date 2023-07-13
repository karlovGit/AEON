using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.AEOHSolution.AccountingDocumentBase;

namespace aeon.AEOHSolution
{
  partial class AccountingDocumentBaseClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      var currentUser = Users.Current;
      _obj.State.Properties.Guid1C.IsEnabled = currentUser.IncludedIn(Roles.Administrators) || currentUser.IncludedIn(aeon.Integration1C.PublicConstants.Module.ResponsibleForIntegration1CRoleGuid);
    }

  }
}