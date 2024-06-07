using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using SqsProducer.Shared;

namespace TeamCalendarEventBot.SqsConnection
{
    public class SqsMessageProducer
    {
        private AmazonSQSClient _sqsClient;
        private readonly string _queueUrl;
        private readonly string _awsregion;
        private readonly string _accessKey;
        private readonly string _secret;
        private readonly string _seviceUrl;
        public SqsMessageProducer()
        {
            _accessKey = "ignore";
            _secret = "ignore";
            _seviceUrl = "http://localhost:4566";
            _queueUrl = "https://sqs.eu-central-1.localhost.localstack.cloud:4566/000000000000/mainQueue";
            _awsregion = "eu-central-1";

            var awsCreds = new BasicAWSCredentials(_accessKey, _secret);
            var config = new AmazonSQSConfig
            {
                ServiceURL = _seviceUrl,
                AuthenticationRegion = _awsregion
            };

            _sqsClient = new AmazonSQSClient(awsCreds, config);
        }

        public async Task Send(string message, string context, DateTimeOffset sendDate)
        {
            SendMessageRequest sendMessageRequest = new SendMessageRequest(_queueUrl, message);

            var meta = MessageMeta.InitHeader(context, sendDate);
            sendMessageRequest.MessageAttributes = meta.ToMessageAttributes();

            await _sqsClient.SendMessageAsync(sendMessageRequest);
        }
    }
}