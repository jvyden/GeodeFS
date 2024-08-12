using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using GeodeFS.Api;
using Org.BouncyCastle.Bcpg.OpenPgp;
using PgpCore;
using PgpCore.Models;

static string BytesToHexString(ReadOnlySpan<byte> data)
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

EncryptionKeys keys = new(File.ReadAllText("/home/jvyden/test.pub"),File.ReadAllText("/home/jvyden/test.key"), "test");

Console.WriteLine(keys.PrivateKey.KeyId);

using PGP pgp = new(keys);
foreach (PgpPublicKeyRingWithPreferredKey ring in pgp.EncryptionKeys.PublicKeyRings)
{
   foreach (PgpPublicKey key in ring.PgpPublicKeyRing.GetPublicKeys())
   {
      foreach (string userId in key.GetUserIds())
      {
         Console.WriteLine(userId);
         Console.WriteLine(BytesToHexString(key.GetFingerprint()));
      }
   }
}

// Console.WriteLine(BytesToHexString(keys.SecretKey.GetFingerprint()));
// Console.WriteLine();

using HttpClient client = new();
client.BaseAddress = new Uri("http://127.0.0.1:10061/api/");

HttpResponseMessage response = await client.PostAsync("register", new StringContent(JsonSerializer.Serialize(new RegistrationRequest
{
   Message = pgp.ClearSignArmoredString("register"),
   Pubkey = File.ReadAllText("/home/jvyden/test.pub"),
})));

response.EnsureSuccessStatusCode();
Console.WriteLine(await response.Content.ReadAsStringAsync());