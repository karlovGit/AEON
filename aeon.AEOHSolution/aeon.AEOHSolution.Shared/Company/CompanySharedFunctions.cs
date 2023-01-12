using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.AEOHSolution.Company;

namespace aeon.AEOHSolution.Shared
{
  partial class CompanyFunctions
  {    
    
    /// <summary>
    /// Изменить поле Для канцелярии.
    /// </summary>
    /// <param name="TIN">ИНН.</param>
    /// <param name="TRRC">КПП.</param>
    public void ChangeIsForOffice(string TIN, string TRRC)
    {
      if (!string.IsNullOrEmpty(TIN) && !string.IsNullOrEmpty(TRRC))
        _obj.IsForOffice = false;
    }

    /// <summary>
    /// Доступность полей ИНН и КПП.
    /// </summary>
    /// <param name="isForOffice">Для канцелярии.</param>
    public void IsRequiredTINAndTRRC(bool isForOffice)
    {
      if (!isForOffice)
        return;
      
      _obj.State.Properties.TIN.IsRequired = false;
      _obj.State.Properties.TRRC.IsRequired = false;
    }

  }
}