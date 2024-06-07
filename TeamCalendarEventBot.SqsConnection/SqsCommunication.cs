using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TeamCalendarEventBot.Domain.Listener;

namespace TeamCalendarEventBot.SqsConnection
{
    public class SqsCommunication : ISqsCommunication
    {
        private SqsMessageProducer messageProducer;
        private ILogger<SqsCommunication> _logger;

        public SqsCommunication(ILogger<SqsCommunication> logger)
        {
            _logger = logger;
            try
            {
                messageProducer = new SqsMessageProducer();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public async Task Send(string context, DateTimeOffset date, string message)
        {
            await messageProducer.Send(message, context, date);
        }
    }
}
