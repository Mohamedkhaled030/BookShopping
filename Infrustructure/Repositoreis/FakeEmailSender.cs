using BookShopping.Infrustructure.Abstruct;

namespace BookShopping.Infrustructure.Repositoreis
{
    public class FakeEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Just return a completed task (no actual email sending)
            return Task.CompletedTask;
        }
    }
}
