using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tigerspike.Solv.Application.Models;

namespace Tigerspike.Solv.Application.Services
{
	public interface IPaymentService
	{

		/// <summary>
		/// Payment account id of the Platform
		/// </summary>
		string PaymentReceiverAccountId { get; }

		/// <summary>
		/// Returns the PayPal urls for the FE to redirect to, so solvers can authorize Solv
		/// to use their PayPal account.
		/// </summary>
		/// <param name="advocateId">The advocate id to be used as a tracking id</param>
		Task<string> GetPartnerReferralUrl(Guid advocateId);

		/// <summary>
		/// Validate whether the solver has given the right permissions to Solv
		/// (This one is used when we don't know his/her PayPal account yet and we need to check blindly)
		/// We might eventually replace the previous one with this one.
		/// </summary>
		/// <param name="advocateId">The advocate id (which was used as a tracking id in the referral url request)</param>
		Task<(string merchantId, bool paymentReceivable, bool emailVerified)> GetPayPalSolverStatus(Guid advocateId);

		/// <summary>
		/// Returns the PayPal url for the FE to redirect to, so clients can authorize Solv
		/// to use their PayPal account for charging payments.
		/// It also returns the Billing Agreement token to be saved.
		/// </summary>
		/// <param name="brandId">The brand id</param>
		Task<(string url, string baToken)> GenerateBillingAgreement(Guid brandId);

		/// <summary>
		/// Finalize the billing agreement for a Brand after getting the token from PayPal
		/// </summary>
		/// <param name="brandId">The brand id</param>
		/// <param name="billingAgreementToken">The token that was given by PayPal</param>
		Task<(string payerId, string billingAgreementId)> FinalizeBillingAgreement(Guid brandId, string billingAgreementToken);

		/// <summary>
		/// Sets the risk transaction context for API call
		/// </summary>
		/// <param name="context">The risk transaction context</param>
		/// <returns>Tracking id for the context</returns>
		Task<string> SetRiskTransactionContext(RiskTransactionContext context);

		/// <summary>
		/// Execute the payment from a client to a solver.
		/// We will refer to the PayPal order as a payment.
		/// The method involves 3 calls to PayPal endpoint to create/validate/capture the payment, which we only care whether it happened or not as a whole.
		/// </summary>
		/// <param name="requestId">An id to be used in the header to make the request idempotent (PayPal won't repeate the payment if we used the same id again)</param>
		/// <param name="trackingId">The trackingId for risk transaction context</param>
		/// <param name="payerId">The PayPal payer Id (merchant id) which is in our case is the client (brand)</param>
		/// <param name="billingAgreementId">The billing agreement id that refer to the permission to make payment on behalf of the client (captured when connecting PayPal)</param>
		/// <param name="amount">The amount that should be transfered</param>
		/// <param name="currencyCode">The currency to be used in the payment</param>
		/// <param name="payeeId">The PayPal payee id (merchant Id) which is in our case is the Solver account / Platform account</param>
		/// <param name="invoiceId">An invoice number to be referred by the Solver and the Client, it will show up in the PayPal dashboard</param>
		/// <param name="description">The description of the payment, also shows up in PayPal dashboard</param>
		/// <param name="items">The list of items to be enumerated in the order</param>
		Task<string> ExecutePayment(string requestId, string trackingId, string payerId, string billingAgreementId, decimal amount, string currencyCode, string payeeId, string invoiceId, string description, IList<(string name, decimal ticketPrice)> items);
	}
}