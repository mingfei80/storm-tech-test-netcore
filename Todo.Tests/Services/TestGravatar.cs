using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Todo.Tests.Services
{
    public static class Gravatar
    {
        private static HttpClient _httpClient = new HttpClient();

        // Property to allow setting HttpClient for testing
        public static HttpClient HttpClientInstance
        {
            get => _httpClient;
            set => _httpClient = value;
        }

        public static async Task<string> GetGravatarProfileNameAsync(string email)
        {
            try
            {
                var emailHash = GetHash(email.Trim().ToLower());
                var requestUrl = $"https://en.gravatar.com/{emailHash}.json";

                var response = await _httpClient.GetAsync(requestUrl);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var gravatarProfile = JsonSerializer.Deserialize<GravatarProfile>(jsonResponse);
                    return gravatarProfile?.entry?[0]?.displayName ?? email;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            return email; // Fallback to email if something goes wrong
        }

        public static string GetHash(string emailAddress)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.Default.GetBytes(emailAddress.Trim().ToLowerInvariant());
                var hashBytes = md5.ComputeHash(inputBytes);

                var builder = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    builder.Append(b.ToString("X2"));
                }
                return builder.ToString().ToLowerInvariant();
            }
        }

        private class GravatarProfile
        {
            public GravatarEntry[] entry { get; set; }
        }

        private class GravatarEntry
        {
            public string displayName { get; set; }
        }
    }
}

