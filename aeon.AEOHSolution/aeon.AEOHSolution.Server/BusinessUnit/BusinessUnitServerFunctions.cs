using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.AEOHSolution.BusinessUnit;

namespace aeon.AEOHSolution.Server
{
  partial class BusinessUnitFunctions
  {

    /// <summary>
    /// 
    /// </summary>
    [Public,Remote]
    public override void  SynchronizeCEOInRole()
    {
      base.SynchronizeCEOInRole();
    }
    
    [Public,Remote]
    public override void UpdateSignatureSettings()
    {
      base.UpdateSignatureSettings();
    }

  }
}