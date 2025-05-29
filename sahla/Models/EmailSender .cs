using System.Net.Mail;
using System.Net;
using sahla.Utility;


public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("worldtroidgaming@gmail.com", "tokt qjjm uxwj bgno")
            };

            return client.SendMailAsync(
                new MailMessage(from: "worldtroidgaming@gmail.com",
                                to: email,
                                subject,
                                message
                                )
                {
                    IsBodyHtml = true

                }

                );

        }
    }

