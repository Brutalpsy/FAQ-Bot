using System;
using System.Collections.Generic;

namespace Bot.Models
{
    public class BotResponseEventArgs : EventArgs
    {
        public List<Activity> Activities { get; set; }
    }
}
