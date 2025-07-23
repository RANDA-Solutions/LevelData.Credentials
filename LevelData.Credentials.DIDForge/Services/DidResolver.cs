using LevelData.Credentials.DIDForge.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LevelData.Credentials.DIDForge.Services
{
    public class DidResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public DidResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IDidResolver GetResolver(string did)
        {
            var method = did.Split(':')[1]; // Extract the DID method (e.g., "key" or "web")
            return method switch
            {
                "key" => _serviceProvider.GetRequiredService<DidKeyResolver>(),
                "web" => _serviceProvider.GetRequiredService<DidWebResolver>(),
                _ => throw new NotSupportedException($"DID method '{method}' is not supported.")
            };
        }
    }
}
