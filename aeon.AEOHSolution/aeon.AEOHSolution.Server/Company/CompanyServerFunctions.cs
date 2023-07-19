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
      
      #region старый
     // company.Nonresident = _obj.Nonresident;
     // company.Name = _obj.Name;
     // company.TIN = _obj.TIN;
     // company.TRRC = _obj.TRRC;
     // company.PSRN = _obj.PSRN;
     // company.NCEO = _obj.NCEO;
     // company.NCEA = _obj.NCEA;
     // company.City = _obj.City;
     // company.Phones = _obj.Phones;
     // company.LegalName = _obj.LegalName;
     // company.Region = _obj.Region;
     // company.LegalAddress = _obj.LegalAddress;
     // company.PostalAddress = _obj.PostalAddress;
     // company.Status = _obj.Status;
     // company.Email = _obj.Email;
     // company.Homepage = _obj.Homepage;
     // company.Account = _obj.Account;
     // company.Bank = _obj.Bank;
     // company.Code = _obj.Code;
      #endregion
        //Закрываем старого контрагента:
       //_obj.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;
       //_obj.Save();
        //Создаем новую НОР:
        var NOR =  aeon.AEOHSolution.BusinessUnits.Create();
        NOR.Name = _obj.Name;
        NOR.LegalName = _obj.LegalName;
        NOR.Region = _obj.Region;
        NOR.TIN = _obj.TIN;
        NOR.TRRC = _obj.TRRC;
        NOR.Company = _obj;
        NOR.Save();
      
    }

  }
}