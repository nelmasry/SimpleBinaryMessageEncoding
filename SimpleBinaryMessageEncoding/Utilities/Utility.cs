using SimpleBinaryMessageEncoding.Model;

namespace SimpleBinaryMessageEncoding.Utilities
{
    internal static class Utility
    {
        readonly static int MaxHeadersCount = 63;
        readonly static int MaxHeaderSize = 1023;
        readonly static int MaxPayloadSize = 256 * 1024;
        internal static string ErrorMessage = "Invalid message: The message does not meet the required format.";

        internal static bool IsHeadersCountWithinLimit(Dictionary<string, string> headers)
        {
            return headers.Count <= MaxHeadersCount;
        }

        internal static bool IsPayloadWithinLimit(byte[] payload)
        {
            return payload.Length <= MaxPayloadSize;
        }

        internal static bool IsHeadersSizeWithinLimit(Dictionary<string, string> headers)
        {
            foreach (KeyValuePair<string,string> header in headers)
            {
                if (GetAsciiBytes(header.Key).Length > MaxHeaderSize || GetAsciiBytes(header.Value).Length > MaxHeaderSize)
                    return false;
            }
            return true;
        }

        internal static bool IsHeadersASCII(Dictionary<string, string> headers)
        {
            foreach (KeyValuePair<string, string> header in headers)
            {
                if (!IsAscii(header.Key) || !IsAscii(header.Value))
                    return false;
            }
            return true;
        }

        internal static bool IsPayloadASCII(byte[] payload)
        {
            foreach (byte b in payload)
            {
                if (!IsAscii(b))
                    return false;
            }
            return true;
        }

        internal static bool IsMessageASCII(byte[] message)
        {
            foreach (byte b in message)
            {
                if (!IsAscii(b))
                    return false;
            }
            return true;
        }

        internal static bool IsAscii(string s)
        {
            foreach (char c in s)
            {
                if (!IsAscii(c))
                    return false;
            }
            return true;
        }

        internal static bool IsAscii(byte b)
        {
            return b < 128;
        }
        internal static bool IsAscii(char c)
        {
            return c < 128;
        }


        internal static byte[] GetAsciiBytes(string input)
        {
            byte[] result = new byte[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                result[i] = (byte)input[i];
            }
            return result;
        }

        internal static string GetStringFromAsciiBytes(List<byte> data, int length)
        {
            if (data.Count < length)
                throw new MessageCodecInvalidDataException(ErrorMessage);

            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = (char)data[i];
            }
            return new string(chars);
        }

        internal static bool IsValidMessage(Message message)
        {
            return message != null && message.Payload != null && message.Payload.Length > 0 && message.Headers != null && message.Headers.Count > 0;
        }

        internal static bool IsValidMessage(byte[] data)
        {
            return data != null && data.Length > 0 && data.Length <= 264774 && IsMessageASCII(data);
        }
    }
}
