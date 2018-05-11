namespace Powerfront.BackendTest
{
    public class OperatorReportItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ProactiveSent { get; set; }
        public int ProactiveAnswered { get; set; }
        public int ProactiveResponseRate { get; set; }
        public int ReactiveReceived { get; set; }
        public int ReactiveAnswered { get; set; }
        public int ReactiveResponseRate { get; set; }
        public int TotalChatLengthSeconds { get; set; }
        public int AverageChatLengthSeconds { get; set; }
    }
}