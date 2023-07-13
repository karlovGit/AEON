using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.AEOHSolution.ConditionBase;

namespace aeon.AEOHSolution.Shared
{
  partial class ConditionBaseFunctions
  {
    public override System.Collections.Generic.Dictionary<string, List<Nullable<Enumeration>>> GetSupportedConditions()
    {
      var baseSupport = base.GetSupportedConditions();
      baseSupport["d1d2a452-7732-4ba8-b199-0a4dc78898ac"].Add(ConditionType.Stamping);
      
      var officialDocuments = Sungero.Docflow.PublicFunctions.DocumentKind.GetDocumentGuids(typeof(Sungero.Docflow.IOfficialDocument));

      foreach (var typeGuid in officialDocuments)
        baseSupport[typeGuid].Add(ConditionType.LeaderInvolved);
      
      return baseSupport;
    }
    public override Sungero.Docflow.Structures.ConditionBase.ConditionResult CheckCondition(Sungero.Docflow.IOfficialDocument document, Sungero.Docflow.IApprovalTask task)
    {
      if (_obj.ConditionType == ConditionType.Stamping)
      {
        var outgoingLetter = aeon.AEOHSolution.OutgoingLetters.As(document);
        
        if (outgoingLetter != null)
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(outgoingLetter.Stamping == aeon.AEOHSolution.OutgoingLetter.Stamping.Yes, string.Empty);
        else
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(null, "Условие не может быть вычислено. Отправляемый документ не того вида.");
      }
      
      if (_obj.ConditionType == ConditionType.LeaderInvolved)
      {
        if (document != null && document.BusinessUnit != null)
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(aeon.AEOHSolution.BusinessUnits.As(document.BusinessUnit).LeaderInvolvedNegotiation.GetValueOrDefault(), string.Empty);
        else
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(null, "Условие не может быть вычислено. Отправляемый документ не того вида.");
      }
      
      return base.CheckCondition(document, task);
    }
  }
}