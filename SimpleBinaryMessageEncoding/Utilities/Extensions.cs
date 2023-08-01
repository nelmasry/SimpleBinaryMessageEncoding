
namespace SimpleBinaryMessageEncoding.Utilities
{
    internal static class ListExtensions
    {
        /// <summary>
        /// Remove items from list safely without unhandled exception.<see cref="MessageCodecInvalidDataException"/> will be thrown in case if any error occurred.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <exception cref="MessageCodecInvalidDataException"></exception>
        public static void RemoveSafe(this List<byte> bytes, int index, int count = 0)
        {
            try
            {
                if (count == 0)
                    bytes.RemoveAt(index);
                else
                    bytes.RemoveRange(index, count);
            }
            catch (Exception)
            {
                throw new MessageCodecInvalidDataException(Utility.ErrorMessage);
            }
        }
    }
}
