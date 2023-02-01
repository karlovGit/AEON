using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.AEOHSolution.ApprovalStage;

namespace aeon.AEOHSolution.Server
{
  partial class ApprovalStageFunctions
  {
    // Получить исполнителей этапа без раскрытия групп и ролей.
    // <param name="task">Задача.</param>
    // <param name="additionalApprovers">Доп.согласующие.</param>
    // <returns>Исполнители.</returns>
    [Remote(IsPure = true), Public]
    public override List<IRecipient> GetStageRecipients(Sungero.Docflow.IApprovalTask task, List<IRecipient> additionalApprovers)
    {
      var recipients = base.GetStageRecipients(task, additionalApprovers);
      
      var role = _obj.ApprovalRoles
        .Where(x => x.ApprovalRole.Type == aeon.CustomM.CustomApprovalRole.Type.ManagersInit)
        .Select(x => aeon.CustomM.CustomApprovalRoles.As(x.ApprovalRole)).Where(x => x != null)
        .SingleOrDefault();
      
      if (role!= null)
        recipients.AddRange(aeon.CustomM.PublicFunctions.CustomApprovalRole.Remote.GetRolePerformersN(role, task));
      
      return recipients;
    }
  }
}