using System.Security.Cryptography;
using System.Text;

namespace GeodeFS.Common.Verification;

public static class CryptoHelper
{
    private static string BytesToHexString(ReadOnlySpan<byte> data)
    {
        Span<char> hexChars = stackalloc char[data.Length * 2];

        for (int i = 0; i < data.Length; i++)
        {
            byte b = data[i];
            hexChars[i * 2] = GetHexChar(b >> 4); // High bits
            hexChars[i * 2 + 1] = GetHexChar(b & 0x0F); // Low bits
        }

        return new string(hexChars);

        static char GetHexChar(int value)
        {
            return (char)(value < 10 ? '0' + value : 'a' + value - 10);
        }
    }

    public static string Sha256(string data) => Sha256(Encoding.UTF8.GetBytes(data));
    public static string Sha256(ReadOnlySpan<byte> data) => BytesToHexString(SHA256.HashData(data));
}