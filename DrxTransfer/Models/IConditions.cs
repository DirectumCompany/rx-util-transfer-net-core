using System.Collections.Generic;

namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Условие.
  /// </summary>
  public class IConditions : IConditionBases
  {
    public List<ICollectionAddressees> Addressees { get; set; }
  }
}
