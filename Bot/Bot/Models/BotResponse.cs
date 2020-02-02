using System.Collections.Generic;

namespace Bot.Models
{
    public class BotResponse
    {
        public string Watermark { get; set; }
        public List<Activity> Activities { get; set; }
    }
}
