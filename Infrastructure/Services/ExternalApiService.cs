using Infrastructure.Services.Interfaces;

namespace Infrastructure.Services
{
    public class ExternalApiService : IExternalApiService
    {
        private readonly HttpClient _httpClient;

        public ExternalApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> SyncLoanApplicationRequestAsync(string payload, bool isNewCustomer, CancellationToken cancellationToken)
        {
            try
            {
                var content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");
               
                // Determine the HTTP method based on whether it's a new customer or an existing one
                var method = isNewCustomer ? HttpMethod.Post : HttpMethod.Put;
            
                using var request = new HttpRequestMessage(method, "/api/LoanApplication")
                {
                    Content = content
                };

                using var response = await _httpClient.SendAsync(request, cancellationToken);

                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
              
                return false;
            }
            catch (TaskCanceledException)
            {
                return false;
            }
        }
    }
}
