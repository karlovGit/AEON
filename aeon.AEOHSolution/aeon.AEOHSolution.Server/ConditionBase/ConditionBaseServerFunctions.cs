using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.AEOHSolution.ConditionBase;

namespace aeon.AEOHSolution.Server
{
  partial class ConditionBaseFunctions
  {
    public override string GetConditionName()
    {
      using (TenantInfo.Culture.SwitchTo())
      {
        if (_obj.ConditionType == ConditionType.Stamping)
          return aeon.AEOHSolution.ConditionBases.Resources.Stamping;
        
        if (_obj.ConditionType == ConditionType.LeaderInvolved)
          return "Руководитель участвует в согласовании?";
      }
      
      return base.GetConditionName();
    }
  }
}