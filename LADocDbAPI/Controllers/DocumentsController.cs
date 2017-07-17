#region

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
    public class DocumentsController : ApiController
    {
        /// <summary>
        ///     Get a List of Douments
        /// </summary>
        /// <param name="ridDb">DatabaseId</param>
        /// <param name="ridColl">CollectionId</param>
        /// <param name="partitionKey">Must be included if and only if the collection is created with a partitionKey definition</param>
        /// <returns>FeedResponse</returns>
        [Metadata("GetListOfDouments",
            "While consistency level is defined at the database account level during account creation, read consistency can be overridden to suffice the need of the application. The override is set per GET operation by setting the x-ms-consistency-level header to the desired level. The consistency override can only be the same or weaker than the level that was set during account creation Consistency levels from weakest to strongest is Eventual, Session, Bounded Staleness and Strong. \'For instance, if during account creation the consistency level was set to Session, the override cannot be Strong or Bounded. For information on consistent levels Consistency levels in DocumentDB."
            )]
        [HttpGet, Route("{ridDb}/colls/{ridColl}/")]
        [SwaggerOperation("GetListOfDouments")]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<FeedResponse<dynamic>> GetListOfDouments(
            string ridDb,
            string ridColl,
            string partitionKey = null)
        {
            var feedOptions = new FeedOptions();

            if (partitionKey != null)
            {
                feedOptions.PartitionKey = new PartitionKey(partitionKey);
            }
            else
            {
                feedOptions = null;
            }


            var dbContext = new DocumentDbContext();
            var doclLink = UriFactory.CreateDocumentCollectionUri(ridDb, ridColl);
            return await dbContext.ReadClient.ReadDocumentFeedAsync(doclLink, feedOptions).ConfigureAwait(false);
        }


        /// <summary>
        ///     Create a Document
        /// </summary>
        /// <param name="ridDb">DatabaseId</param>
        /// <param name="ridColl">CollectionId</param>
        /// <param name="document">Document</param>
        /// <param name="id">Unique Id for Document. If not set then an Unique  Indentifer will be set as the documentId</param>
        /// <param name="ttl">OPTIONAL: Time To Live (seconds) - Overrides Collection Setting for Document</param>
        /// <param name="indexingDirective">Default | Include | Exclude</param>
        /// <param name="partitionKey">OPTIONAL: If the collection is created with a partitionKey definition</param>
        /// <returns>ResourceResponse</returns>
        [Metadata("CreateDocument",
            "Documents are automatically indexed. If automatic indexing is turned off at the collection level, documents can be manually included or excluded from being indexed by using the x-ms-indexing-directive header during the POST operation. Documents must adhere to size limits, as specified in DocumentDB Limits."
            )]
        [HttpPost, Route("{ridDb}/colls/{ridColl}/")]
        [SwaggerOperation("CreateDocument")]
        [SwaggerResponse(HttpStatusCode.Created)]
        public async Task<ResourceResponse<Document>> CreateDocument(
            string ridDb,
            string ridColl,
            object document,

            IndexingDirective indexingDirective,
            string id = null,
            int? ttl = null,
            string partitionKey = null)
        {
            var requestOptions = new RequestOptions {IndexingDirective = indexingDirective};

            if (partitionKey != null)
            {
                requestOptions.PartitionKey = new PartitionKey(partitionKey);
            }


            var dbContext = new DocumentDbContext();
            var doclLink = UriFactory.CreateDocumentCollectionUri(ridDb, ridColl);
            if (id != null)
            {
                return
               await
                   dbContext.Client.CreateDocumentAsync(doclLink, new { id, ttl, document }, requestOptions, true)
                       .ConfigureAwait(false);
            }
            return await dbContext.Client.CreateDocumentAsync(doclLink, new {ttl, document}, requestOptions).ConfigureAwait(false);
        }
       
    


        /// <summary>
        ///     Delete a Document
        /// </summary>
        /// <param name="ridDb">DatabaseId</param>
        /// <param name="ridColl">CollectionId</param>
        /// <param name="ridDoc">DocumentId</param>
        /// <param name="partitionKey">Must be included if and only if the collection is created with a partitionKey definition</param>
        /// <returns>ResourceResponse</returns>
        [Metadata("DeleteDocument",
            "Performing a DELETE operation on a specific document resource deletes the document resource from the collection."
            )]
        [HttpDelete, Route("{ridDb}/colls/{ridColl}/docs/{ridDoc}")]
        [SwaggerOperation("DeleteDocument")]
        [SwaggerResponse(HttpStatusCode.NoContent)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<ResourceResponse<Document>> DeleteDocument(
            string ridDb,
            string ridColl,
            string ridDoc,
            string partitionKey = null)

        {
            var requestOptions = new RequestOptions();

            if (partitionKey != null)
            {
                requestOptions.PartitionKey = new PartitionKey(partitionKey);
            }
            else
            {
                requestOptions = null;
            }
         

            var dbContext = new DocumentDbContext();
            var doclLink = UriFactory.CreateDocumentUri(ridDb, ridColl, ridDoc);
            return await dbContext.Client.DeleteDocumentAsync(doclLink, requestOptions).ConfigureAwait(false);
        }


        /// <summary>
        ///     Get a Document By its Id
        /// </summary>
        /// <param name="ridDb">DatabaseId</param>
        /// <param name="ridColl">CollectionId</param>
        /// <param name="ridDoc">DocumentId</param>
        /// <param name="partitionKey">
        ///     The partition key value for the document to be read. Must be included if and only if the
        ///     collection is created with a partitionKey definition
        /// </param>
        /// <returns>ResourceResponse</returns>
        [Metadata("GetDocumentById",
            "While consistency level is defined at the database account level during account creation, read consistency can be overridden to suffice the need of the application. The override is set per GET operation by setting the x-ms-consistency-level header to the desired level. The consistency override can only be the same or weaker than the level that was set during account creation Consistency levels from weakest to strongest is Eventual, Session, Bounded Staleness and Strong. \'For instance, if during account creation the consistency level was set to Session, the override cannot be Strong or Bounded. For information on consistent levels Consistency levels in DocumentDB."
            )]
        [HttpGet, Route("{ridDb}/colls/{ridColl}/docs/{ridDoc}")]
        [SwaggerOperation("GetDocumentById")]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<ResourceResponse<Document>> GetDocumentById(
            string ridDb,
            string ridColl,
            string ridDoc,
            string partitionKey = null)
        {
            var requestOptions = new RequestOptions();

            if (partitionKey != null)
            {
                requestOptions.PartitionKey = new PartitionKey(partitionKey);
            }
            else
            {
                requestOptions = null;
            }

            var dbContext = new DocumentDbContext();
            var doclLink = UriFactory.CreateDocumentUri(ridDb, ridColl, ridDoc);
            return await dbContext.ReadClient.ReadDocumentAsync(doclLink, requestOptions).ConfigureAwait(false);
        }


        /// <summary>
        ///     Upsert a Document
        /// </summary>
        /// <param name="ridDb">DatabaseId</param>
        /// <param name="ridColl">CollectionId</param>
        /// <param name="document">Document - required when propertyName/ propertyValue are null </param>
        /// <param name="propertyName">Name of dynamic property</param>
        /// <param name="propertyValue">Value of dynamic property</param>
        /// <param name="partitionKey">
        ///     Must be included if and only if the collection is created with a partitionKey definition
        /// </param>
        /// <returns>ResourceResponse</returns>
        [Metadata("UpsertDocument",
            "Performing a POST operation on a specific document resource replaces the entire document resource. All user settable properties, including the id, and the user defined JSON elements must be submitted in the body to perform the replacement- Any element omissions result in unintended data loss as this operation is a full replace operation" +
                "<br /><br />" +
                "Besides replacing the body, you can add a dynamic property (object) with value.  A Document can extend Resources ."
            )]
        [HttpPost, Route("{ridDb}/colls/{ridColl}/docs")]
        [SwaggerOperation("UpsertDocument")]
        [SwaggerResponse(HttpStatusCode.Created)]
        [SwaggerResponse(HttpStatusCode.NotFound, "The document no longer exists, i.e. the document was deleted")]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.Conflict)]
        [SwaggerResponse(HttpStatusCode.RequestEntityTooLarge,
            "The document size in the request exceeded the allowable document size in a request")]
        public async Task<ResourceResponse<Document>> UpsertDocument(
            string ridDb,
            string ridColl,
            object document = null,
            string propertyName = null,
            string propertyValue = null,
            string partitionKey = null)
        {
            var dbContext = new DocumentDbContext();
            var doclLink = UriFactory.CreateDocumentCollectionUri(ridDb, ridColl);
            var requestOptions = new RequestOptions();

            if (partitionKey != null)
            {
                requestOptions.PartitionKey = new PartitionKey(partitionKey);
            }
            else
            {
                requestOptions = null;
            }

            if (propertyName != null && propertyValue != null)
            {
                return
                    await
                        dbContext.Client.UpsertDocumentAsync(doclLink, new {propertyName = propertyValue})
                            .ConfigureAwait(false);
            }


            return await dbContext.Client.UpsertDocumentAsync(doclLink, document, requestOptions).ConfigureAwait(false);
        }

        /// <summary>
        ///     Replace a Document
        /// </summary>
        /// <param name="ridDb">DatabaseId</param>
        /// <param name="ridColl">CollectionId</param>
        /// <param name="ridDoc">DocumentId</param>
        /// <param name="document">Document</param>
        /// <param name="partitionKey">
        ///     Must be included if and only if the collection is created with a partitionKey definition
        /// </param>
        /// <returns>ResourceResponse</returns>
        [Metadata("ReplaceDocument",
            "Performing a POST operation on a specific document resource replaces the entire document resource. All user settable properties, including the id, and the user defined JSON elements must be submitted in the body to perform the replacement- Any element omissions result in unintended data loss as this operation is a full replace operation" +
                "<br /><br />" +
                "Besides replacing the body, you can add a dynamic property (object) with value.  A Document can extend Resources ."
            )]
        [HttpPost, Route("{ridDb}/colls/{ridColl}/docs/{ridDoc}")]
        [SwaggerOperation("ReplaceDocument")]
        [SwaggerResponse(HttpStatusCode.Created)]
        [SwaggerResponse(HttpStatusCode.NotFound, "The document no longer exists, i.e. the document was deleted")]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.Conflict)]
        [SwaggerResponse(HttpStatusCode.RequestEntityTooLarge,
            "The document size in the request exceeded the allowable document size in a request")]
        public async Task<ResourceResponse<Document>> ReplaceDocument(
            string ridDb,
            string ridColl,
            string ridDoc,
            object document,
            string partitionKey = null)
        {
            var dbContext = new DocumentDbContext();
            var doclLink = UriFactory.CreateDocumentUri(ridDb, ridColl, ridDoc);
            var requestOptions = new RequestOptions();

            if (partitionKey != null)
            {
                requestOptions.PartitionKey = new PartitionKey(partitionKey);
            }
            else
            {
                requestOptions = null;
            }


            return await dbContext.Client.ReplaceDocumentAsync(doclLink, document, requestOptions).ConfigureAwait(false);
        }
    }
}