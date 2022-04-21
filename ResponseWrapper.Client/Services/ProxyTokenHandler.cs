using Newtonsoft.Json;
using System;
using System.Text;

namespace ResponseWrapper.Client.Services
{
    public class ProxyTokenHandler
    {
        public static string DecodeProxyToken(string proxy)
        {
            var dec = Base64UrlEncoder.Decode(proxy);

            var proxyToken = JsonConvert.DeserializeObject<ProxyTokenResponse>(dec);

            return proxyToken.access_token;
        }
        internal class ProxyTokenResponse
        {
            public string proxy_token { get; set; }

            public string access_token { get; set; }
        }

        static class Base64UrlEncoder
        {
            private static char base64PadCharacter = '=';

            private static string doubleBase64PadCharacter = "==";

            private static char base64Character62 = '+';

            private static char base64Character63 = '/';

            private static char base64UrlCharacter62 = '-';

            private static char _base64UrlCharacter63 = '_';

            /// <summary>
            /// The following functions perform base64url encoding which differs from regular base64 encoding as follows
            /// * padding is skipped so the pad character '=' doesn't have to be percent encoded
            /// * the 62nd and 63rd regular base64 encoding characters ('+' and '/') are replace with ('-' and '_')
            /// The changes make the encoding alphabet file and URL safe.
            /// </summary>
            /// <param name="arg">string to encode.</param>
            /// <returns>Base64Url encoding of the UTF8 bytes.</returns>
            internal static string Encode(string arg)
            {
                if (arg == null)
                {
                    //throw LogHelper.LogArgumentNullException("arg");
                }
                return Encode(Encoding.UTF8.GetBytes(arg));
            }

            /// <summary>
            /// Converts a subset of an array of 8-bit unsigned integers to its equivalent string representation that is encoded with base-64-url digits. Parameters specify
            /// the subset as an offset in the input array, and the number of elements in the array to convert.
            /// </summary>
            /// <param name="inArray">An array of 8-bit unsigned integers.</param>
            /// <param name="length">An offset in inArray.</param>
            /// <param name="offset">The number of elements of inArray to convert.</param>
            /// <returns>The string representation in base 64 url encodingof length elements of inArray, starting at position offset.</returns>
            /// <exception cref="T:System.ArgumentNullException">'inArray' is null.</exception>
            /// <exception cref="T:System.ArgumentOutOfRangeException">offset or length is negative OR offset plus length is greater than the length of inArray.</exception>
            internal static string Encode(byte[] inArray, int offset, int length)
            {
                if (inArray == null)
                {
                    //throw LogHelper.LogArgumentNullException("inArray");
                }
                return Convert.ToBase64String(inArray, offset, length).Split(new char[1]
                {
            base64PadCharacter
                })[0].Replace(base64Character62, base64UrlCharacter62).Replace(base64Character63, _base64UrlCharacter63);
            }

            /// <summary>
            /// Converts a subset of an array of 8-bit unsigned integers to its equivalent string representation that is encoded with base-64-url digits. Parameters specify
            /// the subset as an offset in the input array, and the number of elements in the array to convert.
            /// </summary>
            /// <param name="inArray">An array of 8-bit unsigned integers.</param>
            /// <returns>The string representation in base 64 url encodingof length elements of inArray, starting at position offset.</returns>
            /// <exception cref="T:System.ArgumentNullException">'inArray' is null.</exception>
            /// <exception cref="T:System.ArgumentOutOfRangeException">offset or length is negative OR offset plus length is greater than the length of inArray.</exception>
            internal static string Encode(byte[] inArray)
            {
                if (inArray == null)
                {
                    //throw LogHelper.LogArgumentNullException("inArray");
                }
                return Convert.ToBase64String(inArray, 0, inArray.Length).Split(new char[1]
                {
            base64PadCharacter
                })[0].Replace(base64Character62, base64UrlCharacter62).Replace(base64Character63, _base64UrlCharacter63);
            }

            /// <summary>
            ///  Converts the specified string, which encodes binary data as base-64-url digits, to an equivalent 8-bit unsigned integer array.</summary>
            /// <param name="str">base64Url encoded string.</param>
            /// <returns>UTF8 bytes.</returns>
            internal static byte[] DecodeBytes(string str)
            {
                if (str == null)
                {
                    //throw LogHelper.LogExceptionMessage(new ArgumentNullException("str"));
                }
                str = str.Replace(base64UrlCharacter62, base64Character62);
                str = str.Replace(_base64UrlCharacter63, base64Character63);
                switch (str.Length % 4)
                {
                    case 2:
                        str += doubleBase64PadCharacter;
                        break;
                    case 3:
                        str += base64PadCharacter.ToString();
                        break;
                    default:
                    //throw LogHelper.LogExceptionMessage(new FormatException(LogHelper.FormatInvariant("IDX10400: Unable to decode: '{0}' as Base64url encoded string.", str)));
                    case 0:
                        break;
                }
                return Convert.FromBase64String(str);
            }

            /// <summary>
            /// Decodes the string from Base64UrlEncoded to UTF8.
            /// </summary>
            /// <param name="arg">string to decode.</param>
            /// <returns>UTF8 string.</returns>
            internal static string Decode(string arg)
            {
                return Encoding.UTF8.GetString(DecodeBytes(arg));
            }
        }
    }
}
