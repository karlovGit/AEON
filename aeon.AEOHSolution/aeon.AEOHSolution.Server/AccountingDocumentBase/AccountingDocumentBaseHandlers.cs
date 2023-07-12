using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.AEOHSolution.AccountingDocumentBase;

namespace aeon.AEOHSolution
{
  partial class AccountingDocumentBaseServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      bool cuser = Sungero.CoreEntities.Users.Current.IsSystem ?? false;
      
      if(    _obj.LifeCycleState.Value.Value == "Draft"   &&
             _obj.State.Properties.LifeCycleState.IsChanged  && 
//             _obj.Note.Contains("Отправлен через сервис обмена Диадок") &&
              _obj.LocationState.Contains("Отправлен через сервис обмена Диадок") &&
              cuser )
      { 
      var task = Sungero.Workflow.SimpleTasks.Create();
      var step1 = task.RouteSteps.AddNew();
      step1.AssignmentType = Sungero.Workflow.SimpleTask.AssignmentType.Notice;
      step1.Performer = aeon.AEOHSolution.BusinessUnits.Get(_obj.BusinessUnit.Id).ChiefAccountant;    // Задать исполнителя по этапу.
      task.Attachments.Add(_obj);
      task.Subject = "Проверить документ !";
      task.Start();  
      }
        

      }  
    }
  }

