// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Net;

namespace Garnet.client.test
{
    /// <summary>
    /// Configuration for client tests.
    /// Reads endpoint from GARNET_TEST_ENDPOINT environment variable (format: "host:port").
    /// Defaults to 127.0.0.1:6379 if not set.
    /// </summary>
    internal static class TestConfig
    {
        /// <summary>
        /// Get the Garnet server endpoint to connect to.
        /// Reads from GARNET_TEST_ENDPOINT environment variable (format: "host:port").
        /// Supports both IP addresses and hostnames. Defaults to 127.0.0.1:6379.
        /// </summary>
        public static EndPoint GetEndPoint()
        {
            var envEndpoint = Environment.GetEnvironmentVariable("GARNET_TEST_ENDPOINT");
            if (!string.IsNullOrEmpty(envEndpoint))
            {
                var lastColon = envEndpoint.LastIndexOf(':');
                if (lastColon > 0 && int.TryParse(envEndpoint.AsSpan(lastColon + 1), out var port))
                {
                    var host = envEndpoint[..lastColon];
                    // Use IPEndPoint for IP addresses, DnsEndPoint for hostnames
                    if (IPAddress.TryParse(host, out var ipAddress))
                        return new IPEndPoint(ipAddress, port);
                    return new DnsEndPoint(host, port);
                }
            }
            return new IPEndPoint(IPAddress.Loopback, 6379);
        }

        /// <summary>
        /// Create a new GarnetClient connected to the configured endpoint.
        /// </summary>
        public static GarnetClient GetGarnetClient(bool recordLatency = false)
            => new GarnetClient(GetEndPoint(), recordLatency: recordLatency);
    }
}
