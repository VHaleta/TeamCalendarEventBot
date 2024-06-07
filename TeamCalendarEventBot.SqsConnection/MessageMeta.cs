using Amazon.SQS.Model;

namespace SqsProducer.Shared
{
    public static class MessageTypeConstants
    {
        public const string MessageContext = "MESSAGE_CONTEXT";
        public const string MessageCreatedDateTimeOffsetKey = "MESSAGE_SENDDATETIME";
    }

    public class MessageMeta
    {
        private const string MessageDataType = "String";

        public Guid MessageId { get; init; }
        public DateTimeOffset SendDateTime { get; init; }
        public string Context {  get; init; }

        public static MessageMeta InitHeader(string context, DateTimeOffset sendDate)
        {
            return new MessageMeta
            {
                MessageId = new Guid(),
                SendDateTime = sendDate,
                Context = context,
            };
        }

        public Dictionary<string, MessageAttributeValue> ToMessageAttributes()
        {
            return new Dictionary<string, MessageAttributeValue>
                {
                    {
                       MessageTypeConstants.MessageContext,
                       new MessageAttributeValue()
                       {
                           DataType = MessageDataType,
                           StringValue = Context,
                       }
                    },
                    {
                       MessageTypeConstants.MessageCreatedDateTimeOffsetKey,
                       new MessageAttributeValue()
                       {
                           DataType = MessageDataType,
                           StringValue = SendDateTime.ToString(),
                       }
                    },
                };
        }
    }

}
