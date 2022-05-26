namespace DrxTransfer.IntegrationServicesClient
{
    [EntityName("Ссылка на субъект")]
    public class IRecipientsLinks
    {
        public int Id { get; set; }
        public IUsers Member { get; set; }

    }
}
