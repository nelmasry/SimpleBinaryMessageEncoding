# Simple Binary Message Encoding

Simple Binary Message Encoding is a library for encoding and decoding binary messages in a signaling protocol. The library is designed for passing messages between peers in a real-time communication application.

## Table of Contents
1. [About the Project](#about-the-project)
2. [Assumptions](#assumptions)
3. [Getting Started](#getting-started)
4. [Usage](#usage)
5. [Testing](#testing)
6. [Contributing](#contributing)

## About the project

- Encode a `Message` object into a binary representation.
- Decode a binary representation back into a `Message` object.
- Support for variable number of headers (name-value pairs) and a binary payload.
- Message should have the following:
  - Headers are ASCII-encoded strings with a maximum size of 1023 bytes each.
  - Messages can have up to 63 headers.
  - Payload size is limited to 256 KiB (262,144 bytes).
  - Payload can contain binary data without any specific character encoding (**Only ASCII characters**).
    
- Solution consists of two projects (.NET 6.0 / C#):
  - SimpleBinaryMessageEncoding: Class library having the code for Message encoding & decoding
  - SimpleBinaryMessageEncoding.Tests: Test project for unit tests

## Assumptions
-  Project is a class library that will be used by developers to encode and decode messages
-  For simplicity, IMessageCodec interface has the Encode and Decode functionality implemented in SimpleMessageCodec class
-  Utilities project is added for helper methods and extension used by SimpleMessageCodec class
    - Utility class has all validations and helper methods
    - Extensions for any extension methods
    - MessageCodecInvalidDataException as a custom exceptoin used by SimpleMessageCodec
-  Assumed that message payload can contain binary data without any specific character encoding (**Only ASCII characters**).

## Getting Started

To use the library in your project, follow these steps:

1. Clone the repository:
   `git clone https://github.com/<USERNAME>/<REPO_NAME>.git`
3. Build the project:`dotnet build`
4. Reference the library in your project.
      
## Usage
To use the Simple Binary Message Encoding library, you need to create an instance of the SimpleMessageCodec class and use its Encode and Decode methods as follows:
```csharp
using SimpleBinaryMessageEncoding.MessageCodec;
using SimpleBinaryMessageEncoding.Model;

// Create a new instance of the codec
IMessageCodec codec = new SimpleMessageCodec();

// Create a Message object
var headers = new Dictionary<string, string>
{
    { "Content-Type", "text/plain" },
    { "Authorization", "Bearer ABC123" }
};
var payload = new byte[] { 0x48 0x65 0x6C 0x6C 0x6F };
var message = new Message
{
    Headers = headers,
    Payload = payload
};

// Encode the message to a byte array
byte[] encodedData = codec.Encode(message);

// Decode the byte array back to a Message object
Message decodedMessage = codec.Decode(encodedData);
```

## Testing
The library comes with unit tests to ensure its correctness. To run the tests, use the following command: 
`dotnet test`

## Contributing
Contributions are welcome! If you find any issues or have suggestions for improvements, please feel free to open an issue or submit a pull request.
