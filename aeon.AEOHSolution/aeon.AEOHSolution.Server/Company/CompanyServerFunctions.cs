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
      return !aeon.AEOHSolution.BusinessUnits.GetAll().Any(x=>((x.TIN==_obj.TIN && x.TRRC==_obj.TRRC) || (x.TIN==_obj.TIN)) );
    }
    
    [Public]
    public bool ClosedNORFound()
    {
      return aeon.AEOHSolution.BusinessUnits.GetAll().Any(x=>((x.TIN==_obj.TIN && x.TRRC==_obj.TRRC) || (x.TIN==_obj.TIN)) && x.Status.Equals(Sungero.CoreEntities.DatabookEntry.Status.Closed));
    }
    
    [Public]
    public void OpenNOR()
    {
     // var norList =  aeon.AEOHSolution.BusinessUnits.GetAll().Where(xx=>xx.Company.Equals(_obj)); //неправильно ищет
      var norList =  aeon.AEOHSolution.BusinessUnits.GetAll().Where(xx=>xx.TIN==_obj.TIN && xx.TRRC==_obj.TRRC && _obj.Status.Equals(Status.Closed));
      if (norList.Any()) 
      {
        var nor = norList.FirstOrDefault();
        nor.Status = Status.Active;
        nor.Save();
      }
      else 
      {
        Logger.Debug("variable norList is empty");
      }
      
    }
    
    //    public bool OpenClosedNOR()
    //    {
    //      return !aeon.AEOHSolution.BusinessUnits.GetAll()
    //    }
    /// <summary>
    /// Создать НОР
    /// </summary>
    [Public]
    public void SCreateNOR()
    {
      var company =  aeon.AEOHSolution.BusinessUnits.Create();
      company.Nonresident = _obj.Nonresident;
      company.Name = _obj.Name;
      company.TIN = _obj.TIN;
      company.TRRC = _obj.TRRC;
      company.PSRN = _obj.PSRN;
      company.NCEO = _obj.NCEO;
      company.NCEA = _obj.NCEA;
      company.City = _obj.City;
      company.Phones = _obj.Phones;
      company.LegalName = _obj.LegalName;
      company.Region = _obj.Region;
      company.LegalAddress = _obj.LegalAddress;
      company.PostalAddress = _obj.PostalAddress;
      company.Status = _obj.Status;
      company.Email = _obj.Email;
      company.Homepage = _obj.Homepage;
      company.Account = _obj.Account;
      company.Bank = _obj.Bank;
      company.Code = _obj.Code;
      company.Company = _obj;
      company.Save();
      // Синхронизировать с ролью "Руководители наших организаций".
      PublicFunctions.BusinessUnit.Remote.SynchronizeCEOInRole(company);
      
      
      // Создать или обновить права подписи у руководителя.
      PublicFunctions.BusinessUnit.Remote.UpdateSignatureSettings(company);
      
      
      
    }

  }
}