using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.AEOHSolution.Company;

namespace aeon.AEOHSolution.Client
{
  partial class CompanyActions
  {
    public virtual void CreateNOR(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if ( aeon.AEOHSolution.PublicFunctions.Company.NORNotFound(_obj))
      {
        try{
          aeon.AEOHSolution.PublicFunctions.Company.SCreateNOR(_obj);
          e.AddInformation("Наша организация создана");
        }
        catch(Exception ex)
        {
          e.AddError("Вознилка ошибка: " + ex.Message);
        }
        
      }
      
      else e.AddInformation("По данному контрагенту уже заведена наша организация");
      
     
    }

    public virtual bool CanCreateNOR(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      if (_obj.State.IsInserted) return false;
      else return true;
    }

  }

}