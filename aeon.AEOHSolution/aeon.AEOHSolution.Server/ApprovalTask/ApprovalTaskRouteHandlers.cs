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
      
      var stage = _obj.ApprovalRule.Stages.Where(s => s.Stage != null)
        .Where(s => s.Stage.StageType == Sungero.Docflow.ApprovalStage.StageType.Approvers)
        .FirstOrDefault(s => s.Number == _obj.StageNumber);
      
      if (stage != null && e.Block.Performers.Any())
      {
        var stageCustom = AEOHSolution.ApprovalStages.As(stage);
        
        if (stageCustom != null && stageCustom.SkipRenegotiation == true)
        {
          var listPerformers = e.Block.Performers.ToList();
          var reworkAssignments = Sungero.Docflow.ApprovalReworkAssignments.GetAll(x => x.Task == _obj).OrderBy(x => x.Created).LastOrDefault();
          
          foreach (var performer in listPerformers)
          {
            var hasAssignment = Sungero.Docflow.ApprovalAssignments.GetAll(x => x.Task == _obj && x.Performer == performer && x.Completed.HasValue
                                                                           && x.Result == Sungero.Docflow.ApprovalAssignment.Result.Approved);
            
            var hasManagerAssignment = Sungero.Docflow.ApprovalManagerAssignments.GetAll(x => x.Task == _obj && x.Performer == performer && x.Completed.HasValue && x.Created > _obj.Started
                                                                                         && x.Result == Sungero.Docflow.ApprovalManagerAssignment.Result.Approved);
            
            if (reworkAssignments != null)
            {
              hasAssignment = Sungero.Docflow.ApprovalAssignments.GetAll(x => x.Completed.HasValue && x.Completed > reworkAssignments.Created);
              
              hasManagerAssignment = Sungero.Docflow.ApprovalManagerAssignments.GetAll(x => x.Completed.HasValue && x.Completed > reworkAssignments.Created);
            }
            
            if (hasAssignment.Any() || hasManagerAssignment.Any())
              e.Block.Performers.Remove(performer);
          }
        }
      }
    }

  }
}