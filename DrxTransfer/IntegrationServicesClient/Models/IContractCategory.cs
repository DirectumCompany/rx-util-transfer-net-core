﻿using System.Collections.Generic;

namespace DrxTransfer.IntegrationServicesClient
{
  [EntityName("Категории договоров")]
  public class IContractCategory
  {
    public string Name { get; set; }
    public string Status { get; set; }
    public int Id { get; set; }
    public List<ICollectionDocumentKinds> DocumentKinds { get; set; }
    public string Note { get; set; }
    public override string ToString()
    {
      return Name;
    }
  }
}
