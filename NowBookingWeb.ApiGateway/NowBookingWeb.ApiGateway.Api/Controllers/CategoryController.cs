using NowBookingWeb.Category.Client;

namespace NowBookingWeb.ApiGateway.Api.Controllers
{
    /// <summary>
    /// Контроллер категорий.
    /// </summary>
    [Route("booking/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryServiceClient _categoryClient;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="categoryClient">Сервис категорий.</param>
        public CategoryController(ICategoryServiceClient categoryClient)
        {
            _categoryClient = categoryClient;
        }

        /// <summary>
        /// Метод получает список категорий.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Список категорий.</returns>
        [HttpGet]
        [Route("categories")]
        public async Task<IActionResult> GetCategoriesAsync(CancellationToken cancellationToken)
        {
            IEnumerable<CategoryOutput> result = await _categoryClient.GetCategoriesAsync(cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Метод удаляет категорию.
        /// </summary>
        /// <param name="categoryId">Id категории.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Удаленная категория.</returns>
        [HttpDelete]
        [Route("category/{categoryId}")]
        public async Task<IActionResult> RemoveCategoryAsync([FromRoute] int categoryId,
            CancellationToken cancellationToken)
        {
            CategoryOutput result = await _categoryClient.RemoveCategoryAsync(categoryId, cancellationToken);

            return Ok(result);
        }
    }
}
