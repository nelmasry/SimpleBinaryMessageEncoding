using SimpleBinaryMessageEncoding.Model;
using SimpleBinaryMessageEncoding.Utilities;

namespace SimpleBinaryMessageEncoding.MessageCodec
{
    /// <summary>
    /// Simple message codec to encode and decode messages
    /// </summary>
    public interface IMessageCodec
    {
        /// <summary>
        /// Encode the given message to a binary representation.
        /// </summary>
        /// <param name="message">The Message to be encoded.</param>
        /// <returns>The encoded message as a byte array.</returns>
        /// <exception cref="MessageCodecInvalidDataException">Thrown when the provided message is invalid or does not meet the required format.</exception>
        /// <exception cref="Exception">Thrown if an unexpected error occurs during encoding.</exception>
        /// <remarks>
        /// <para>
        /// The method adheres to the following constraints:
        /// - The message must have at least one header and a non-null payload.
        /// - The number of headers cannot exceed 63.
        /// - Each header's name and value must be ASCII-encoded strings of at most 1023 bytes each.
        /// - Payload must be ASCII-encoded string.
        /// - The payload is limited to 256 KiB (262,144 bytes).
        /// </para>
        /// </remarks>
        byte[] Encode(Message message);
        /// <summary>
        /// Decode a binary representation to a <see cref="Message"/> object.
        /// </summary>
        /// <param name="data">The byte array containing the encoded message.</param>
        /// <returns>The decoded Message object.</returns>
        /// <exception cref="MessageCodecInvalidDataException">Thrown when the provided encoded data is invalid or does not meet the required format.</exception>
        /// <exception cref="Exception">Thrown if an unexpected error occurs during decoding.</exception>
        /// <remarks>
        /// <para>
        /// The decoding process converts the binary data into a Message object, including headers and payload.
        /// </para>
        /// </remarks>
        Message Decode(byte[] data);
    }

    public class SimpleMessageCodec : IMessageCodec
    {
        public byte[] Encode(Message message)
        {
            if (!IsValidMessage(message))
                throw new MessageCodecInvalidDataException(Utility.ErrorMessage);

            List<byte> encodedMessage = new List<byte>
                {
                    (byte)message.Headers.Count
                };

            foreach (KeyValuePair<string, string> header in message.Headers)
            {
                byte[] nameBytes = Utility.GetAsciiBytes(header.Key);
                byte[] valueBytes = Utility.GetAsciiBytes(header.Value);

                encodedMessage.Add((byte)nameBytes.Length);
                encodedMessage.Add((byte)valueBytes.Length);

                encodedMessage.AddRange(nameBytes);
                encodedMessage.AddRange(valueBytes);
            }

            encodedMessage.AddRange(message.Payload);

            return encodedMessage.ToArray();
        }

        public Message Decode(byte[] data)
        {
            if (!IsValidBinaryData(data))
                throw new MessageCodecInvalidDataException(Utility.ErrorMessage);

            List<byte> messageData = new List<byte>(data);
            Message message = new Message()
            {
                Headers = new Dictionary<string, string>()
            };

            int headersCount = messageData[0];
            messageData.RemoveSafe(0);

            for (int i = 0; i < headersCount; i++)
            {
                int nameLength = messageData[0];
                int valueLength = messageData[1];
                messageData.RemoveSafe(0, 2);

                string name = Utility.GetStringFromAsciiBytes(messageData, nameLength);
                messageData.RemoveSafe(0, nameLength);

                string value = Utility.GetStringFromAsciiBytes(messageData, valueLength);
                messageData.RemoveSafe(0, valueLength);

                message.Headers.Add(name, value);
            }

            message.Payload = messageData.ToArray();

            return message;
        }

        private bool IsValidBinaryData(byte[] binaryData)
        {
            return Utility.IsValidMessage(binaryData);
        }

        private bool IsValidMessage(Message message)
        {
            return Utility.IsValidMessage(message) &&
                   Utility.IsHeadersCountWithinLimit(message.Headers) &&
                   Utility.IsPayloadWithinLimit(message.Payload) &&
                   Utility.IsHeadersSizeWithinLimit(message.Headers) &&
                   Utility.IsHeadersASCII(message.Headers) &&
                   Utility.IsPayloadASCII(message.Payload);
        }
    }
}
