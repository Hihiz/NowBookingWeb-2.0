using Microsoft.AspNetCore.Mvc;

namespace NowBookingWeb.Category.Api.Controllers
{
    /// <summary>
    /// Контроллер категорий.
    /// </summary>
    [Route("booking/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="categoryService">Сервис категорий.</param>
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
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
            IEnumerable<CategoryOutput> result = await _categoryService.GetCategoriesAsync(cancellationToken);

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
            CategoryOutput result = await _categoryService.RemoveCategoryAsync(categoryId, cancellationToken);

            return Ok(result);
        }
    }
}
