#region

using System;
using System.Configuration;
using Microsoft.Azure.Documents.Client;

#endregion

namespace LADocDbAPI
{
    /// <summary>
    /// </summary>
    public class DocumentDbContext : IDisposable
    {
        /// <summary>
        /// </summary>
        public static string EndPoint = ConfigurationManager.AppSettings["endpoint"];

        /// <summary>
        /// </summary>
        public static string AuthKey = ConfigurationManager.AppSettings["masterKey"];


        /// <summary>
        /// 
        /// </summary>
        public static string ReadKey = ConfigurationManager.AppSettings["ReadKey"];

       

        /// <summary>
        /// </summary>
        public static readonly ConnectionPolicy ConnectionPolicy = new ConnectionPolicy
        {
            UserAgentSuffix = "DocumentDbApi"
        };

        private DocumentClient _client;

        /// <summary>
        ///     Create the connection
        /// </summary>
        public DocumentClient Client
        {
            get
            {
                if (_client != null) return _client;
                var endpointUri = new Uri(EndPoint);
                _client = new DocumentClient(endpointUri, AuthKey, new ConnectionPolicy
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConnectionProtocol = Protocol.Tcp,
                    UserAgentSuffix = "DocumentDbApi"
                });

                return _client;
            }
        }

        /// <summary>
        ///  Read only Key
        /// </summary>
        public DocumentClient ReadClient
        {
            get
            {
                if (_client != null) return _client;
                var endpointUri = new Uri(EndPoint);
                _client = new DocumentClient(endpointUri, ReadKey, new ConnectionPolicy
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConnectionProtocol = Protocol.Tcp,
                    UserAgentSuffix = "DocumentDbApi"
                });

                return _client;
            }

        }

        /// <summary>
        /// </summary>
        public static string CollectionLink { get; set; }

        #region Implementation of IDisposable

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}