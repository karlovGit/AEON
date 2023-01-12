using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.CustomM.CustomApprovalRole;

namespace aeon.CustomM.Shared
{
  partial class CustomApprovalRoleFunctions
  {
    public override List<Sungero.Docflow.IDocumentKind> Filter(List<Sungero.Docflow.IDocumentKind> kinds)
    {
      var query = base.Filter(kinds);
      
//      if (_obj.Type == aeon.CustomM.CustomApprovalRole.Type.CompanyAAccount || _obj.Type == aeon.CustomM.CustomApprovalRole.Type.CompanyBAccount)
//        query = query.Where(k => k.DocumentType.DocumentTypeGuid == "f37c7e63-b134-4446-9b5b-f8811f6c9666" ||
//                            k.DocumentType.DocumentTypeGuid == "265f2c57-6a8a-4a15-833b-ca00e8047fa5").ToList();
      
      return query;
    }
  }
}