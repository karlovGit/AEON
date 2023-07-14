using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.AEOHSolution.Company;

namespace aeon.AEOHSolution.Server
{
  partial class CompanyFunctions
  {

    [Public]
    public bool NORNotFound()
    {
      return !aeon.AEOHSolution.BusinessUnits.GetAll().Any(x=>(x.TIN==_obj.TIN && x.TRRC==_obj.TRRC) || (x.TIN==_obj.TIN));
    }
    /// <summary>
    /// Создать НОР
    /// </summary>
    [Public]
    public void SCreateNOR()
    {
        //Закрываем старого контрагента:
        _obj.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;
        _obj.Save();
        //Создаем новую НОР:
        var NOR =  aeon.AEOHSolution.BusinessUnits.Create();
        NOR.Name = _obj.Name;
        NOR.LegalName = _obj.LegalName;
        NOR.Region = _obj.Region;
        NOR.TIN = _obj.TIN;
        NOR.TRRC = _obj.TRRC;
        //NOR.Company = _obj;
        NOR.Save();
      
    }

  }
}