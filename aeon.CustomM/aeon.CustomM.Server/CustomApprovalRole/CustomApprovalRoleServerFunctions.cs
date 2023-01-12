using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.CustomM.CustomApprovalRole;

namespace aeon.CustomM.Server
{
  partial class CustomApprovalRoleFunctions
  {
    public override Sungero.Company.IEmployee GetRolePerformer(Sungero.Docflow.IApprovalTask task)
    {
      // Бухгалтер компании А.
      if (_obj.Type == aeon.CustomM.CustomApprovalRole.Type.CompanyAAccount)
      {
        var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
        
        var contractualDocument = Sungero.Contracts.ContractualDocuments.As(document);
        var contractStatement = Sungero.FinancialArchive.ContractStatements.As(document);
        
        var businessUnit = contractualDocument != null ? contractualDocument.BusinessUnit :
          (contractStatement != null ? contractStatement.BusinessUnit : null);
        
        if (businessUnit != null )
          return aeon.AEOHSolution.BusinessUnits.As(businessUnit).ChiefAccountant;
        
        return null;
      }

      // Бухгалтер компании Б.
      if (_obj.Type == aeon.CustomM.CustomApprovalRole.Type.CompanyBAccount)
      {
        var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
        
        var contractualDocument = Sungero.Contracts.ContractualDocuments.As(document);
        var contractStatement = Sungero.FinancialArchive.ContractStatements.As(document);
        
        var counterparty = contractualDocument != null ? contractualDocument.Counterparty :
          (contractStatement != null ? contractStatement.Counterparty : null);
        
        if (counterparty != null)
        {
          var businessUnit = aeon.AEOHSolution.BusinessUnits.GetAll(x => x.Company != null && x.Company.Id == counterparty.Id).FirstOrDefault();
          
          if (businessUnit != null)
            return businessUnit.ChiefAccountant;
        }
        
        return null;
      }
      
      return base.GetRolePerformer(task);
    }
  }
}