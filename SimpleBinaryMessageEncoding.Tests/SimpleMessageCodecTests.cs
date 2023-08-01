using SimpleBinaryMessageEncoding.MessageCodec;
using SimpleBinaryMessageEncoding.Utilities;
using Xunit;
using Message = SimpleBinaryMessageEncoding.Model.Message;

namespace SimpleBinaryMessageEncoding.Tests
{
    public class SimpleMessageCodecTests
    {
        private readonly SimpleMessageCodec codec;

        public SimpleMessageCodecTests()
        {
            codec = new SimpleMessageCodec();
        }

        [Fact]
        public void Encode_Returns_BinaryData_WhenValidMessage()
        {
            var headers = new Dictionary<string, string>
            {
                { "h1", "v1" },
                { "h2", "v2" }
            };
            var payload = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };
            var originalMessage = new Message
            {
                Headers = headers,
                Payload = payload
            };
            
            var encodedData = codec.Encode(originalMessage);

            var expectedEncodedMessage = new byte[]
            {
                0x02, 0x02, 0x02, 0x68, 0x31, 0x76, 0x31, 0x02, 0x02, 0x68, 0x32, 0x76, 0x32, 0x01, 0x02, 0x03, 0x04, 0x05
            };

            Assert.Equal(expectedEncodedMessage, encodedData);
        }

        [Fact]
        public void Dencode_Returns_Message_WhenValidData()
        {
            var expectedDecodedMessage = new Message
            {
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "text/plain" },
                    { "Authorization", "Bearer ABC123" }
                },
                Payload = new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x66, 0x72, 0x6F, 0x6D, 0x20, 0x74, 0x68, 0x65, 0x20, 0x6F, 0x74, 0x68, 0x65, 0x72, 0x20, 0x73, 0x69, 0x64, 0x65, 0x2E }
            };

            var encodedMessage = new byte[]
            {
                0x02, 0x0C, 0x0A, 0x43, 0x6F, 0x6E, 0x74, 0x65, 0x6E, 0x74, 0x2D, 0x54, 0x79, 0x70, 0x65, 0x74,
                0x65, 0x78, 0x74, 0x2F, 0x70, 0x6C, 0x61, 0x69, 0x6E, 0x0D, 0x0D, 0x41, 0x75, 0x74, 0x68, 0x6F,
                0x72, 0x69, 0x7A, 0x61, 0x74, 0x69, 0x6F, 0x6E, 0x42, 0x65, 0x61, 0x72, 0x65, 0x72, 0x20, 0x41,
                0x42, 0x43, 0x31, 0x32, 0x33, 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x66, 0x72, 0x6F, 0x6D, 0x20,
                0x74, 0x68, 0x65, 0x20, 0x6F, 0x74, 0x68, 0x65, 0x72, 0x20, 0x73, 0x69, 0x64, 0x65, 0x2E
            };

            var decodedMessage = codec.Decode(encodedMessage);

            Assert.Equal(expectedDecodedMessage.Payload, decodedMessage.Payload);
            Assert.Equal(expectedDecodedMessage.Headers, decodedMessage.Headers);
        }

        [Theory]
        [MemberData(nameof(GetInvalidMessages))]
        public void Encode_Throws_MessageCodecInvalidDataException_WhenInValidMessage(Message message)
        {
            Assert.Throws<MessageCodecInvalidDataException>(() => codec.Encode(message));
        }

        [Theory]
        [MemberData(nameof(GetInvalidBinaryData))]
        public void Dencode_Throws_MessageCodecInvalidDataException_WhenInValidBinary(byte[] binaryData)
        {
            Assert.Throws<MessageCodecInvalidDataException>(() => codec.Decode(binaryData));
        }



        // helper methods
        public static IEnumerable<object[]> GetInvalidMessages()
        {
            return new List<object[]>
        {
            new object[] { null },
            new object[] { new Message() },
            new object[] { new Message() { Payload = null } },
            new object[] { new Message() { Payload = new byte[0] } },
            new object[] { new Message() { Headers = null } },
            new object[] { new Message() { Headers = new Dictionary<string, string>() } },

            // Headers exceed the limit of 63 headers
            new object[]
            {
                new Message()
                {
                    Headers = GetMaxHeaders(),
                    Payload = new byte[] { 0x01, 0x02, 0x03 }
                }
            },

            // Payload size exceeds the limit of 256 KiB
            new object[]
            {
                new Message()
                {
                    Headers = new Dictionary<string, string> { { "Header", "Value" } },
                    Payload = new byte[256 * 1024 + 1]
                }
            },
            // Payload contains non-ASCII characters
            new object[]
            {
                new Message()
                {
                    Headers = new Dictionary<string, string> { { "Header", "Value" } },
                    Payload = new byte[]{ 0xE4 , 0xF6 , 0xFC }
                }
            },
             //Header name or value exceeds the limit of 1023 bytes
            new object[]
            {
                new Message()
                {
                    Headers = new Dictionary<string, string> { { "Header", new string('A', 1024) } },
                    Payload = new byte[] { 0x01, 0x02, 0x03 }
                }
            },

            // Header contains non-ASCII characters
            new object[]
            {
                new Message()
                {
                    Headers = new Dictionary<string, string> { { "Header", "Value" }, { "Header2", "Non-ASCII הצü" } },
                    Payload = new byte[] { 0x01, 0x02, 0x03 }
                }
            },
        };
        }

        private static Dictionary<string, string> GetMaxHeaders()
        {
            var headers = new Dictionary<string, string>();
            for (int i = 0; i < 64; i++)
            {
                headers.Add($"Header{i}", $"Value{i}");
            }
            return headers;
        }

        public static IEnumerable<object[]> GetInvalidBinaryData()
        {
            return new List<object[]>
        {
            new object[] { null }, // null data
            new object[] { new byte[0] }, // empty data
            new object[] { new byte[] { 0x80, 0x81, 0x82 } }, // Non-ASCII data
            new object[] { new byte[264775] }, // Data length exceeds the limit
            new object[] { new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 } }, // invalid message
        };
        }
    }
}