using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Brand;
using Tigerspike.Solv.Domain.Enums;

namespace Tigerspike.Solv.Application.Interfaces
{
	public interface IBrandService
	{
		/// <summary>
		/// Returns single brand details
		/// </summary>
		/// <param name="id">Id of the brand</param>
		/// <returns>Brand details</returns>
		Task<BrandModel> Get(Guid brandId);

		/// <summary>
		/// Returns brand public details
		/// </summary>
		/// <param name="id">Id of the brand</param>
		/// <returns>Brand public details</returns>
		Task<BrandPublicModel> GetPublicProfile(Guid brandId);

		/// <summary>
		/// Get abandon reasons associated with brand
		/// </summary>
		/// <param name="brandId">The brand id</param>
		Task<IEnumerable<AbandonReasonModel>> GetAbandonReasons(Guid brandId);

		/// <summary>
		/// Get tags associated with brand
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="activeOnly">To get all tags or just active ones</param>
		/// <param name="level">The ticket level</param>
		Task<IEnumerable<TagModel>> GetTags(Guid brandId, bool activeOnly = true, TicketLevel? level = null);

		/// <summary>
		/// Assign the abandon reasons to brand.
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="abandonReasons">The abandon reasons</param>
		Task CreateAbandonReasons(Guid brandId, AbandonReasonModel[] abandonReasons);

		/// <summary>
		/// Assign the tags to brand.
		/// </summary>
		/// <param name="tags">The tags</param>
		Task CreateTags(CreateTagModel[] tags);

		/// <summary>
		/// Gets all brands available in the system
		/// </summary>
		/// <param name="includePractice">Whether to include the practice brands.</param>
		Task<List<BrandModel>> GetAll(bool includePractice = false);

		/// <summary>
		/// Returns brands associated with advocate
		/// </summary>
		/// <param name="advocateId">Id of the advocate</param>
		/// <param name="isPractice">Filter practice brands, pass null to fetch all regardless</param>
		/// <returns>List of brands and details of it's association with the advocate</returns>
		Task<IEnumerable<AdvocateBrandModel>> GetForAdvocate(Guid advocateId, bool? isPractice = null);

		/// <summary>
		/// Sets new ticket price for the brand
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="price">New price</param>
		/// <param name="userId">Author of the change</param>
		Task SetTicketPrice(Guid brandId, decimal price, Guid userId);

		/// <summary>
		/// Calculates ticket fee based on it's price and fee percentage
		/// </summary>
		/// <param name="ticketPrice">Ticket price</param>
		/// <param name="feePercentage">Fee percentage</param>
		/// <returns>Ticket fee, rounded to two decimal places</returns>
		decimal CalculateTicketFee(decimal ticketPrice, decimal feePercentage);

		/// <summary>
		/// Gets id of the brand associated with client
		/// </summary>
		/// <param name="clientId">The client i</param>
		/// <returns>Id of the brand</returns>
		Task<Guid> GetClientBrandId(Guid clientId);

		/// <summary>
		/// Gets the brand associated with client
		/// </summary>
		/// <param name="clientId">The client id</param>
		Task<BrandModel> GetClientBrand(Guid clientId);

		/// <summary>
		/// It generate the PayPal billing agreement and save the token generated for later verification
		/// The url will be returned
		/// </summary>
		/// <param name="brandId"></param>
		/// <returns></returns>
		Task<string> GenerateBillingAgreementUrl(Guid brandId);

		/// <summary>
		/// Setup payment for brand after acquiring the agreement token from PayPal.
		/// </summary>
		/// <param name="billingAgreementToken">The billing agreement token from PayPal</param>
		Task SetupPaymentAccount(Guid brandId, string billingAgreementToken);

		/// <summary>
		/// Assign the selected brands to the passed advocate applications.
		/// </summary>
		Task Assign(Guid[] brandIds, Guid[] advocateApplicationIds);

		/// <summary>
		/// Sends an email to platform email address with details of new brand wanting to get in touch
		/// </summary>
		/// <param name="model">The cleint details</param>
		/// <param name="subject">The subject of the application/inquiry</param>
		Task Apply(BrandApplicationModel model, string subject);

		/// <summary>
		/// Create a new brand
		/// </summary>
		/// <param name="model">Input data model</param>
		Task<Guid?> Create(CreateBrandModel model);

		/// <summary>
		/// Assign the Whitelist Phrases selected brands.
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="whitelistPhrases">The whitelist Phrases</param>
		Task<string[]> AddWhitelistPhrases(Guid brandId, string[] whitelistPhrases);

		/// <summary>
		/// Creates new api key for the brand
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="apiKey">Set of api keys to create</param>
		Task CreateApiKey(Guid brandId, ApiKeyModel apiKey);

		/// <summary>
		/// Generates random api keys for the brand
		/// </summary>
		/// <param name="brandId">The brand id</param>
		Task<ApiKeyModel> GenerateApiKey(Guid brandId);

		/// <summary>
		/// Deletes Phrases
		/// </summary>
		/// <param name="brandId">The brand </param>
		Task<string[]> DeleteWhitelistPhrase(Guid brandId, string[] whitelistPhrases);
		/// Create a new quiz
		/// </summary>
		/// <param name="model">Input data model</param>
		Task CreateQuiz(Guid brandId, QuizModel model);

		/// <summary>
		/// Get brand induction sections
		/// </summary>
		/// <param name="brandId">The brand id</param>
		Task<BrandInductionModel> GetBrandInductionModel(Guid brandId);

		/// <summary>
		/// Set brand induction sections
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="model">Induction model</param>
		Task PostInductionSections(Guid brandId, BrandInductionModel model);

		/// <summary>
		/// Upload publicly available asset for the brand
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="stream">The asset stream</param>
		/// <param name="assetName">Asset file name</param>
		/// <returns>URL of the asset</returns>
		Task<string> UploadAsset(Guid brandId, Stream stream, string assetName);

		/// <summary>
		/// Set new contract details on brand
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="model">New contract details</param>
		/// <returns>URL of the new contract</returns>
		Task<string> SetContract(Guid brandId, BrandContractModel model);

		/// <summary>
		/// Uploads tickets import to S3 bucket.
		/// </summary>
		/// <param name="file">The destination email address.</param>
		/// <param name="uploadPath">The upload path.</param>
		/// <param name="bucketName">The S3 bucket name.</param>
		/// <param name="contentType">The content type of file to be uploaded.</param>
		/// <param name="metaData">The tags for the uploaded file.</param>
		/// <returns>Return the key under which Amazon S3 object is to be stored.</returns>
		Task<string> UploadTicketsImportToS3Bucket(Stream file, string uploadPath, string bucketName, string contentType, Dictionary<string, object> metaData);

		/// <summary>
		/// Returns list of categories specific to brand.
		/// </summary>
		/// <param name="brandId">Brand id.</param>
		/// <param name="isEnabled">IsEnabled flag.</param>
		/// <returns>List of categories.</returns>
		Task<IEnumerable<CategoryModel>> GetCategories(Guid brandId, bool isEnabled = true);

		/// <summary>
		/// Create brand specific categories.
		/// </summary>
		/// <param name="categories">Categories to be created.</param>
		/// <param name="brandId">Brand for which categories to be created.</param>
		/// <returns></returns>
		Task CreateCategories(CreateCategoryModel categories, Guid brandId);

		/// <summary>
		/// Returns brand access on current endpoint
		/// </summary>
		/// <param name="brandId">The brandId Id</param>
		Task<bool> CheckCustomerTicketsEndpointEnabled(Guid brandId);
	}
}