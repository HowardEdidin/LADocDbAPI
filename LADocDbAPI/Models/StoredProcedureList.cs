using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Documents.Client;

namespace LADocDbAPI.Models
{
    /// <summary>
    /// </summary>
    public class StoredProcedureList
    {
        //private static readonly ConnectionPolicy ConnectionPolicy = new ConnectionPolicy
        //{
        //    UserAgentSuffix = "DocumentDbApi-LogicApps/1"
        //};

        /// <summary>
        ///     s
        /// </summary>
        public static List<dynamic> SprocList => GetListofProcedureIds();

        /// <summary>
        /// </summary>
        /// <summary>
        /// </summary>
        /// <returns></returns>
        private static List<dynamic> GetListofProcedureIds()
        {
            var collectionLink = UriFactory.CreateDocumentCollectionUri(DocumentDbContext.DatabaseId,
                DocumentDbContext.CollectionId);

            using (
                var client = new DocumentClient(new Uri(DocumentDbContext.EndPoint), DocumentDbContext.AuthKey))
            {
                var sprocIds =
                    client.CreateStoredProcedureQuery(collectionLink, "SELECT VALUE s.id FROM sprocs s").ToList();

                return sprocIds;
            }
        }


/*
        private static List<string> GetListofProcedureInputs()
        {

           var results = new List<string>();

            var collectionLink = UriFactory.CreateDocumentCollectionUri(DocumentDbContext.DatabaseId,
                DocumentDbContext.CollectionId);

            using (
                var client = new DocumentClient(new Uri(DocumentDbContext.EndPoint), DocumentDbContext.AuthKey,
                    ConnectionPolicy))
            {
             var  list =
                    client.CreateStoredProcedureQuery(collectionLink, "SELECT s.body FROM sprocs s").ToList();

               

                foreach (var item in list)
                {
                
                    results.AddRange(item);
                    
                }
                return results;
            }
        }
*/
    }
}