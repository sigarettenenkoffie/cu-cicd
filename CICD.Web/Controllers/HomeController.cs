using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CICD.Web.Models;
using CICD.Lib;
using CICD.Web.ViewModels;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;

namespace CICD.Web.Controllers
{
    public class HomeController : Controller
    {
        public Task<IActionResult> Index()
        {

            return Index("1,2,3");
        }

        [HttpPost("input")]
        public async Task<IActionResult> Index(string input)
        {
            StringCalculatorViewModel calculationResult = CalculateSum(input);
            await SendMail(calculationResult);
            return View(calculationResult);
        }

        private async Task SendMail(StringCalculatorViewModel content)
        {
            const string MAIL_HOST = "mail"; // mvc app in Docker container
            //const string MAIL_HOST = "localhost"; // mvc app uitgevoerd vanaf lokaal OS (niet in container)

            const int MAIL_PORT = 1025;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("CICD Web Application", "cicdsolution@howestgp.be"));
            message.To.Add(new MailboxAddress("Student", "student@sumwanted.com"));
            message.Subject = "Your calculated Sum, Siegje!!!";
            message.Body = new TextPart("plain")
            {
                Text = $"Hello, your calculation result: sum of {content.Input} = {content.Sum} " +
                $"{(content.Error != String.Empty ? $"\n\nError: {content.Error} " : "")}"
            };
            using (var mailClient = new SmtpClient())
            {
                await mailClient.ConnectAsync(MAIL_HOST, MAIL_PORT, SecureSocketOptions.None);
                await mailClient.SendAsync(message);
                await mailClient.DisconnectAsync(true);
            }

        }

        private StringCalculatorViewModel CalculateSum(string input)
        {
            StringCalculatorViewModel scvm = new StringCalculatorViewModel
            {
                Input = input
            };
            try
            {
                StringCalculator stringCalculator = new StringCalculator();
                int sum = stringCalculator.Add(input);
                scvm.Sum = sum;
            }
            catch (Exception ex)
            {
                scvm.Sum = 0;
                scvm.Error = ex.Message;
            }
            return scvm;

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
