using System.Linq;
using System.Net;
using System.Web.Http;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Swashbuckle.Swagger.Annotations;
using TRex.Metadata;

namespace LADocDbAPI.Controllers
{
    /// <summary>
    /// </summary>
    public class QueryController : ApiController
    {
        /// <summary>
        ///     Query Documents in a Collection
        /// </summary>
        /// <param name="ridDb">DatabaseId</param>
        /// <param name="ridColl">CollectionId</param>
        /// <param name="sqlExpression">The SQL Query</param>
        /// <param name="partitionKey">
        ///     The partition key value for the documents to be read. Must be included
        /// </param>
        /// <returns>ResourceResponse</returns>
        [Metadata("QuueyDocumentsFromPartition")]
        [Route("{ridDb}/colls/{ridColl}")]
        [SwaggerOperation("QuueyDocuments")]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IQueryable<dynamic> QuueyDocumentsFromPartition(
            [Metadata("DatabaseId")] string ridDb,
            [Metadata("CollectionId")] string ridColl,
            [Metadata("Query")] string sqlExpression,
            [Metadata("PartitionKey")] string partitionKey)
        {
            var feedOptions = new FeedOptions
            {
                EnableCrossPartitionQuery = true,
                PartitionKey = new PartitionKey(partitionKey),
                MaxDegreeOfParallelism = -1
            };


            var dbContext = new DocumentDbContext();
            var doclLink = UriFactory.CreateDocumentCollectionUri(ridDb, ridColl);

            return dbContext.Client.CreateDocumentQuery<dynamic>(doclLink, sqlExpression, feedOptions);
        }
    }
}