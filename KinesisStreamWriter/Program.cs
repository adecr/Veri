using System;
using System.IO;
using System.Text;
using Amazon;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Bogus;
using Model;
using Newtonsoft.Json;

namespace KinesisStreamWriter
{
    class Program
    {
        static void Main(string[] args)
        {
            var awskey = args[0];
            var iterations = Int32.Parse(args[1]);

            for (int i = 0; i < iterations; i++)
            {
                var telemetry = MakeRandomTelemetry();
                StreamEventToKineses(telemetry, awskey);
            }

        }

        private static void StreamEventToKineses(TelemetryEvent telemetry, string secretKey)
        {
            var json = JsonConvert.SerializeObject(telemetry);
            Console.Write(json);

            AWSCredentials credentials = new BasicAWSCredentials("AKIAIDNFSJBBZ3CSD2KQ", secretKey);
            AmazonKinesisClient kinesisClient = new AmazonKinesisClient(credentials, RegionEndpoint.EUWest1);
            const string kinesisStreamName = "AdeVericon";

            byte[] dataAsBytes = Encoding.UTF8.GetBytes(json);
            using (MemoryStream memoryStream = new MemoryStream(dataAsBytes))
            {
                try
                {
                    PutRecordRequest requestRecord = new PutRecordRequest();
                    requestRecord.StreamName = kinesisStreamName;
                    requestRecord.PartitionKey = telemetry.DeviceID;
                    requestRecord.Data = memoryStream;

                    var responseRecord =  kinesisClient.PutRecordAsync(requestRecord);
                    responseRecord.Wait();
                    Console.WriteLine("Successfully sent record {0} to Kinesis. Sequence number: {1}", telemetry.DeviceID, responseRecord.Result.SequenceNumber);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to send record {0} to Kinesis. Exception: {1}", telemetry.DeviceID, ex.Message);
                }
            }
        }

        private static TelemetryEvent MakeRandomTelemetry()
        {
            var telemetryEvent = new TelemetryEvent {DeviceID = Guid.NewGuid().ToString()};

            var telemetryGenerator = new Faker<Telemetry>()
                .StrictMode(true)
                .RuleFor(o => o.BoilerType, f => f.Random.AlphaNumeric(3))
                .RuleFor(o => o.BoilerModel, f => f.Random.Words(2))
                .RuleFor(o => o.BoilerSerial, f => f.Random.AlphaNumeric(20))
                .RuleFor(o => o.Timestamp, f => f.Date.Recent(2))
                .RuleFor(o => o.TemperatureC, f => f.Random.Float(30, 60))
                .RuleFor(o => o.PressureBar, f => f.Random.Float(0, 5));

            telemetryEvent.Content = telemetryGenerator.Generate();
            return telemetryEvent;
        }
    }
}
