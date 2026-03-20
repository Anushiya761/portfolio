using AnushiyaPortfolio.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace AnushiyaPortfolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EmailSettings _emailSettings;

        public HomeController(ILogger<HomeController> logger, IOptions<EmailSettings> emailSettings)
        {
            _logger = logger;
            _emailSettings = emailSettings.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        [HttpPost]
        public IActionResult SendEmail(string Name, string Email, string Message)
        {
            try
            {
                var mail = new MailMessage();
                mail.From = new MailAddress(_emailSettings.Email);
                mail.To.Add(_emailSettings.Email);
                mail.Subject = "New Message from Portfolio Website";

                mail.Body = $"Name: {Name}\n" +
                            $"Email: {Email}\n\n" +
                            $"Message:\n{Message}";

                var smtp = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
                {
                    Credentials = new NetworkCredential(
                        _emailSettings.Email,
                        _emailSettings.Password
                    ),
                    EnableSsl = true
                };

                smtp.Send(mail);

                TempData["Success"] = "Message sent successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error sending message: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
