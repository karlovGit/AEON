using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.AEOHSolution.Company;

namespace aeon.AEOHSolution
{
  partial class CompanyServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      
      #region Изменение свойств для интеграции с 1С.
      
      if (_obj.State.IsInserted && !_obj.IsCardReadOnly.GetValueOrDefault() && string.IsNullOrEmpty(_obj.Guid1C))
        _obj.IsMustBeSent1C = true;
      
      if (_obj.State.Properties.IsSuccessfullyCreated1C.IsChanged && _obj.IsSuccessfullyCreated1C.GetValueOrDefault())
        _obj.CorrelationId = null;
      
      #endregion
      
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      _obj.IsMustBeSent1C = false;
      _obj.IsSuccessfullyCreated1C = false;
      _obj.IsForOffice = false;
    }
  }

}