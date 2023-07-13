using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using aeon.CustomM.TaskNoteApproval;

namespace aeon.CustomM.Server
{
  partial class TaskNoteApprovalRouteHandlers
  {

    public virtual void StartBlock4(aeon.CustomM.Server.NoticeArguments e)
    {
      // Subject для каждого результата свой.
      if (_obj.Result == aeon.CustomM.TaskNoteApproval.Result.Approved)
      {
        e.Block.Subject = Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(aeon.AEOHSolution.ApprovalTasks.Resources.DocAgreed,
                                                                                    _obj.DocumentGroup.OfficialDocuments.First().Name, _obj.AgreedBy.Name);
      }
      else if (_obj.Result == aeon.CustomM.TaskNoteApproval.Result.ForRevision)
      {
        e.Block.Subject = Sungero.Docflow.PublicFunctions.Module.TrimSpecialSymbols(aeon.AEOHSolution.ApprovalTasks.Resources.DocumentSentRevision,
                                                                                    _obj.DocumentGroup.OfficialDocuments.First().Name);
      }
      
      e.Block.Performers.Add(_obj.Assignee);
    }

    public virtual void StartBlock3(aeon.CustomM.Server.NoticeArguments e)
    {
      
    }
  }

}