using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelData.Credentials.DIDForge.Helpers
{
    public static class UriToDidWebHelper
    {
        public static string ConvertUriToDidWeb(string uri)
        {
            // Ensure the URI is valid and starts with "https://"
            if (String.IsNullOrWhiteSpace(uri) || !uri.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("URI must start with 'https://'", nameof(uri));
            }
            // Remove the "https://" prefix
            string didWeb = uri.Substring("https://".Length);
            // Replace slashes with colons
            didWeb = didWeb.Replace("/", ":");
            // Prepend the "did:web:" prefix
            return $"did:web:{didWeb}";
        }
    }
}
