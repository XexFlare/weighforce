using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using WeighForce.Data.EF;
using WeighForce.Models;

namespace WeighForce.Services
{
    public class EmailService
    {
        private readonly IFluentEmail _email;

        public EmailService(IFluentEmail email)
        {
            _email = email;
        }

        public async void SendReport(string address, Report model)
        {
            var email = _email
                .To(address)
                .Subject("Test email")
                .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Pages/Email/Weekly.cshtml", model);
            await email.SendAsync();
        }
        public async Task SendDailyReport(string address, Report model)
        {
            var email = _email
                .To(address)
                .Subject($"Dispatch Summary for {model.StartDate}")
                .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Pages/Email/Daily.cshtml", model);
            await email.SendAsync(); System.Console.WriteLine("Sending to {0}", address);
        }
        public async Task SendTrainsReport(List<Address> addresses, List<Dispatch> model)
        {
            var date = DateTime.Today.ToShortDateString();
            var email = _email
                .To(addresses.First().EmailAddress, addresses.First().Name)
                .Subject($"Trains Received: {date}")
                .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Pages/Email/Trains.cshtml", model);
            if (addresses.Count > 1)
            {
                addresses.Remove(addresses.First());
                email.CC(addresses);
            }
            await email.SendAsync();
            Console.WriteLine("Sending to {0}", addresses.First().EmailAddress);
        }
        public async Task SendWeeklyReport(string address, Report model)
        {
            var email = _email
                .To(address)
                .Subject("Weekly Dispatch Summary")
                .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Pages/Email/Weekly.cshtml", model);
            await email.SendAsync();
            System.Console.WriteLine("Sending to {0}", address);
        }
        public async Task SendTransporterReport(string address, List<TransporterMismatch> details)
        {
            if (details.Count > 0)
            {
                var email = _email
                    .To(address)
                    .Subject("Transporter Mismatch Summary")
                    .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Pages/Email/TransporterMismatch.cshtml", details);
                await email.SendAsync();
            }
        }
    }
}