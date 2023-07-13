using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using System.IO;

namespace aeon.Integration1C.Client
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Открыть
    /// </summary>
    public virtual void OpenBodyDocument()
    {
      var dialog = Dialogs.CreateInputDialog("Open Body document");
      var document = dialog.AddSelect("Document", true, Sungero.Docflow.OfficialDocuments.Null);
      if (dialog.Show() == DialogButtons.Ok)
      {
        var version = document.Value.LastVersion.Body;
        var filePath = Path.Combine(Path.GetTempPath(), string.Format("{0}.{1}", document.Value.Id, "xml"));
        Logger.Error(filePath);
        using (var stream = version.Read())
        {
          using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
          {
            stream.CopyTo(fileStream);
          }
        }
      }
    }
    
    /// <summary>
    /// Открыть запись справочника Настройки интеграции.
    /// </summary>
    public virtual void ShowSettingsIntegration()
    {
      SettingsIntegrations.GetAll().FirstOrDefault().Show();
    }

  }
}