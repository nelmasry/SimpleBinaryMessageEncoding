using SimpleBinaryMessageEncoding.Model;

namespace SimpleBinaryMessageEncoding.Utilities
{
    internal static class Utility
    {
        readonly static int MaxHeadersCount = 63;
        readonly static int MaxHeaderSize = 1023;
        readonly static int MaxPayloadSize = 256 * 1024;
        internal static string ErrorMessage = "Invalid message: The message does not meet the required format.";

        /// <summary>
        /// Validate if Header count within the limit 
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        internal static bool IsHeadersCountWithinLimit(Dictionary<string, string> headers)
        {
            return headers.Count <= MaxHeadersCount;
        }
        
        /// <summary>
        /// Validate if payload size within the limit
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        internal static bool IsPayloadWithinLimit(byte[] payload)
        {
            return payload.Length <= MaxPayloadSize;
        }

        /// <summary>
        /// Validate each header size within the limit
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        internal static bool IsHeadersSizeWithinLimit(Dictionary<string, string> headers)
        {
            foreach (KeyValuePair<string,string> header in headers)
            {
                if (GetAsciiBytes(header.Key).Length > MaxHeaderSize || GetAsciiBytes(header.Value).Length > MaxHeaderSize)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Validate if headers key and value are ASCII encoded
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        internal static bool IsHeadersASCII(Dictionary<string, string> headers)
        {
            foreach (KeyValuePair<string, string> header in headers)
            {
                if (!IsAscii(header.Key) || !IsAscii(header.Value))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Validate if payload bytes for non-ASCII encoded characters
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        internal static bool IsPayloadASCII(byte[] payload)
        {
            foreach (byte b in payload)
            {
                if (!IsAscii(b))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Validate if binaryData bytes for non-ASCII encoded characters
        /// </summary>
        /// <param name="binaryData"></param>
        /// <returns></returns>
        internal static bool IsMessageASCII(byte[] binaryData)
        {
            foreach (byte b in binaryData)
            {
                if (!IsAscii(b))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Check if string is ASCII encoded
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        internal static bool IsAscii(string s)
        {
            foreach (char c in s)
            {
                if (!IsAscii(c))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Check if byte is of ASCII encoded character
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        internal static bool IsAscii(byte b)
        {
            return b < 128;
        }

        /// <summary>
        /// Check if character is ASCII encoded
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        internal static bool IsAscii(char c)
        {
            return c < 128;
        }

        /// <summary>
        /// Get bytes from sting input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static byte[] GetAsciiBytes(string input)
        {
            byte[] result = new byte[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                result[i] = (byte)input[i];
            }
            return result;
        }
        
        /// <summary>
        /// Get string from binary data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        /// <exception cref="MessageCodecInvalidDataException"></exception>
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

        /// <summary>
        /// Validate message to be encoded
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        internal static bool IsValidMessage(Message message)
        {
            return message != null && message.Payload != null && message.Payload.Length > 0 && message.Headers != null && message.Headers.Count > 0;
        }

        /// <summary>
        /// Validate binary data to be decoded
        /// </summary>
        /// <param name="binaryData"></param>
        /// <returns></returns>
        internal static bool IsValidMessage(byte[] binaryData)
        {
            return binaryData != null && binaryData.Length > 0 && binaryData.Length <= 264774 && IsMessageASCII(binaryData);
        }
    }
}
