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
      //если нет НОР (ИНН/КПП) со статусом "Открыт"
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
      else
      {
        //если есть НОР (ИНН/КПП) со статусом "Закрыт"
        if (aeon.AEOHSolution.PublicFunctions.Company.ClosedNORFound(_obj))
        {
          //То открываем НОР:
          aeon.AEOHSolution.PublicFunctions.Company.OpenNOR(_obj);
          e.AddWarning("Была обнаружена закрытая НОР. Запись была открыта");
        }
        else e.AddInformation("По данному контрагенту уже существует открытая наша организация");
      }
      
    }

    public virtual bool CanCreateNOR(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      if (_obj.State.IsInserted || _obj.Status.Equals(Status.Closed))  return false;
      else return true;
    }

  }

}