using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.AEOHSolution.OutgoingLetter;

namespace aeon.AEOHSolution.Shared
{
  partial class OutgoingLetterFunctions
  {
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();
      
      var isVisualMode = ((Sungero.Domain.Shared.IExtendedEntity)_obj).Params.ContainsKey(Sungero.Docflow.PublicConstants.OfficialDocument.IsVisualModeParamName);

      if (isVisualMode)
        _obj.State.Properties.Stamping.IsRequired = true;
    }
  }
}