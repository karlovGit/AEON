using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace aeon.Integration1C.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      CreateSettingsIntegration();
      CreateCounterpartyKinds();
      CreateRoles();
      GrantRightsOnDatabooks();
      CreateDocumentKinds();
    }

    /// <summary>
    /// Создать запись справочника Настройки интеграции.
    /// </summary>
    public static void CreateSettingsIntegration()
    {
      if (SettingsIntegrations.GetAll().Any())
        return;
      
      var setting = SettingsIntegrations.Create();
      setting.Name = aeon.Integration1C.Resources.SettingsIntegrationName;
      setting.Save();
    }

    /// <summary>
    /// Создать виды контрагента.
    /// </summary>
    public static void CreateCounterpartyKinds()
    {
      if (CounterpartyKinds.GetAll().Any())
        return;
      
      var kindNames = new List<string> { aeon.Integration1C.Resources.LegalEntityCounterpartyKind,
        aeon.Integration1C.Resources.NaturalPersonCounterpartyKind,
        aeon.Integration1C.Resources.SelfEmployedCounterpartyKind,
        aeon.Integration1C.Resources.SeparateSubdivisionCounterpartyKind,
        aeon.Integration1C.Resources.GovernmentBodyCounterpartyKind };
      foreach (var name in kindNames)
      {
        var newKind = CounterpartyKinds.Create();
        newKind.Name = name;
        newKind.Save();
      }
    }

    /// <summary>
    /// Создать предопределенные роли.
    /// </summary>
    public static void CreateRoles()
    {
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(aeon.Integration1C.Resources.ResponsibleFoIntegration1CRoleName,
                                                                      aeon.Integration1C.Resources.ResponsibleFoIntegration1CRoleName,
                                                                      Constants.Module.ResponsibleForIntegration1CRoleGuid);
    }

    /// <summary>
    /// Выдать права на справочники модуля.
    /// </summary>
    public static void GrantRightsOnDatabooks()
    {
      InitializationLogger.Debug("Init: Grant rights on databooks.");
      
      var responsibleForIntegration = Functions.Module.GetResponsibleForIntegration1CRole();
      var counterpartyResponsibleRole = Sungero.Docflow.PublicInitializationFunctions.Module.GetCounterpartyResponsibleRole();
      
      var changeRights = DefaultAccessRightsTypes.Change;
      var readRights = DefaultAccessRightsTypes.Read;
      
      SettingsIntegrations.AccessRights.Grant(responsibleForIntegration, changeRights);
      CounterpartyKinds.AccessRights.Grant(responsibleForIntegration, changeRights);
      CounterpartyKinds.AccessRights.Grant(counterpartyResponsibleRole, readRights);
      
      SettingsIntegrations.AccessRights.Save();
      CounterpartyKinds.AccessRights.Save();
    }

    /// <summary>
    /// Создать виды документов.
    /// </summary>
    public static void CreateDocumentKinds()
    {
      InitializationLogger.Debug("Init: Create document kinds.");
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(aeon.Integration1C.Resources.NonFormalizedSimpleDocumentKindName, aeon.Integration1C.Resources.NonFormalizedSimpleDocumentKindName,
                         Sungero.Docflow.DocumentKind.NumberingType.NotNumerable, Sungero.Docflow.DocumentKind.DocumentFlow.Inner, false, false, Constants.Module.FormalizedDocTypeGuids.SimpleDocumentTypeGuid,
                         new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendActionItem, Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval, Sungero.Docflow.OfficialDocuments.Info.Actions.SendForAcquaintance },
                         Constants.Module.NonFormalizedSimpleDocumentKind);
    }
  }
}
