namespace NowBookingWeb.Booking.Api.Controllers
{
    /// <summary>
    /// Контроллер категорий.
    /// </summary>
    [Route("booking/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
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
    }
}
