namespace Tazkartk.Helpers
{
    public class StripeSettings
    {
        public string SecretKey { get; set; }
        public string PublishableKey { get; set; }
        public string notification_url { get; set; }
        public string redirection_url { get; set; }
        public string stripe_signature {  get; set; }
    }
}
