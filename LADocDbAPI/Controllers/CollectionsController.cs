#region

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
    public class CollectionsController : ApiController
    {
        /// <summary>
        ///     Query Documents
        /// </summary>
        /// <param name="ridDb">DatabaseId</param>
        /// <param name="ridColl">CollectionId</param>
        /// <param name="query">SQL Query</param>
        /// <returns></returns>
        [Metadata("QueryDocuments",
            "You can query arbitrary JSON documents in a collection by performing a POST against the “colls” resource in DocumentDB. The SQL syntax of DocumentDB provides hierarchical, relational, and spatial query operators to query and project documents"
            )]
        [HttpPost, Route("{ridDb}/colls{ridColl}/docs")]
        [SwaggerOperation("QueryDocuments")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound, "The document no longer exists, i.e. the document was deleted")]
        public IQueryable<dynamic> QueryDocuments(
            [Metadata("DatabaseId")] string ridDb,
            [Metadata("CollectionId")] string ridColl,
            [Metadata("Query")] SqlQuerySpec query)
        {
            var dbContext = new DocumentDbContext();
            var doclLink = UriFactory.CreateDocumentCollectionUri(ridDb, ridColl);

            return dbContext.ReadClient.CreateDocumentQuery(doclLink, query);
        }

        /// <summary>
        ///     Create new Collection
        /// </summary>
        /// <param name="ridDb">DatabaseId</param>
        /// <param name="id">Collection Id</param>
        /// <param name="indexingDirective">Default | Include | Exclude</param>
        /// <param name="partitionKey">Partition Definitions</param>
        /// <returns></returns>
        [Metadata("CreateCollection",
            "Create a new Collection"
            )]
        [HttpPost, Route("{ridDb}/colls")]
        [SwaggerOperation("CreateCollection")]
        [SwaggerResponse(HttpStatusCode.Created)]
        [SwaggerResponse(HttpStatusCode.BadRequest,
            "This means something was wrong with the request supplied. It is likely that an id was not supplied for the new collection"
            )]
        [SwaggerResponse(HttpStatusCode.Conflict,
            "This means a DocumentCollection with an id matching the id you supplied already existed.")]
        [SwaggerResponse(HttpStatusCode.Forbidden,
            "This means you attempted to exceed your quota for collections. Contact support to have this quota increased."
            )]
        public Task<ResourceResponse<DocumentCollection>> CreateCollection(
            [Metadata("DatabaseId")] string ridDb,
            [Metadata("CollectionId")] string id,
            [Metadata("IndexingDirective", "Default | Include | Exclude")] IndexingDirective indexingDirective,
            [Metadata("Partition Key")] string partitionKey = null)
        {
            var dbContext = new DocumentDbContext();
            var doclLink = UriFactory.CreateDatabaseUri(ridDb);
            var requestOptions = new RequestOptions {IndexingDirective = indexingDirective};

            if (partitionKey != null)
            {
                requestOptions.PartitionKey = new PartitionKey(partitionKey);
            }


            return dbContext.Client.CreateDocumentCollectionAsync(doclLink, new DocumentCollection {Id = id},
                requestOptions);
        }
    }
}