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

        public SqsCommunication()
        {
            messageProducer = new SqsMessageProducer();
        }

        public async Task Send(string context, DateTimeOffset date, string message)
        {
            await messageProducer.Send(message, context, date);
        }
    }
}
