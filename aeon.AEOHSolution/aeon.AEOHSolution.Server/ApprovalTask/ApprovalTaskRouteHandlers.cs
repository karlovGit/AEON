using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using aeon.AEOHSolution.ApprovalTask;

namespace aeon.AEOHSolution.Server
{
  partial class ApprovalTaskRouteHandlers
  {

    public override void StartBlock6(Sungero.Docflow.Server.ApprovalAssignmentArguments e)
    {
      base.StartBlock6(e);
      
      if (e.Block.Performers.Any())
      {
        var listPerformers = e.Block.Performers.ToList();
        
        foreach (var performer in listPerformers)
        {
          var hasAssignment = Sungero.Docflow.ApprovalAssignments.GetAll(x => x.Task == _obj && x.Performer == performer && x.Completed.HasValue).Any();
          var hasManagerAssignment = Sungero.Docflow.ApprovalManagerAssignments.GetAll(x => x.Task == _obj && x.Performer == performer && x.Completed.HasValue && x.Created > _obj.Started).Any();
          
          if (hasAssignment || hasManagerAssignment)
            e.Block.Performers.Remove(performer);
        }
      }
    }

  }
}