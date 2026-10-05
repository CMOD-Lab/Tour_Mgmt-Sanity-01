using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Tour_Management.Web.Tests
{
    /// <summary>
    /// A simple in-memory ISession implementation for unit testing.
    /// </summary>
    public class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _store = new Dictionary<string, byte[]>();

        public bool IsAvailable => true;
        public string Id => Guid.NewGuid().ToString();
        public IEnumerable<string> Keys => _store.Keys;

        public void Clear() => _store.Clear();

        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public void Remove(string key) => _store.Remove(key);

        public void Set(string key, byte[] value) => _store[key] = value;

        public bool TryGetValue(string key, out byte[] value)
        {
            return _store.TryGetValue(key, out value!);
        }
    }

    /// <summary>
    /// Extension methods for TestSession to mimic ISession extension methods.
    /// Uses the same encoding as Microsoft.AspNetCore.Http.SessionExtensions.
    /// </summary>
    public static class TestSessionExtensions
    {
        public static void SetString(this ISession session, string key, string value)
        {
            session.Set(key, Encoding.UTF8.GetBytes(value));
        }

        public static string? GetString(this ISession session, string key)
        {
            if (session.TryGetValue(key, out var data))
                return Encoding.UTF8.GetString(data);
            return null;
        }

        /// <summary>
        /// Sets an int32 value using big-endian byte order (matching ASP.NET Core SessionExtensions).
        /// </summary>
        public static void SetInt32(this ISession session, string key, int value)
        {
            var bytes = new byte[]
            {
                (byte)(value >> 24),
                (byte)(0xFF & (value >> 16)),
                (byte)(0xFF & (value >> 8)),
                (byte)(0xFF & value)
            };
            session.Set(key, bytes);
        }

        /// <summary>
        /// Gets an int32 value using big-endian byte order (matching ASP.NET Core SessionExtensions).
        /// </summary>
        public static int? GetInt32(this ISession session, string key)
        {
            if (!session.TryGetValue(key, out var data) || data.Length < 4)
                return null;
            return data[0] << 24 | data[1] << 16 | data[2] << 8 | data[3];
        }
    }
}
