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
    /// <param name="nonresident">Нерезидент.</param>
    /// <param name="counterKind">Вид контрагента.</param>
    public void IsRequiredTINAndTRRC(bool isForOffice, bool nonresident, Integration1C.ICounterpartyKind counterKind)
    {
      _obj.State.Properties.TIN.IsRequired = !isForOffice && !nonresident;
      _obj.State.Properties.TRRC.IsRequired = !isForOffice && !nonresident &&
        (counterKind == null || (counterKind != null && !counterKind.IsMissingTRRC.GetValueOrDefault()));
      _obj.State.Properties.RegNumber.IsRequired = nonresident && !isForOffice;
    }

  }
}