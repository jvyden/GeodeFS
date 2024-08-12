using System.Net.Http.Json;
using GeodeFS.Api;
using Org.BouncyCastle.Bcpg.OpenPgp;
using PgpCore;

EncryptionKeys keys = new(File.ReadAllText("/home/jvyden/test.key"), "test");

Console.WriteLine(keys.PrivateKey.KeyId);

using PGP pgp = new(keys);
foreach (PgpSecretKeyRing ring in pgp.EncryptionKeys.SecretKeys.GetKeyRings())
{
   foreach (PgpSecretKey key in ring.GetSecretKeys())
   {
      foreach (string userId in key.UserIds)
      {
         Console.WriteLine(userId);
      }
   }
}

using HttpClient client = new();
client.BaseAddress = new Uri("http://127.0.0.1:10061/api/");

await client.PostAsJsonAsync("register", new RegistrationRequest
{
   Message = pgp.ClearSignArmoredString("register"),
   Pubkey = File.ReadAllText("/home/jvyden/test.pub"),
});