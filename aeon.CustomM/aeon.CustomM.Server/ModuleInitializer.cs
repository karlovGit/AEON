using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace aeon.CustomM.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      CreateRoles();
      
      CreateApprovalRole(aeon.CustomM.CustomApprovalRole.Type.CompanyAAccount, "Бухгалтер компании А");
      CreateApprovalRole(aeon.CustomM.CustomApprovalRole.Type.CompanyBAccount, "Бухгалтер компании Б");
      CreateApprovalRole(aeon.CustomM.CustomApprovalRole.Type.ManagerInit, "Согласование с вышестоящим руководителем");
      CreateApprovalRole(aeon.CustomM.CustomApprovalRole.Type.ManagersInit, "Согласование с вышестоящим руководителем (множественное)");
      CreateApprovalRole(aeon.CustomM.CustomApprovalRole.Type.ActualSignatory, "Фактический подписант");
      CreateApprovalRole(aeon.CustomM.CustomApprovalRole.Type.HeadOfNOR, "Руководитель НОР");
    }
    
    /// <summary>
    /// Создать предопределенные роли.
    /// </summary>
    public static void CreateRoles()
    {
      InitializationLogger.Debug("Init: Create Default Roles");
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(aeon.CustomM.Resources.ChiefAccountantsOurOrgsName,
                                                                      aeon.CustomM.Resources.ChiefAccountantsOurOrgsName,
                                                                      Constants.Module.ChiefAccountantsOurOrgs);
      
      // Исключения из Согласование с вышестоящим руководителем (множественное).
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(aeon.CustomM.Resources.ExceptionsToSupervisoryRole,
                                                                      aeon.CustomM.Resources.DescriptionExceptionsToSupervisoryRole,
                                                                      Constants.Module.ExceptionsToSupervisory);
    }
    
    /// <summary>
    /// Создание роли.
    /// </summary>
    public static void CreateApprovalRole(Enumeration roleType, string description)
    {
      var role = CustomApprovalRoles.GetAll().Where(r => Equals(r.Type, roleType)).FirstOrDefault();
      // Проверяет наличие роли.
      if (role == null)
      {
        role = CustomApprovalRoles.Create();
        role.Type = roleType;
      }
      
      role.Description = description;
      role.Save();
    }
    
  }
}
