using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace aeon.Integration1C.Server
{
  public class ModuleJobs
  {

    /// <summary>
    /// Отправить Договорные документы в 1С.
    /// </summary>
    public virtual void SendContractualDocsIn1C()
    {
      var async = AsyncHandlers.SendContractualDocsIn1CAsync.Create();
      async.ExecuteAsync();
    }

    /// <summary>
    /// Получить результаты интеграции из 1С.
    /// </summary>
    public virtual void GetResultsIn1C()
    {
      var async = AsyncHandlers.GetIntegrationsResultsAsync.Create();
      async.ExecuteAsync();
    }

    /// <summary>
    /// Получить данные из 1С.
    /// </summary>
    public virtual void GetDataIn1C()
    {
      var asyncCompany = AsyncHandlers.GetCompanyIn1CAsync.Create();
      asyncCompany.ExecuteAsync();
      
      var asyncContract = AsyncHandlers.GetContractualDocIn1CAsync.Create();
      asyncContract.ExecuteAsync();
    }

    /// <summary>
    /// Отправить Организации в 1С.
    /// </summary>
    public virtual void SendCompanyIn1C()
    {
      var async = AsyncHandlers.SendCompanyIn1CAsync.Create();
      async.ExecuteAsync();
    }

  }
}