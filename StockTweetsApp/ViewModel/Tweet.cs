﻿using System;
using System.Windows.Media;

namespace StockTweetsApp.ViewModel
{
    public class Tweet
    {
        public ulong Id { get; set; }
        public string UserName { get; set; }

        public string Message { get; set; }

        public string ImageSource { get; set; }

        public int RetweetedCount { get; set; }

        public DateTime CreatedAt { get; set; }
        public ulong SinceId { get; set; }
    }
}
