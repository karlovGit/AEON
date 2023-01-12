using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.AEOHSolution.ContractualDocument;

namespace aeon.AEOHSolution
{
  partial class ContractualDocumentServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      
      #region Изменение свойств для интеграции с 1С.
      
      if ((_obj.State.IsInserted && string.IsNullOrEmpty(_obj.Guid1C)) || (!Users.Current.IsSystem.GetValueOrDefault() && _obj.State.IsChanged))
      {
        _obj.IsMustBeSent1C = true;
        _obj.IsSuccessfullyCreated1C = false;
      }
      
      if (_obj.State.Properties.IsSuccessfullyCreated1C.IsChanged)
        _obj.CorrelationId = null;
      
      #endregion
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      _obj.IsMustBeSent1C = false;
      _obj.IsSuccessfullyCreated1C = false;
    }
  }

}