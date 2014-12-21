using System;

namespace StockTweetsApp.Model
{
    public class Tweet
    {
        public ulong Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string UserName { get; set; }

        public string Message { get; set; }

        public int RetweetedCount { get; set; }

        public string ImageSource { get; set; }
    }
}
