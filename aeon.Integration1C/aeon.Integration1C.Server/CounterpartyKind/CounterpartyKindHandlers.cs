using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.Integration1C.CounterpartyKind;

namespace aeon.Integration1C
{
  partial class CounterpartyKindServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.IsMissingTRRC = false;
    }
  }

}