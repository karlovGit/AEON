using System;
using Sungero.Core;

namespace aeon.CustomM.Constants
{
  public static class Module
  {
    // GUID роли "Главные бухгалтеры наших организаций".
    [Sungero.Core.Public]
    public static readonly Guid ChiefAccountantsOurOrgs = Guid.Parse("E8487138-7B1E-4383-A5CF-B6BE6E80A969");
    
    // GUID роли "Исключения из Согласование с вышестоящим руководителем (множественное)".
    [Sungero.Core.Public]
    public static readonly Guid ExceptionsToSupervisory = Guid.Parse("1D5105A6-231A-49C3-B1FC-B6623C502F20");
  }
}