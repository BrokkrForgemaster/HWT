using System;
using System.Collections.Generic;      // List<>, IReadOnlyList<>
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json; // .Select(...), .Distinct(...), .OrderBy(...)
using System.Threading;                // CancellationTokenSource
using System.Threading.Tasks;          // Task, async/await
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;            // KeyEventArgs, Key
using HWT.Application.Interfaces;       // IUexCorpService
using HWT.Domain.Entities;
using Microsoft.Extensions.Configuration; // CommodityFuture, CommodityPrice
using Microsoft.Extensions.Logging;     // ILogger<T>

namespace HWT.Presentation.Views
{
    /// <summary>
    /// Interaction logic for LocopsView.xaml
    ///
    /// We now support “search by name” via the editable ComboBox.
    /// - On load: fetch all CommodityFuture objects to populate the ComboBox ItemsSource (just the names).
    /// - When user picks or types a name and presses Enter (or picks from drop-down), call the “prices by name” endpoint.
    /// </summary>
    public partial class LocopsView : UserControl
    {
        private readonly IUexCorpService       _uexService;
        private readonly ILogger<LocopsView>   _logger;
        private List<string>                   _commodityNames = new();
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public LocopsView(IUexCorpService uexService,
                          ILogger<LocopsView> logger, HttpClient httpClient, IConfiguration configuration)
        {
            if (uexService == null) throw new ArgumentNullException(nameof(uexService));
            if (logger == null)     throw new ArgumentNullException(nameof(logger));

            InitializeComponent();
            _configuration = configuration;
            _uexService = uexService;
            _logger      = logger;
            _httpClient = httpClient;

            Loaded += LocopsView_Loaded;
        }

        private async void LocopsView_Loaded(object sender, RoutedEventArgs e)
        {
            // Unsubscribe so it only runs once
            Loaded -= LocopsView_Loaded;

            await PopulateComboBoxAsync();
        }
        
        private bool RefineryJobsTabIsSelected()
        {
            // Find the Refinery Jobs TabItem by Header (or by Index if you know it)
            var selected =MainTabControl.SelectedItem as TabItem;
            return selected != null && (selected.Header as string) == "Refinery Jobs";
        }

        private async void RefreshRefineryJobsButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadRefineryJobsAsync();
        }

        private async Task LoadRefineryJobsAsync()
        {
            try
            {
                // Disable the Refresh button while loading
                RefreshRefineryJobsButton.IsEnabled = false;
                RefineryJobsStatusText.Text = "Loading refinery jobs…";

                // Replace this with how you actually store/retrieve your secret key:
                string secretKey = GetUexSecretKey();
                if (string.IsNullOrWhiteSpace(secretKey))
                {
                    RefineryJobsStatusText.Text = "Error: missing UEX secret key.";
                    return;
                }

                // Prepare the request
                var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    "https://api.uexcorp.space/2.0/user_refineries_jobs/");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", secretKey);

                // Send
                var sendAsync = _httpClient.SendAsync(request);
                HttpResponseMessage response = await sendAsync;
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    RefineryJobsStatusText.Text = "Invalid or forbidden secret key.";
                    RefineryJobsDataGrid.ItemsSource = null;
                    return;
                }
                else if (!response.IsSuccessStatusCode)
                {
                    RefineryJobsStatusText.Text = $"API error: {response.StatusCode}";
                    RefineryJobsDataGrid.ItemsSource = null;
                    return;
                }

                // Read JSON
                string json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                // Some endpoints may return an object with a “status” field instead of an array:
                List<RefineryJob> jobs;
                if (json.TrimStart().StartsWith("["))
                {
                    jobs = JsonSerializer.Deserialize<List<RefineryJob>>(json, options) 
                           ?? new List<RefineryJob>();
                }
                else
                {
                    // parse envelope to check for “no_refinery_jobs_found”
                    using var doc = JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("status", out var statusProp)
                        && statusProp.GetString() == "no_refinery_jobs_found")
                    {
                        jobs = new List<RefineryJob>();
                    }
                    else
                    {
                        // Fallback: try to deserialize as array anyway
                        jobs = JsonSerializer.Deserialize<List<RefineryJob>>(json, options) 
                               ?? new List<RefineryJob>();
                    }
                }

                if (jobs.Count == 0)
                {
                    RefineryJobsStatusText.Text = "No refinery jobs found.";
                    RefineryJobsDataGrid.ItemsSource = null;
                }
                else
                {
                    RefineryJobsStatusText.Text = $"{jobs.Count} job(s) loaded.";
                    RefineryJobsDataGrid.ItemsSource = jobs;
                }
            }
            catch (Exception ex)
            {
                RefineryJobsStatusText.Text = $"Error fetching jobs: {ex.Message}";
                RefineryJobsDataGrid.ItemsSource = null;
            }
            finally
            {
                RefreshRefineryJobsButton.IsEnabled = true;
            }
        }

        private string GetUexSecretKey()
        {
           return _configuration["AppSettings:UexSecretKey"] 
                  ?? throw new InvalidOperationException("UEX secret key is not configured.");
        }
        /// <summary>
        /// Fetch the list of all futures (just to fill the ComboBox with commodity names).
        /// </summary>
        private async Task PopulateComboBoxAsync()
        {
            const int timeoutMs = 15_000;
            using var cts = new CancellationTokenSource(timeoutMs);

            IReadOnlyList<CommodityFuture> futures = Array.Empty<CommodityFuture>();
            try
            {
                _logger.LogInformation("Fetching commodity futures to populate ComboBox...");
                futures = await _uexService
                    .GetCommodityFuturesAsync(cts.Token)
                    .ConfigureAwait(false);

                _logger.LogInformation("Fetched {Count} futures.", futures.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch commodity futures for ComboBox.");
            }
            
            _commodityNames = futures
                .Select(f => f.Name?.Trim() ?? string.Empty)
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Populate ComboBox on UI thread
            await Dispatcher.InvokeAsync(() =>
            {
                CommodityCombo.ItemsSource = _commodityNames;
            });
        }

        /// <summary>
        /// Called when the user selects a different item in the ComboBox.
        /// We retrieve prices by the exact commodity name.
        /// </summary>
        private async void CommodityCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CommodityCombo.SelectedItem is not string selectedName)
                return;

            await FetchPricesByNameAsync(selectedName);
        }

        /// <summary>
        /// If the user types and presses Enter, also trigger the search (even if they typed a brand-new name).
        /// </summary>
        private async void CommodityCombo_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string typedName = CommodityCombo.Text.Trim();
                if (!string.IsNullOrEmpty(typedName))
                {
                    await FetchPricesByNameAsync(typedName);
                }
            }
        }

        /// <summary>
        /// Calls the service to fetch CommodityPrice list by commodity name, then updates the DataGrid.
        /// </summary>
        private async Task FetchPricesByNameAsync(string commodityName)
        {
            const int timeoutMs = 15_000;
            using var cts = new CancellationTokenSource(timeoutMs);

            List<CommodityPrice> priceResults = new();

            try
            {
                _logger.LogInformation("Fetching prices for commodity \"{Name}\"...", commodityName);
                var result = await _uexService
                    .GetCommodityPricesByNameAsync(commodityName, cts.Token)
                    .ConfigureAwait(false);

                if (result == null)
                {
                    _logger.LogWarning("No price data returned for \"{Name}\".", commodityName);
                }
                else
                {
                    priceResults = result.ToList();
                    _logger.LogInformation("Fetched {Count} price records for \"{Name}\".",
                                           priceResults.Count, commodityName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching prices for \"{Name}\".", commodityName);
            }

            // Update DataGrid on UI thread
            await Dispatcher.InvokeAsync(() =>
            {
                CommoditiesGrid.ItemsSource = priceResults;
            });
        }
        
        private void CommodityCombo_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (CommodityCombo.Text == "Type or select commodity name...")
                CommodityCombo.Text = "";
        }

        private void CommodityCombo_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(CommodityCombo.Text))
                CommodityCombo.Text = "Type or select commodity name...";
        }

    }
}
