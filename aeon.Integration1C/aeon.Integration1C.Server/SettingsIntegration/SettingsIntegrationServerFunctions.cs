using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.Integration1C.SettingsIntegration;

namespace aeon.Integration1C.Server
{
  partial class SettingsIntegrationFunctions
  {

    /// <summary>
    /// Получить настройки интеграции.
    /// </summary>
    /// <returns>Настройки интеграции.</returns>
    [Public]
    public static ISettingsIntegration GetSettingsIntegration()
    {
      return SettingsIntegrations.GetAll().FirstOrDefault();
    }

  }
}