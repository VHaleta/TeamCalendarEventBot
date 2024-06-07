using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCalendarEventBot.Domain.Listener
{
    public interface ISqsCommunication
    {
        public Task Send(string context, DateTimeOffset date, string message);
    }
}
