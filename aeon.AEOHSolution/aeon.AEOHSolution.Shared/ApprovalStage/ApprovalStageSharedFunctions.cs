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
        baseRoles.Add(aeon.CustomM.CustomApprovalRole.Type.HeadOfNOR);
      }
      
      if (_obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Approvers || _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.SimpleAgr ||
          _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Notice && _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Manager)
      {
        baseRoles.Add(aeon.CustomM.CustomApprovalRole.Type.ManagerInit);
      }
      
      if (_obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Sending)
      {
        baseRoles.Add(aeon.CustomM.CustomApprovalRole.Type.CompanyAAccount);
        baseRoles.Add(aeon.CustomM.CustomApprovalRole.Type.Signatory);
        baseRoles.Add(aeon.CustomM.CustomApprovalRole.Type.ActualSignatory);
      }
      
      if (_obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Sign)
      {
        baseRoles.Add(aeon.CustomM.CustomApprovalRole.Type.ActualSignatory);
      }

      return baseRoles;
    }
    
    /// <summary>
    /// Установить видимость свойств.
    /// </summary>
    public override void SetPropertiesVisibility()
    {
      base.SetPropertiesVisibility();
      
      var isApprovers = _obj.StageType == StageType.Approvers;
      _obj.State.Properties.SkipRenegotiation.IsVisible = isApprovers;
      _obj.State.Properties.ChangeSequence.IsVisible = isApprovers;
      _obj.State.Properties.NotifyApprovers.IsVisible = isApprovers;
    }
    
    public override void SetPropertiesAvailability()
    {
      base.SetPropertiesAvailability();
      
      var isSign = _obj.StageType == StageType.Sign;
      
      if (isSign)
        _obj.State.Properties.ApprovalRole.IsEnabled = true;
    }
  }
}