using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.AEOHSolution.BusinessUnit;

namespace aeon.AEOHSolution.Client
{
  partial class BusinessUnitActions
  {
    public virtual void CloseNOR(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      
      //меняем readonly в карточке контрагента
      var contragent =  _obj.Company;
      contragent.IsCardReadOnly = false;
      contragent.Note = null;
      contragent.Save();
      //создаем новую организацию с закрытым статусом и проставляем ссылку на новую организацию для НО
      aeon.AEOHSolution.PublicFunctions.BusinessUnit.CreateNewCompany(_obj);
      _obj.Status = Status.Closed;
      foreach (var prop in _obj.State.Properties)
      {
        prop.IsEnabled = false;
      }
      
      _obj.Save();
      e.AddWarning("Наша организация закрыта");
      
      
    }

    public virtual bool CanCloseNOR(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return  aeon.AEOHSolution.PublicFunctions.BusinessUnit.CheckCloseNORAbility(_obj);
    }

  }

}