using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCalendarEventBot.DataStorage.DataJsonFile
{
    internal class EventDataProvider : JsonFileDataProviderBase, IEventDataProvider
    {
        public EventDataProvider(IFileProvider fileProvider) : base(fileProvider)
        {
            
        }
    }
}
