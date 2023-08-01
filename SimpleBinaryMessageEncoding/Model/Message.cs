namespace SimpleBinaryMessageEncoding.Model
{
    /// <summary>
    /// Message to be encoded
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Headers of the message
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }
        /// <summary>
        /// Message payload
        /// </summary>
        public byte[] Payload { get; set; }
    }
}