using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.AEOHSolution.BusinessUnit;

namespace aeon.AEOHSolution
{
  partial class BusinessUnitServerHandlers
  {
    
   
   
    public override void Saving(Sungero.Domain.SavingEventArgs e)
    {
      //base.Saving(e);
      #region Синхронизировать свойства нашей организации с организацией

      // Создать организацию, соответствующую нашей организации.
      var company = _obj.Company;
      if (company == null)
      {
        company = Sungero.Parties.Companies.Create();
        _obj.Company = company;
      }

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

      if (_obj.HeadCompany != null)
        company.HeadCompany = _obj.HeadCompany.Company;
      else
        company.HeadCompany = null;

      company.Note = string.Format("{0}{1}", BusinessUnits.Resources.BusinessUnitComment, _obj.Note);
      company.IsCardReadOnly = true;
      
      #endregion
      
      // Синхронизировать с ролью "Руководители наших организаций".
      // Functions.BusinessUnit.SynchronizeCEOInRole(_obj);
      
      // Создать или обновить права подписи у руководителя.
      //Functions.BusinessUnit.UpdateSignatureSettings(_obj);
    }

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      
      var role = Roles.GetAll().FirstOrDefault(x => x.Sid == aeon.CustomM.PublicConstants.Module.ChiefAccountantsOurOrgs);
      if (role != null)
      {
        if (_obj.ChiefAccountant != null && !role.RecipientLinks.Any(r => Equals(r.Member, _obj.ChiefAccountant)))
          role.RecipientLinks.AddNew().Member = _obj.ChiefAccountant;
      }
    }
  }

}