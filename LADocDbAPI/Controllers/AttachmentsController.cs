#region

using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
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
    public class AttachmentsController : ApiController
    {
        /// <summary>
        /// Get List Of Attachments
        /// </summary>
        /// <param name="ridDb">DatabaseId</param>
        /// <param name="ridColl">CollectionId</param>
        /// <param name="ridDoc">DocumentId</param>
        /// <param name="query"></param>
        /// <param name="partitionKey">
        ///     Must be included if and only if the collection is created with a partitionKey definition
        /// </param>
        /// <returns></returns>
        [Metadata("GetAttachment", "Get a Attachment under a Document")]
        [HttpGet, Route("{ridDb}/colls/{ridColl}/docs/{ridDoc}/attachments")]
        [SwaggerOperation("GetListOfAttachments")]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IQueryable<dynamic> GetAttachment(
            string ridDb,
            string ridColl,
            string ridDoc,
            string query,
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
            var doclLink = UriFactory.CreateDocumentUri(ridDb, ridColl, ridDoc);
            return dbContext.ReadClient.CreateAttachmentQuery(doclLink, query, feedOptions);
        }


        /// <summary>
        /// Create an Attachment
        /// </summary>
        /// <param name="ridDb">DatabaseId</param>
        /// <param name="ridColl">CollectionId</param>
        /// <param name="ridDoc">DocunentId</param>
        /// <param name="mediaLink">You can reuse the mediaLink property to store an external location e.g., 
        /// a file share or an Azure Blob Storage URI.  
        /// DocumentDB will not perform garbage collection on mediaLinks for external locations.
        /// </param>
        /// <param name="partitionKey">
        ///     Must be included if and only if the collection is created with a partitionKey definition
        /// </param>
        /// <param name="slug">
        /// The name of the attachment(with extension). This is only **required** when raw media is submitted to the
        ///  DocumentDB attachment storage
        /// </param>
        /// <param name="mediaStream">Attachment File as a stream. Requires the Slug</param>
        /// <returns></returns>
        [Metadata("CreateAttachment",
            "There are two ways to create an attachment resource – post the media content to DocumentDB like in the AtomPub Protocol , or post just the attachment metadata to media stored externally.  " +
                "<br /><br />" +
                "Attachments can be created as managed or unmanaged. If attachments are created as managed through DocumentDB, then it is assigned a system generated mediaLink. DocumentDB then automatically performs garbage collection on the media when parent document is deleted "
            )]
        [HttpPost, Route("{ridDb}/colls/{ridColl}/docs/{ridDoc}/attachments")]
        [SwaggerOperation("CreateAttachment")]
        [SwaggerResponse(HttpStatusCode.Created)]
        [SwaggerResponse(HttpStatusCode.BadRequest, "The JSON body is invalid")]
        [SwaggerResponse(HttpStatusCode.Conflict,
            "The id or Slug provided for the new attachment has been taken by an existing attachment."
            )]
        [SwaggerResponse(HttpStatusCode.RequestEntityTooLarge,
            "The document size in the request exceeded the allowable document size in a request, which is 512k")]
        public async Task<ResourceResponse<Attachment>> CreateAttachment(
           [Metadata("DatabaseId")]  string ridDb,
           [Metadata("CollectionId")]  string ridColl,
            string ridDoc,
            object mediaLink = null,
            string partitionKey = null,
            string slug = null,
            Stream mediaStream = null)
        {
            var doclLink = UriFactory.CreateDocumentUri(ridColl, ridColl, ridDoc);
            var contentType = string.Empty;
            if (!string.IsNullOrEmpty(slug))
            {
                contentType = MimeMapping.GetMimeMapping(slug);
            }


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


            if (mediaStream == null && mediaLink != null)
            {
                return
                    await
                        dbContext.Client.CreateAttachmentAsync(doclLink, mediaLink, requestOptions).ConfigureAwait(false);
            }
            return await dbContext.Client.CreateAttachmentAsync(doclLink, mediaStream,
                new MediaOptions {ContentType = contentType, Slug = slug  }, requestOptions).ConfigureAwait(false);
        }

        
    }
}