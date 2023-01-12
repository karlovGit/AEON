using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.AEOHSolution.ApprovalStage;

namespace aeon.AEOHSolution.Shared
{
  partial class ApprovalStageFunctions
  {
    public override List<Enumeration?> GetPossibleRoles()
    {
      var baseRoles = base.GetPossibleRoles();

      if (_obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Approvers || _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.SimpleAgr ||
          _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Notice)
      {
        baseRoles.Add(aeon.CustomM.CustomApprovalRole.Type.CompanyAAccount);
        baseRoles.Add(aeon.CustomM.CustomApprovalRole.Type.CompanyBAccount);
      }

      return baseRoles;
    }
  }
}