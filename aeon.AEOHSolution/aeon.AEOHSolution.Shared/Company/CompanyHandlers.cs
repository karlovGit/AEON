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

    public virtual void RegNumberChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue && !string.IsNullOrEmpty(e.NewValue))
        _obj.IsForOffice = false;
    }

    public override void NonresidentChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      base.NonresidentChanged(e);
      
      if (e.NewValue.HasValue && e.NewValue != e.OldValue)
        Functions.Company.IsRequiredTINAndTRRC(_obj, _obj.IsForOffice.GetValueOrDefault(), e.NewValue.Value);
    }

    public override void TRRCChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      base.TRRCChanged(e);
      
      if (e.NewValue != e.OldValue && !string.IsNullOrEmpty(e.NewValue))
        Functions.Company.ChangeIsForOffice(_obj, _obj.TIN, e.NewValue);
    }

    public override void TINChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      base.TINChanged(e);
      
      if (e.NewValue != e.OldValue && !string.IsNullOrEmpty(e.NewValue))
        Functions.Company.ChangeIsForOffice(_obj, e.NewValue, _obj.TRRC);
    }

    public virtual void IsForOfficeChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if (e.NewValue.HasValue && e.NewValue != e.OldValue)
        Functions.Company.IsRequiredTINAndTRRC(_obj, e.NewValue.Value, _obj.Nonresident.GetValueOrDefault());
    }

  }
}