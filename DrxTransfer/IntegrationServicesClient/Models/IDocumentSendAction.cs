namespace DrxTransfer.IntegrationServicesClient
{
  [EntityName("Действие по отправке")]
  public class IDocumentSendAction
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public string ActionGuid { get; set; }

    public override string ToString()
    {
      return Name;
    }
  }
}
