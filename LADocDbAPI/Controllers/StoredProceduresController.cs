#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Swashbuckle.Swagger.Annotations;
using TRex.Metadata;

#endregion

namespace LADocDbAPI.Controllers
{
    /// <summary>
    /// </summary>
    public class StoredProceduresController : ApiController
    {
     


        /// <summary>
        ///  Excecute StoredProcedure
        /// </summary>
        /// <param name="ridDb">DatabaseId</param>
        /// <param name="ridColl">CollectionId</param>
        /// <param name="ridProc">ProcedureId</param>
        /// <param name="partitionKey">
        ///     Must be included if and only if the collection is created with a partitionKey definition
        /// </param>
        /// <param name="procedureParams">Parameters</param>
        /// <returns>StoredProcedureResponse</returns>
        [Metadata("ExcecuteStoredProcedure",
            " For information on how stored procedures work, including execution of a stored procedure, see [DocumentDB programming Stored procedures](http://azure.microsoft.com/eus/documentation/articles/documentdb-programming/)"
            )]
        [HttpPost, Route("{ridDb}/colls{ridColl}/procs/{ridProc}")]
        [SwaggerOperation("ExcecuteStoredProcedure")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.BadRequest, "The syntax is incorrect")]
        public async Task<StoredProcedureResponse<dynamic>> ExcecuteStoredProcedure(
            [Metadata("DatabaseId")] string ridDb,
            [Metadata("ProcedureId")] string ridProc,
            [Metadata("CollectionId")] string ridColl,
            [Metadata("Partition Key")] string partitionKey = null,
            [Metadata("Procedure Input Parameters")] params dynamic[] procedureParams)

        {
           
            
            var doclLink = UriFactory.CreateStoredProcedureUri(ridDb, ridColl, ridProc);

            
            var feedOptions = new FeedOptions();

            if (partitionKey != null)
            {
                feedOptions.PartitionKey = new PartitionKey(partitionKey);
            }
            else
            {
                feedOptions = null;
            }

            using (
                var client = new DocumentClient(new Uri(DocumentDbContext.EndPoint), DocumentDbContext.AuthKey))
            {
                return
                    await
                        client.ExecuteStoredProcedureAsync<dynamic>(doclLink, procedureParams, feedOptions)
                            .ConfigureAwait(false);
            }
        }

        /// <summary>
        ///     Get List of Stored Procedures in a Collection
        /// </summary>
        /// <param name="ridDb">DatabaseId</param>
        /// <param name="ridColl">CollectionId</param>
        /// <returns>List</returns>
        [Metadata("GetListofStoredProcedures", "Get List of Stored Procedures in a Collection")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public IList<dynamic> GetListofProcedures(
            [Metadata("DatabaseId")] string ridDb,
            [Metadata("CollectionId")] string ridColl)
        {
            var collectionLink = UriFactory.CreateDocumentCollectionUri(ridDb, ridColl);

            using (
                var client = new DocumentClient(new Uri(DocumentDbContext.EndPoint), DocumentDbContext.ReadKey))
            {
                return
                   client.CreateStoredProcedureQuery(collectionLink, "SELECT VALUE s.id FROM sprocs s").ToList();
            }
        }
    }
}