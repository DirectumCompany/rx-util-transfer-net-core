namespace DrxTransfer.IntegrationServicesClient
{
  /// <summary>
  /// Способ доставки документов.
  /// </summary>
  public class IMailDeliveryMethods
  {
    public string Name { get; set; }
    public string Note { get; set; }
    public string Status { get; set; }
    public string Sid { get; set; }

    public int Id { get; set; }

    public override string ToString()
    {
      return Name;
    }
  }
}
