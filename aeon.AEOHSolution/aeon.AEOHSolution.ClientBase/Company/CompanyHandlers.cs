using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.AEOHSolution.Company;

namespace aeon.AEOHSolution
{
  partial class CompanyClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      var currentUser = Users.Current;
      var isAdministrator = currentUser.IncludedIn(Roles.Administrators);
      if (!isAdministrator)
      {
        var isCounterpartiesResponsible = currentUser.IncludedIn(Sungero.Docflow.PublicConstants.Module.RoleGuid.CounterpartiesResponsibleRole);
        var isResponsibleForIntegration1C = currentUser.IncludedIn(aeon.Integration1C.PublicConstants.Module.ResponsibleForIntegration1CRoleGuid);
        var isClerks = currentUser.IncludedIn(Sungero.Docflow.PublicConstants.Module.RoleGuid.ClerksRole);
        
        _obj.State.Properties.Guid1C.IsEnabled = isResponsibleForIntegration1C;
        _obj.State.Properties.IsForOffice.IsEnabled = isClerks || isCounterpartiesResponsible;
        _obj.State.Properties.CounterpartyKind.IsEnabled = isCounterpartiesResponsible || isResponsibleForIntegration1C;
      }
      
      Functions.Company.IsRequiredTINAndTRRC(_obj, _obj.IsForOffice.GetValueOrDefault(), _obj.Nonresident.GetValueOrDefault());
    }
  }

}