﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Shift4.Response;
using Shift4.Request;
using Shift4.Exception;
using System.Security.Cryptography;
using Shift4.Internal;
using Shift4.Request.Checkout;
using System.IO;
using Shift4.Converters;

namespace Shift4
{
    /// <summary>
    /// Shift4 API Service
    /// </summary>
    public class Shift4Gateway
    {
        private const string CHARGES_PATH = "charges";
        private const string CREDITS_PATH = "credits";
        private const string TOKENS_PATH = "tokens";
        private const string CUSTOMERS_PATH = "customers";
        private const string CARDS_PATH = "customers/{0}/cards";
        private const string PAYMENT_METHODS_PATH = "payment-methods";
        private const string PLANS_PATH = "plans";
        private const string SUBSCRIPTIONS_PATH = "subscriptions";
        private const string EVENTS_PATH = "events";
        private const string BLACKLIST_RULE_PATH = "blacklist";
        private const string FILES_PATH = "files";
        private const string DISPUTES_PATH= "disputes";
        private const string FRAUD_WARNING_PATH = "/fraud-warnings";
        private const string REFUNDS_PATH = "refunds";
        private const string PAYOUTS_PATH = "payouts";
        private const string PAYOUT_TRANSACTIONS_PATH = "payout-transactions";
        private IApiClient _apiClient;
        private ISignService _signService;
        private IConfigurationProvider _configurationProvider;

        public Shift4Gateway(string secretKey, string serverUrl = "https://api.shift4.com/", string uploadsUrl = "https://uploads.api.shift4.com")
        {
            var configurationProvider = new ConfigurationProvider(secretKey, serverUrl, uploadsUrl);
            _configurationProvider = configurationProvider;
            _apiClient = new ApiClient(new Shift4.Internal.HttpClient(), configurationProvider, new FileExtensionToMimeMapper());
            _signService = new SignService(configurationProvider);
        }

        public Shift4Gateway(IApiClient apiClient, IConfigurationProvider configurationProvider, ISignService signService)
        {
            _apiClient = apiClient;
            _signService = signService; 
            _configurationProvider = configurationProvider;
        }

        #region public

        #region Charge

        public async Task<Charge> CreateCharge(ChargeRequest chargeRequest, RequestOptions requestOptions = null)
        {
            return await SendRequest<Charge>(HttpMethod.Post, CHARGES_PATH, chargeRequest, requestOptions);
        }

        public async Task<Charge> CaptureCharge(CaptureRequest capture, RequestOptions requestOptions = null)
        {
            var url = string.Format("{0}/{1}/capture", CHARGES_PATH, capture.ChargeId);
            return await SendRequest<Charge>(HttpMethod.Post, url, capture, requestOptions);
        }

        public async Task<Refund> CreateRefund(RefundRequest refund, RequestOptions requestOptions = null)
        {
            return await SendRequest<Refund>(HttpMethod.Post, REFUNDS_PATH, refund, requestOptions);
        }

        public async Task<Refund> UpdateRefund(RefundUpdateRequest request, RequestOptions requestOptions = null)
        {
            var url = string.Format("{0}/{1}", REFUNDS_PATH, request.RefundId);
            return await SendRequest<Refund>(HttpMethod.Post, url, request, requestOptions);
        }

        public async Task<Refund> RetrieveRefund(string RefundId)
        {
            var url = string.Format("{0}/{1}", REFUNDS_PATH, RefundId);
            return await SendRequest<Refund>(HttpMethod.Get, url);
        }

        public async Task<Charge> RetrieveCharge(string id)
        {
            var url = string.Format("{0}/{1}", CHARGES_PATH, id);
            return await SendRequest<Charge>(HttpMethod.Get, url);
        }

        public async Task<Charge> UpdateCharge(ChargeUpdateRequest chargeUpdate, RequestOptions requestOptions = null)
        {
            var url = string.Format("{0}/{1}", CHARGES_PATH, chargeUpdate.ChargeId);
            return await SendRequest<Charge>(HttpMethod.Post, url, chargeUpdate, requestOptions);
        }

        public async Task<ListResponse<Charge>> ListCharges()
        {
            return await SendListRequest<Charge>(HttpMethod.Get, CHARGES_PATH);
        }

        public async Task<ListResponse<Charge>> ListCharges(ChargeListRequest request)
        {
            return await SendListRequest<Charge>(HttpMethod.Get, CHARGES_PATH, request);
        }

        public async Task<Token> CreateToken(TokenRequest createTokenRequest)
        {
            return await SendRequest<Token>(HttpMethod.Post, TOKENS_PATH, createTokenRequest);
        }

        public async Task<Token> RetrieveToken(string id)
        {
            var url = string.Format("{0}/{1}", TOKENS_PATH, id);
            return await SendRequest<Token>(HttpMethod.Get, url);
        }

        #endregion

        #region Customers

        public async Task<Customer> CreateCustomer(CustomerRequest createCustomerRequest, RequestOptions requestOptions = null)
        {
            return await SendRequest<Customer>(HttpMethod.Post, CUSTOMERS_PATH, createCustomerRequest, requestOptions);
        }

        public async Task<Customer> UpdateCustomer(CustomerUpdateRequest updateCustomerRequest, RequestOptions requestOptions = null)
        {
            var url = string.Format("{0}/{1}", CUSTOMERS_PATH, updateCustomerRequest.CustomerId);
            return await SendRequest<Customer>(HttpMethod.Post, url, updateCustomerRequest, requestOptions);
        }

        public async Task<Customer> RetrieveCustomer(string id)
        {

            var url = string.Format("{0}/{1}", CUSTOMERS_PATH, id);
            return await SendRequest<Customer>(HttpMethod.Get, url);
        }

        public async Task<DeleteResponse> DeleteCustomer(string id)
        {
            var url = string.Format("{0}/{1}", CUSTOMERS_PATH, id);
            return await SendRequest<DeleteResponse>(HttpMethod.Delete, url);
        }

        public async Task<ListResponse<Customer>> ListCustomers()
        {
            return await SendListRequest<Customer>(HttpMethod.Get, CUSTOMERS_PATH);
        }

        public async Task<ListResponse<Customer>> ListCustomers(CustomerListRequest request)
        {
            return await SendListRequest<Customer>(HttpMethod.Get, CUSTOMERS_PATH, request);
        }

        #endregion

        #region Cards

        public async Task<Card> CreateCard(CardRequest createCardRequest, RequestOptions requestOptions = null)
        {
            var url = string.Format(CARDS_PATH, createCardRequest.CustomerId);
            return await SendRequest<Card>(HttpMethod.Post, url, createCardRequest, requestOptions);
        }

        public async Task<Card> RetrieveCard(string customerId, string id)
        {
            var url = string.Format(CARDS_PATH, customerId) + "/" + id;
            return await SendRequest<Card>(HttpMethod.Get, url);
        }

        public async Task<Card> UpdateCard(CardUpdateRequest updateCard, RequestOptions requestOptions = null)
        {
            var url = string.Format(CARDS_PATH, updateCard.CustomerId) + "/" + updateCard.CardId;
            return await SendRequest<Card>(HttpMethod.Post, url, updateCard, requestOptions);
        }

        public async Task<DeleteResponse> DeleteCard(string customerId, string id)
        {
            var url = string.Format(CARDS_PATH, customerId) + "/" + id;
            return await SendRequest<DeleteResponse>(HttpMethod.Delete, url);
        }

        public async Task<ListResponse<Card>> ListCards(string customerId)
        {
            var url = string.Format(CARDS_PATH, customerId);
            return await SendListRequest<Card>(HttpMethod.Get, url);
        }

        public async Task<ListResponse<Card>> ListCards(CardListRequest request)
        {
            var url = string.Format(CARDS_PATH, request.CustomerId);
            return await SendListRequest<Card>(HttpMethod.Get, url, request);
        }

        #endregion

        #region PaymentMethods

        public async Task<PaymentMethod> CreatePaymentMethod(PaymentMethodRequest createPaymentMethodRequest, RequestOptions requestOptions = null)
        {
            return await SendRequest<PaymentMethod>(HttpMethod.Post, PAYMENT_METHODS_PATH, createPaymentMethodRequest, requestOptions);
        }

        public async Task<PaymentMethod> RetrievePaymentMethod(string id)
        {
            return await SendRequest<PaymentMethod>(HttpMethod.Get, PAYMENT_METHODS_PATH + "/" + id);
        }

        public async Task<DeleteResponse> DeletePaymentMethod(string id)
        {
            return await SendRequest<DeleteResponse>(HttpMethod.Delete, PAYMENT_METHODS_PATH + "/" + id);
        }

        public async Task<ListResponse<PaymentMethod>> ListPaymentMethods(PaymentMethodListRequest request)
        {
            return await SendListRequest<PaymentMethod>(HttpMethod.Get, PAYMENT_METHODS_PATH, request);
        }
        
        #endregion

        #region plans

        public async Task<Plan> CreatePlan(PlanRequest createPlanRequest, RequestOptions requestOptions = null)
        {
            return await SendRequest<Plan>(HttpMethod.Post, PLANS_PATH, createPlanRequest, requestOptions);
        }

        public async Task<Plan> RetrievePlan(string id)
        {
            var url = string.Format("{0}/{1}", PLANS_PATH, id);
            return await SendRequest<Plan>(HttpMethod.Get, url);
        }

        public async Task<Plan> UpdatePlan(PlanUpdateRequest updatePlanRequest, RequestOptions requestOptions = null)
        {
            var url = string.Format("{0}/{1}", PLANS_PATH, updatePlanRequest.PlanId);
            return await SendRequest<Plan>(HttpMethod.Post, url, updatePlanRequest, requestOptions);
        }

        public async Task<ListResponse<Plan>> ListPlans()
        {
            return await SendListRequest<Plan>(HttpMethod.Get, PLANS_PATH);
        }

        public async Task<DeleteResponse> DeletePlan(string id)
        {
            var url = string.Format("{0}/{1}", PLANS_PATH, id);
            return await SendRequest<DeleteResponse>(HttpMethod.Delete, url);
        }

        public async Task<ListResponse<Plan>> ListPlans(PlanListRequest request)
        {
            return await SendListRequest<Plan>(HttpMethod.Get, PLANS_PATH, request);
        }

        #endregion

        #region subscriptions

        public async Task<Subscription> CreateSubscription(SubscriptionRequest createSubscriptionRequest, RequestOptions requestOptions = null)
        {
            return await SendRequest<Subscription>(HttpMethod.Post, SUBSCRIPTIONS_PATH, createSubscriptionRequest, requestOptions);
        }

        public async Task<Subscription> RetrieveSubscription(string id)
        {
            var url = SUBSCRIPTIONS_PATH + "/" + id;
            return await SendRequest<Subscription>(HttpMethod.Get, url);
        }

        public async Task<Subscription> UpdateSubscription(SubscriptionUpdateRequest updateSubscriptionRequest, RequestOptions requestOptions = null)
        {
            var url = SUBSCRIPTIONS_PATH + "/" + updateSubscriptionRequest.SubscriptionId;
            return await SendRequest<Subscription>(HttpMethod.Post, url, updateSubscriptionRequest, requestOptions);
        }

        public async Task<ListResponse<Subscription>> ListSubscriptions()
        {
            return await SendListRequest<Subscription>(HttpMethod.Get, SUBSCRIPTIONS_PATH);

        }

        public async Task<Subscription> CancelSubscription(SubscriptionCancelRequest cancelSubscriptionsRequest)
        {
            var url = SUBSCRIPTIONS_PATH + "/" + cancelSubscriptionsRequest.SubscriptionId;
            return await SendRequest<Subscription>(HttpMethod.Delete, url, cancelSubscriptionsRequest);
        }

        public async Task<ListResponse<Subscription>> ListSubscriptions(SubscriptionListRequest request)
        {
            return await SendListRequest<Subscription>(HttpMethod.Get, SUBSCRIPTIONS_PATH, request);
        }

        #endregion

        #region events

        public async Task<Event> RetrieveEvent(string id)
        {
            var url = string.Format("{0}/{1}", EVENTS_PATH, id);
            return await SendRequest<Event>(HttpMethod.Get, url);
        }

        public async Task<ListResponse<Event>> ListEvents()
        {
            return await SendListRequest<Event>(HttpMethod.Get, EVENTS_PATH);
        }

        public async Task<ListResponse<Event>> ListEvents(ListRequest request)
        {
            return await SendListRequest<Event>(HttpMethod.Get, EVENTS_PATH, request);
        }
        #endregion

        #region blacklists

        public async Task<BlacklistRule> CreateBlacklistRule(BlacklistRuleRequest request, RequestOptions requestOptions = null)
        {
            return await SendRequest<BlacklistRule>(HttpMethod.Post, BLACKLIST_RULE_PATH, request, requestOptions);
        }

        public async Task<BlacklistRule> RetrieveBlacklistRule(string id)
        {
            var url = BLACKLIST_RULE_PATH + "/" + id;
            return await SendRequest<BlacklistRule>(HttpMethod.Get, url);
        }

        public async Task<DeleteResponse> DeleteBlacklistRule(string id)
        {
            var url = BLACKLIST_RULE_PATH + "/" + id;
            return await SendRequest<DeleteResponse>(HttpMethod.Delete, url);
        }

        public async Task<ListResponse<BlacklistRule>> ListBlacklistRules()
        {
            return await SendListRequest<BlacklistRule>(HttpMethod.Get, BLACKLIST_RULE_PATH);
        }

        public async Task<ListResponse<BlacklistRule>> ListBlacklistRules(BlacklistRuleListRequest request)
        {
            return await SendListRequest<BlacklistRule>(HttpMethod.Get, BLACKLIST_RULE_PATH, request);
        }

        #endregion

        #region checkout

        public string SignCheckoutRequest(CheckoutRequest checkoutRequest)
        {
            string data = JsonConvert.SerializeObject(checkoutRequest, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore });

            var signature = _signService.Sign(data);

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(signature + "|" + data));
        }

        #endregion

        #region credits

        public async Task<ListResponse<Credit>> ListCredits()
        {
            return await SendListRequest<Credit>(HttpMethod.Get, CREDITS_PATH);
        }

        public async Task<Credit> CreateCredit(CreditRequest request, RequestOptions requestOptions = null)
        {
            return await SendRequest<Credit>(HttpMethod.Post, CREDITS_PATH, request, requestOptions);
        }

        public async Task<Credit> RetrieveCredit(string creditId)
        {
            var url = CREDITS_PATH + "/" + creditId;
            return await SendRequest<Credit>(HttpMethod.Get, url);
        }

        public async Task<Credit> UpdateCredit(CreditUpdateRequest updateRequest, RequestOptions requestOptions = null)
        {
            var url = CREDITS_PATH + "/" + updateRequest.CreditId;
            return await SendRequest<Credit>(HttpMethod.Post, url, updateRequest, requestOptions);
        }
        #endregion

        #region disputes

        public async Task<Dispute> RetrieveDispute(string id)
        {
            var url = string.Format("{0}/{1}", DISPUTES_PATH, id);
            return await SendRequest<Dispute>(HttpMethod.Get, url);
        }

        public async Task<ListResponse<Dispute>> ListDisputes(ListRequest request)
        {
            return await SendListRequest<Dispute>(HttpMethod.Get, DISPUTES_PATH, request);
        }

        public async Task<ListResponse<Dispute>> ListDisputes()
        {
            return await SendListRequest<Dispute>(HttpMethod.Get, DISPUTES_PATH);
        }

        public async Task<Dispute> UpdateDispute(DisputeUpdateRequest request, RequestOptions requestOptions = null)
        {
            var url = string.Format("{0}/{1}", DISPUTES_PATH, request.DisputeId);
            return await SendRequest<Dispute>(HttpMethod.Post, url, request, requestOptions);
        }

        public async Task<Dispute> CloseDispute(string id, RequestOptions requestOptions = null)
        {
            var url = string.Format("{0}/{1}/close", DISPUTES_PATH,id);
            return await SendRequest<Dispute>(HttpMethod.Post, url, requestOptions);
        }

        #endregion

        #region fraud-warnings

        public async Task<FraudWarning> RetrieveFraudWarning(string id)
        {
            var url = FRAUD_WARNING_PATH + "/" + id;
            return await SendRequest<FraudWarning>(HttpMethod.Get, url);
        }
        
        public async Task<ListResponse<FraudWarning>> ListFraudWarnings()
        {
            return await SendListRequest<FraudWarning>(HttpMethod.Get, FRAUD_WARNING_PATH);
        }

        public async Task<ListResponse<FraudWarning>> ListFraudWarnings(FraudWarningListRequest request)
        {
            return await SendListRequest<FraudWarning>(HttpMethod.Get, FRAUD_WARNING_PATH, request);
        }



        #endregion

        #region payouts

        public async Task<ListResponse<Payout>> ListPayouts()
        {
            return await SendListRequest<Payout>(HttpMethod.Get, PAYOUTS_PATH);
        }

        public async Task<ListResponse<Payout>> ListPayouts(PayoutListRequest request)
        {
            return await SendListRequest<Payout>(HttpMethod.Get, PAYOUTS_PATH, request);
        }

        public async Task<Payout> CreatePayout(RequestOptions requestOptions = null)
        {
            return await SendRequest<Payout>(HttpMethod.Post, PAYOUTS_PATH, requestOptions);
        }

        public async Task<Payout> RetrievePayout(string id)
        {
            var url = PAYOUTS_PATH + "/" + id;
            return await SendRequest<Payout>(HttpMethod.Get, url);
        }

        #endregion

        #region payout-transactions

        public async Task<ListResponse<PayoutTransaction>> ListPayoutTransactions(PayoutTransactionListRequest request)
        {
            var url = PAYOUT_TRANSACTIONS_PATH;
            return await SendListRequest<PayoutTransaction>(HttpMethod.Get, url, request);
        }

        #endregion

        #region uploads

        public async Task<FileUpload> CreateFileUpload(FileUploadRequest request)
        {
            return await SendUploadRequest<FileUpload>(HttpMethod.Post, FILES_PATH, request);
        }

        public async Task<FileUpload> RetrieveFileUpload(string id)
        {
            var url = FILES_PATH + "/" + id;
            return await SendRequest<FileUpload>(HttpMethod.Get, url, null,_configurationProvider.GetUploadsUrl());
        }

        public async Task<ListResponse<FileUpload>> ListFileUpload()
        {
            return await SendListRequest<FileUpload>(HttpMethod.Get, FILES_PATH, _configurationProvider.GetUploadsUrl());
        }

        #endregion

        #endregion

        #region private

        private async Task<ListResponse<TList>> SendListRequest<TList>(HttpMethod httpMethod, string path)
        {
            return await SendListRequest<TList>(httpMethod, path, _configurationProvider.GetApiUrl());
        }

        private async Task<ListResponse<TList>> SendListRequest<TList>(HttpMethod httpMethod, string path,string baseUrl)
        {
            Shift4List shift4List = await SendRequest<Shift4List>(httpMethod, path, null, baseUrl);
            return DeserializeList<TList>(shift4List);
        }

        private ListResponse<TList> DeserializeList<TList>(Shift4List shift4List)
        {
            ListResponse<TList> response = new ListResponse<TList>();
            response.TotalCount = shift4List.TotalCount;
            if (shift4List.List != null)
            {
                response.List = shift4List.List.Select(jsonItem => JsonConvert.DeserializeObject<TList>(jsonItem.ToString())).ToList();
            }
            else
            {
                response.List = new List<TList>();
            }
            return response;
        }

        private async Task<ListResponse<TList>> SendListRequest<TList>(HttpMethod httpMethod, string path, object parameters)
        {
            path = path + "?" + GenerateGetPath(parameters);
            path = path.TrimEnd('?'); //if there is no path generated ( params is empty ) remove "?"
            Shift4List shift4List = await SendRequest<Shift4List>(httpMethod, path);
            return DeserializeList<TList>(shift4List);
        }

        private string GenerateGetPath(object parameters, string parentName = null)
        {

            StringBuilder path = new StringBuilder();
            var type = parameters.GetType();
            foreach (var property in type.GetProperties())
            {
                var value = property.GetValue(parameters, null);

                if (value != null && !IsIgnored(property))
                {
                    if (property.PropertyType.IsClass() && !(value is string))
                    {
                        path.Append(GenerateGetPath(value, GetPropertyName(property)));
                    }
                    else
                    {
                        path.Append(GenerateGetSection(value, property, parentName));

                    }
                }
            }
            var finalPath = path.ToString();
            if (parentName == null)
            {
                finalPath = finalPath.TrimEnd('&');
            }
            return finalPath;
        }

        private bool IsIgnored(PropertyInfo property)
        {
            return property.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).FirstOrDefault() != null;
        }

        private string GenerateGetSection(object value, PropertyInfo property, string parentName)
        {
            var propertyName = GetPropertyName(property);
            if (parentName != null)
            {
                var converterValue = ConvertValue(value);
                return parentName + "[" + propertyName + "]=" + converterValue + "&";
            }
            else
            {
                return propertyName + "=" + Uri.EscapeDataString(value.ToString()) + "&";
            }

        }

        private object ConvertValue(object value)
        {
            string stringValue=null;

            if (value is DateTime)
            {
                var converter = new UnixDateConverter();
                var localDate = (DateTime)value;
                stringValue = converter.ToUnixTimeStamp(localDate.ToUniversalTime()).ToString();
            }
            else
            {
                stringValue = value.ToString();
            }
            return Uri.EscapeDataString(stringValue);
        }

        private string GetPropertyName(PropertyInfo property)
        {
            var attribute = property.GetCustomAttributes(typeof(JsonPropertyAttribute), true).FirstOrDefault();
            if (attribute != null)
            {
                return ((JsonPropertyAttribute)attribute).PropertyName;
            }
            else
            {
                return property.Name;
            }
        }

        private async Task<T> SendRequest<T>(HttpMethod method, string action, RequestOptions requestOptions = null)
        {
            return await SendRequest<T>(method, action, null, requestOptions);
        }

        private async Task<T> SendRequest<T>(HttpMethod method, string action, object parameter, RequestOptions requestOptions = null)
        {
            return await SendRequest<T>(method, action, parameter, _configurationProvider.GetApiUrl(), requestOptions);
        }

        private async Task<T> SendRequest<T>(HttpMethod method, string action, object parameter, string baseUrl, RequestOptions requestOptions = null)
        {
            var url = new Uri(new Uri(baseUrl), action);
            return await _apiClient.SendRequest<T>(method, url.ToString(), parameter, requestOptions);
        }

        private async Task<T> SendUploadRequest<T>(HttpMethod method, string action, FileUploadRequest request)
        {
            var url = new Uri(new Uri(_configurationProvider.GetUploadsUrl()), action);
            var form = new Dictionary<string, string>() { { "purpose", JsonConvert.SerializeObject(request.Purpose,new SafeEnumConverter()).Trim('"') } };

            return await _apiClient.SendMultiPartRequest<T>(method, url.ToString(), form, request.File, request.FileName);
        }

        #endregion

    }
}
