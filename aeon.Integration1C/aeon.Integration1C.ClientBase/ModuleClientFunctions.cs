using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace aeon.Integration1C.Client
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Открыть запись справочника Настройки интеграции.
    /// </summary>
    public virtual void ShowSettingsIntegration()
    {
      SettingsIntegrations.GetAll().FirstOrDefault().Show();
    }

  }
}