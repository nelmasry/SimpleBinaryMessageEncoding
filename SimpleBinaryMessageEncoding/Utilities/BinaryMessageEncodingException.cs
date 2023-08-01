
namespace SimpleBinaryMessageEncoding.Utilities
{
    public class MessageCodecInvalidDataException : Exception
    {
        public MessageCodecInvalidDataException(string message) : base(message)
        {
        }

        public MessageCodecInvalidDataException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

}
