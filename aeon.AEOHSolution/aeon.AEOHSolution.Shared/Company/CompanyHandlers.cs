using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using aeon.AEOHSolution.Company;

namespace aeon.AEOHSolution
{
  partial class CompanySharedHandlers
  {

    public virtual void CounterpartyKindChanged(aeon.AEOHSolution.Shared.CompanyCounterpartyKindChangedEventArgs e)
    {
      if (e.NewValue == e.OldValue)
        return;
      
      Functions.Company.IsRequiredTINAndTRRC(_obj, _obj.IsForOffice.GetValueOrDefault(), _obj.Nonresident.GetValueOrDefault(), e.NewValue);
    }

    public virtual void RegNumberChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      if (e.NewValue == e.OldValue)
        return;
      
      if (!string.IsNullOrEmpty(e.NewValue))
        _obj.IsForOffice = false;
    }

    public override void NonresidentChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      base.NonresidentChanged(e);
      
      if (e.NewValue == e.OldValue)
        return;
      
      if (e.NewValue.HasValue)
        Functions.Company.IsRequiredTINAndTRRC(_obj, _obj.IsForOffice.GetValueOrDefault(), e.NewValue.Value, _obj.CounterpartyKind);
    }

    public override void TRRCChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      base.TRRCChanged(e);
      
      if (e.NewValue == e.OldValue)
        return;
      
      if (!string.IsNullOrEmpty(e.NewValue))
        Functions.Company.ChangeIsForOffice(_obj, _obj.TIN, e.NewValue);
    }

    public override void TINChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      base.TINChanged(e);
      
      if (e.NewValue == e.OldValue)
        return;
      
      if (!string.IsNullOrEmpty(e.NewValue))
        Functions.Company.ChangeIsForOffice(_obj, e.NewValue, _obj.TRRC);
    }

    public virtual void IsForOfficeChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if (e.NewValue == e.OldValue)
        return;
      
      if (e.NewValue.HasValue)
        Functions.Company.IsRequiredTINAndTRRC(_obj, e.NewValue.Value, _obj.Nonresident.GetValueOrDefault(), _obj.CounterpartyKind);
    }

  }
}