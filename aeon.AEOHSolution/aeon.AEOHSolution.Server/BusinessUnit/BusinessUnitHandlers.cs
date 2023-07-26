using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.AEOHSolution.BusinessUnit;

namespace aeon.AEOHSolution
{
  partial class BusinessUnitServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      
      
      base.BeforeSave(e);
      
      var role = Roles.GetAll().FirstOrDefault(x => x.Sid == aeon.CustomM.PublicConstants.Module.ChiefAccountantsOurOrgs);
      if (role != null)
      {
        if (_obj.ChiefAccountant != null && !role.RecipientLinks.Any(r => Equals(r.Member, _obj.ChiefAccountant)))
          role.RecipientLinks.AddNew().Member = _obj.ChiefAccountant;
      }
    
      if (_obj.Status.Equals(Status.Closed))
      {
        foreach (var prop in _obj.State.Properties)
        {
          prop.IsEnabled = false;
        }
      }
      
     
    }
  }

}