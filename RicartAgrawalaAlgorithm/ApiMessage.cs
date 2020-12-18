namespace RicartAgrawalaAlgorithm
{
    public enum MessageType
    {
        Request = 0,
        Response = 32768,
    }

    public enum CriticalSection
    {
        Main
    }

    public class ApiMessage
    {
        public MessageType Type { get; set; }
        public uint FromId { get; set; }
        public uint ToId { get; set; }
        public uint Time { get; set; }
        public CriticalSection Section { get; set; }

        public ApiMessage(InternalMessage message)
        {
            Time = (uint) message.time;
            Type = (MessageType) message.message;
            FromId = (uint) message.wParam;
            Section = (CriticalSection) message.lParam;
        }

        public ApiMessage(MessageType type, uint from, uint to, CriticalSection section)
        {
            Type = type;
            FromId = from;
            ToId = to;
            Section = section;
        }

        public override string ToString()
        {
            return $"{Type} TIME:{Time} FROM:{FromId} TO:{ToId}";
        }
    }
}