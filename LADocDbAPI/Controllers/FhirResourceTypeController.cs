using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Swashbuckle.Swagger.Annotations;
using TRex.Metadata;

namespace LADocDbAPI.Controllers
{
    /// <summary>
    ///     FHIR Resource Type Controller
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class FhirResourceTypeController : ApiController
    {
        /// <summary>
        ///     Gets the new or modified FHIR documents from Last Run Date 
        ///     or Create Date of the Collection
        /// </summary>
        /// <param name="databaseId"></param>
        /// <param name="collectionId"></param>
        /// <param name="resourceType"></param>
        /// <param name="startfromBeginning"></param>
        /// <returns></returns>
        [Metadata("Get New or Modified FHIR Documents",
            "Query for new or modifed FHIR Documents By Resource Type " +
            "from Last Run Date or Begiining of Collection creation"
        )]
        [SwaggerResponse(HttpStatusCode.OK, type: typeof(Task<dynamic>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "No New or Modifed Documents found")]
        [SwaggerOperation("GetNewOrModifiedFHIRDocuments")]
        public async Task<dynamic> GetNewOrModifiedFhirDocuments(
            [Metadata("Database Id", "Database Id")] string databaseId,
            [Metadata("Collection Id", "Collection Id")] string collectionId,
            [Metadata("Resource Type", "FHIR resource type name")] string resourceType,
            [Metadata("Start from Beginning ", "Change Feed Option")] bool startfromBeginning
        )
        {
            var collectionLink = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);

            var context = new DocumentDbContext();

            string pkRangesResponseContinuation = null;
            var docs = new List<dynamic>();

            var partitionKeyRanges = new List<PartitionKeyRange>();


            do
            {
                var pkRangesResponse = await context.Client.ReadPartitionKeyRangeFeedAsync(
                    collectionLink, new FeedOptions {RequestContinuation = pkRangesResponseContinuation});

                partitionKeyRanges.AddRange(pkRangesResponse);
                pkRangesResponseContinuation = pkRangesResponse.ResponseContinuation;
            } while (pkRangesResponseContinuation != null);

            foreach (var pkRange in partitionKeyRanges)
            {
                var changeFeedOptions = new ChangeFeedOptions
                {
                    StartFromBeginning = startfromBeginning,
                    RequestContinuation = null,
                    MaxItemCount = -1,
                    PartitionKeyRangeId = pkRange.Id
                };

                using (var query = context.Client.CreateDocumentChangeFeedQuery(collectionLink, changeFeedOptions))
                {
                    do
                    {
                        if (query != null)
                        {
                            var results = await query.ExecuteNextAsync<dynamic>();
                            if (results.Count > 0)
                                docs.AddRange(results.Where(doc => doc.resourceType == resourceType));
                        }
                        else
                        {
                            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
                        }
                    } while (query.HasMoreResults);
                }
            }
            if (docs.Count > 0)
                return docs;
            var msg = new StringContent("No documents found for " + resourceType + " Resource");
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = msg
            };
            return response;
        }
    }
}