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

    public override void CompleteAssignment6(Sungero.Docflow.IApprovalAssignment assignment, Sungero.Docflow.Server.ApprovalAssignmentArguments e)
    {
      base.CompleteAssignment6(assignment, e);
      
      // Уведомлять согласующих о результате согласовании.
      if (assignment.Result == Sungero.Docflow.ApprovalAssignment.Result.Approved || assignment.Result == Sungero.Docflow.ApprovalAssignment.Result.ForRevision)
      {
        var stage = assignment.Stage;
        
        if (stage != null)
        {
          var stageCustom = AEOHSolution.ApprovalStages.As(stage);
          
          if (stageCustom != null && stageCustom.NotifyApprovers == true)
          {
            var document = assignment.DocumentGroup.OfficialDocuments.First();
            var assignments = Sungero.Docflow.ApprovalAssignments.GetAll(x => x.Id != assignment.Id && x.Task == _obj && x.Status == Sungero.Docflow.ApprovalAssignment.Status.InProcess);
            
            foreach (var ass in assignments)
            {
              var task = aeon.CustomM.TaskNoteApprovals.CreateAsSubtask(assignment);
              task.DocumentGroup.OfficialDocuments.Add(_obj.DocumentGroup.OfficialDocuments.First());
              // Вложить задание на согласование.
              task.AssignmentGroup.ApprovalAssignments.Add(ass);
              
              // Subject для каждого результата свой.
              if (assignment.Result == Sungero.Docflow.ApprovalAssignment.Result.Approved)
              {
                task.Result = aeon.CustomM.TaskNoteApproval.Result.Approved;
              }
              else if (assignment.Result == Sungero.Docflow.ApprovalAssignment.Result.ForRevision)
              {
                task.Result = aeon.CustomM.TaskNoteApproval.Result.ForRevision;
              }
              
              task.Assignee = ass.Performer;
              task.AgreedBy = assignment.Performer;
              task.Start();
            }
          }
        }
      }
    }

    public override void StartBlock6(Sungero.Docflow.Server.ApprovalAssignmentArguments e)
    {
      base.StartBlock6(e);
      
      var stage = _obj.ApprovalRule.Stages.Where(s => s.Stage != null)
        .Where(s => s.Stage.StageType == Sungero.Docflow.ApprovalStage.StageType.Approvers)
        .FirstOrDefault(s => s.Number == _obj.StageNumber);
      
      if (stage != null && e.Block.Performers.Any())
      {
        var stageCustom = AEOHSolution.ApprovalStages.As(stage.Stage);
        
        if (stageCustom != null && stageCustom.SkipRenegotiation == true)
        {
          var listPerformers = e.Block.Performers.ToList();
          var reworkAssignments = Sungero.Docflow.ApprovalReworkAssignments.GetAll(x => x.Task == _obj).OrderByDescending(x => x.Created).FirstOrDefault();
          
          foreach (var performer in listPerformers)
          {
            var hasAssignment = Sungero.Docflow.ApprovalAssignments.GetAll(x => x.Task == _obj && x.Performer == performer && x.Completed.HasValue
                                                                           && x.Result == Sungero.Docflow.ApprovalAssignment.Result.Approved);
            
            var hasManagerAssignment = Sungero.Docflow.ApprovalManagerAssignments.GetAll(x => x.Task == _obj && x.Performer == performer && x.Completed.HasValue && x.Created > _obj.Started
                                                                                         && x.Result == Sungero.Docflow.ApprovalManagerAssignment.Result.Approved);
            
            if (reworkAssignments != null)
            {
              hasAssignment = hasAssignment.Where(x => x.Completed.HasValue && x.Completed > reworkAssignments.Created);
              
              hasManagerAssignment = hasManagerAssignment.Where(x => x.Completed.HasValue && x.Completed > reworkAssignments.Created);
            }
            
            if (hasAssignment.Any() || hasManagerAssignment.Any())
              e.Block.Performers.Remove(performer);
          }
        }
        
        // При повторном согласовании нужно меняем старт на "Друг за другом."
        if (stageCustom != null && stageCustom.ChangeSequence == true)
        {
          var hasAssignment = Sungero.Docflow.ApprovalAssignments.GetAll(x => x.Task == _obj && x.Stage == stage.Stage).Any();
          
          if (hasAssignment)
          {
            var recipients = new List<IRecipient>();
            
            var assignments = ApprovalAssignments.GetAll().Where(a => Equals(a.Task, _obj) && Equals(a.TaskStartId, _obj.StartId) && Equals(a.Stage, stageCustom)).ToList();
            
            foreach (var assignment in assignments)
            {
              if (assignment.ForwardedTo == null)
                continue;
              
              recipients.AddRange(assignment.ForwardedTo);
            }
            
            var performers = e.Block.Performers.Select(x => x).Cast<IRecipient>().ToList();
            var performersAdded = performers.Where(x => recipients.Contains(x)).ToList();
            performers = performers.Where(x => !performersAdded.Contains(x)).ToList();
            
            e.Block.Performers.Clear();
            foreach (var performer in performersAdded)
              e.Block.Performers.Add(performer);
            
            foreach (var performer in performers)
              e.Block.Performers.Add(performer);
            
            e.Block.IsParallel = false;
          }
        }
      }
      
    }

  }
}