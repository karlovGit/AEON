using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.Integration1C.SettingsIntegration;

namespace aeon.Integration1C.Client
{
  partial class SettingsIntegrationActions
  {
    public virtual void SetPassword(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var dialog = Dialogs.CreateInputDialog(aeon.Integration1C.SettingsIntegrations.Resources.SetPasswordDialogName);
      var password = dialog.AddString(_obj.Info.Properties.Password.LocalizedName, true);
      dialog.Buttons.AddOkCancel();
      if (dialog.Show() == DialogButtons.Ok)
      {
        _obj.Password = password.Value.Trim();
        _obj.Save();
      }
    }

    public virtual bool CanSetPassword(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}