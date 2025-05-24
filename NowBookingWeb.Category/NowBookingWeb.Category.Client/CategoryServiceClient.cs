using Microsoft.Extensions.Configuration;

namespace NowBookingWeb.Category.Client
{
    /// <summary>
    /// Класс реализует методы клиента микросервиса категорий.   
    /// </summary>
    public class CategoryServiceClient : ICategoryServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _categoryServiceUri;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="clientFactory">Создание обьекта httpClient.</param>
        /// <param name="configuration">Конфигурация.</param>
        public CategoryServiceClient(IHttpClientFactory clientFactory,
            IConfiguration configuration)
        {
            _httpClient = clientFactory.CreateClient();
            _categoryServiceUri = new Uri(configuration.GetSection("Services")["Category"]!);
        }

     
        /// <inheritdoc />
        public async Task<CategoryOutput> RemoveCategoryAsync(int categoryId, CancellationToken cancellationToken)
        {
            using HttpRequestMessage httpRequest = new HttpRequestMessage(
              HttpMethod.Delete,
              _categoryServiceUri + $"booking/category/category/{categoryId}");

            HttpResponseMessage response = await _httpClient.SendAsync(httpRequest, cancellationToken);

            string apiResponse = await response.Content.ReadAsStringAsync();

            CategoryOutput result = JsonConvert.DeserializeObject<CategoryOutput>(apiResponse)!;

            return result;
        }
    }
}