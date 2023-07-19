using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.AEOHSolution.BusinessUnit;

namespace aeon.AEOHSolution.Server
{
  partial class BusinessUnitFunctions
  {

    /// <summary>
    /// 
    /// </summary>
    [Public]
    public virtual void CreateNewCompany()
    {
      var newCompany =  Sungero.Parties.Companies.Create();
      newCompany.Nonresident = _obj.Nonresident;
      newCompany.Name = _obj.Name;
      newCompany.TIN = _obj.TIN;
      newCompany.TRRC = _obj.TRRC;
      newCompany.PSRN = _obj.PSRN;
      newCompany.NCEO = _obj.NCEO;
      newCompany.NCEA = _obj.NCEA;
      newCompany.City = _obj.City;
      newCompany.Phones = _obj.Phones;
      newCompany.LegalName = _obj.LegalName;
      newCompany.Region = _obj.Region;
      newCompany.LegalAddress = _obj.LegalAddress;
      newCompany.PostalAddress = _obj.PostalAddress;
      newCompany.Email = _obj.Email;
      newCompany.Homepage = _obj.Homepage;
      newCompany.Account = _obj.Account;
      newCompany.Bank = _obj.Bank;
      newCompany.Code = _obj.Code;
      
      
      newCompany.Save();
      _obj.Company = newCompany;
      _obj.Save();
      newCompany.Status = Status.Closed;
      newCompany.IsCardReadOnly = true;
      newCompany.Save();
      
    }

    /// <summary>
    /// 
    /// </summary>
    [Public,Remote]
    public override void  SynchronizeCEOInRole()
    {
      base.SynchronizeCEOInRole();
    }
    
    [Public,Remote]
    public override void UpdateSignatureSettings()
    {
      base.UpdateSignatureSettings();
    }

  }
}