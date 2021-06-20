using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseLayer.Models.Messaging
{
    public class ChatViewModel
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; }

        public List<ChatMessage> ChatMessages { get; set; }
    }

    public class ChatMessage
    {
        public int MessageId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
    }
}