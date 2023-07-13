using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.AEOHSolution.ApprovalStage;

namespace aeon.AEOHSolution
{
  partial class ApprovalStageServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.SkipRenegotiation = false;
      _obj.ChangeSequence = false;
      _obj.NotifyApprovers = false;
    }
  }

}