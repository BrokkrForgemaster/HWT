using System.Net.Http.Json;
using HWT.Application.Interfaces;
using HWT.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HWT.Infrastructure.Services
{
    public class UexCorpService : IUexCorpService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UexCorpService> _logger;

        private readonly JsonSerializerSettings _jsonOptions = new JsonSerializerSettings
        {
            ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        public UexCorpService(
            IHttpClientFactory httpClientFactory,
            ILogger<UexCorpService> logger
        )
        {
            if (httpClientFactory == null) throw new ArgumentNullException(nameof(httpClientFactory));
            _httpClient  = httpClientFactory.CreateClient("UEXCorp");
            _logger = logger;
        }

        public async Task<IReadOnlyList<CommodityFuture>> GetCommodityFuturesAsync(CancellationToken cancellationToken)
        {
            const string url = "2.0/commodities";
            _logger.LogInformation("Calling GET {Url}", url);

            using var resp = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("GetCommodityFuturesAsync returned {StatusCode}", resp.StatusCode);
                return Array.Empty<CommodityFuture>();
            }

            string json = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            var wrapper = JsonConvert.DeserializeObject<UexWrapper<List<CommodityFuture>>>(json, _jsonOptions);

            if (wrapper == null || wrapper.Status != "ok" || wrapper.Data == null)
            {
                _logger.LogWarning("Failed to parse commodity futures JSON or status != ok.");
                return Array.Empty<CommodityFuture>();
            }

            return wrapper.Data;
        }

        public async Task<IReadOnlyList<CommodityPrice>?> GetCommodityPricesByNameAsync(string commodityName,
            CancellationToken cancellationToken)
        {
            string encodedName = Uri.EscapeDataString(commodityName);
            string url = $"2.0/commodities_prices?commodity_name={encodedName}";
            _logger.LogInformation("Calling GET {Url}", url);

            using var resp = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("GetCommodityPricesByNameAsync returned {StatusCode}", resp.StatusCode);
                return null;
            }

            string json = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            var wrapper = JsonConvert.DeserializeObject<UexWrapper<List<CommodityPrice>>>(
                json,
                _jsonOptions
            );

            if (wrapper == null || wrapper.Status != "ok" || wrapper.Data == null)
            {
                _logger.LogWarning("Failed to parse commodity prices JSON or status != ok.");
                return null;
            }

            return wrapper.Data;
        }
    }
}