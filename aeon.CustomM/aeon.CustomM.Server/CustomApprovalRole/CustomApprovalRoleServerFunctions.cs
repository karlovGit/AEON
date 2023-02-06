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
      
      // Согласование с вышестоящим руководителем.
      if (_obj.Type == aeon.CustomM.CustomApprovalRole.Type.ManagerInit)
      {
        var department = GetDepartment(task.Author);
        return GetDepartmentManager(department);
      }
      
      return base.GetRolePerformer(task);
    }
    
    /// <summary>
    /// Получить руководителя подразделения c проверкой вышестоящего подразделения.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <returns>Сотрудник.</returns>
    private Sungero.Company.IEmployee GetDepartmentManager(Sungero.Company.IDepartment department)
    {
      if (department == null)
        return null;
      
      if (department.Manager != null)
        return department.Manager;
      
      return GetDepartmentManager(department.HeadOffice);
    }
    
    
    // Получить список исполнителей из роли.
    // <param name="task">Задача.</param>
    // <returns>Список исполнителей.</returns>
    [Remote(IsPure = true), Public]
    public virtual List<Sungero.CoreEntities.IRecipient> GetRolePerformersN(Sungero.Docflow.IApprovalTask task)
    {
      var result = new List<Sungero.CoreEntities.IRecipient>();
      
      if (_obj.Type == aeon.CustomM.CustomApprovalRole.Type.ManagersInit)
      {
        var department = GetDepartment(task.Author);
        var firstManager = GetDepartmentManager(department);
        
        if (firstManager != null)
        {
          result = GetDepartmentManagers(department, result);
          result.Remove(firstManager);
        }
      }
      
      return result;
    }
    
    public static Sungero.Company.IDepartment GetDepartment(IUser user)
    {
      var employee = Sungero.Company.Employees.GetAll().FirstOrDefault(u => Equals(u, user));
      
      if (employee == null)
        return null;

      return employee.Department;
    }
    
    /// <summary>
    /// Получить руководителя подразделения инициатора согласования.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <returns>Сотрудник.</returns>
    private List<Sungero.CoreEntities.IRecipient> GetDepartmentManagers(Sungero.Company.IDepartment department, List<Sungero.CoreEntities.IRecipient> result)
    {
      if (department == null)
        return result;
      
      if (department.Manager != null)
        result.Add(department.Manager);
      
      return GetDepartmentManagers(department.HeadOffice, result);
    }
    
  }
}