using Microsoft.Extensions.Logging;
using NowBookingWeb.Category.Application.Dto.Output.Category;
using NowBookingWeb.Category.Application.Interfaces.Repositories;
using NowBookingWeb.Category.Application.Interfaces.Services;
using NowBookingWeb.Grpc.Contracts.Protos;

namespace NowBookingWeb.Category.Application.Services
{
    /// <summary>
    /// Класс реализует методы сервиса категорий.
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryService> _logger;
        private readonly BookingProtoService.BookingProtoServiceClient _bookingServiceGrpcClient;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="categoryRepository">Репозиторий категорий.</param>
        /// <param name="logger">Логгер.</param>
        /// <param name="bookingServiceGrpcClient">Клиент сервиса бронирования grpc.</param>
        public CategoryService(ICategoryRepository categoryRepository,
            ILogger<CategoryService> logger,
            BookingProtoService.BookingProtoServiceClient bookingServiceGrpcClient)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
            _bookingServiceGrpcClient = bookingServiceGrpcClient;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CategoryOutput>> GetCategoriesAsync(CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<CategoryOutput> result = await _categoryRepository.GetCategoriesAsync();

                return result;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
