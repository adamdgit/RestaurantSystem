using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace BitByByte.Service
{
    public class EmailOuterService
    {
        public EmailInnerService Inner { get; set; }

        // Constructor to establish link between
        // instance of Outer_class to its
        // instance of the Inner_class
        public EmailOuterService()
        {
            this.Inner = new EmailInnerService(this);
        }

        public class EmailInnerService
        {

            private EmailOuterService obj;

            public EmailInnerService(EmailOuterService outer)
            {

                obj = outer;
            }

            public bool SendEmailConfirmation(string emailTo, string firstname, string lastname, string url)
            {
                var email = new MimeMessage();

                // email data
                email.From.Add(MailboxAddress.Parse("adammdemol@gmail.com"));
                email.To.Add(MailboxAddress.Parse(emailTo));
                email.Subject = "Registration Confirmation";
                email.Body = new TextPart(TextFormat.Html) 
                { 
                    Text = "<html><body>" + 
                    "<p>Dear " + firstname + " " + lastname + ",</p>" +
                    "<p>" + "Your BeanScene account has been created successfully.</p>" +
                    "<h1>" + url + "</h1>" + 
                    "<p> Your username/email is: " + emailTo + "</p>" + 
                    "<p>Kind Regards;</p>" +
                    "<p>The BeanScene team.</p>" +
                    "</body></html>"
                };

                // send using gmail
                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                
                smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("adammdemol@gmail.com", "ozob uqaj pwzq ilkx");

                smtp.Send(email);
                smtp.Disconnect(true);

                return true;
            }
        }
    }
}
