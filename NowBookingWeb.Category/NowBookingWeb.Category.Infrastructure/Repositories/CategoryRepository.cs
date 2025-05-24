using Dapper;
using NowBookingWeb.Category.Application.Dto.Output.Category;
using NowBookingWeb.Category.Application.Interfaces.Repositories;
using NowBookingWeb.Category.Infrastructure.Data;
using System.Data;

namespace NowBookingWeb.Category.Infrastructure.Repositories
{
    /// <summary>
    /// Класс реализует методы репозитория категорий. 
    /// </summary>
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DapperDbContext _db;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="db">Контекст БД.</param>
        public CategoryRepository(DapperDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CategoryOutput>> GetCategoriesAsync()
        {
            using IDbConnection connection = await _db.GetConnectionAsync();

            string query = "SELECT \"Id\", " +
                           "\"Name\", " +
                           "\"Description\" " +
                           "FROM \"Categories\" " +
                           "ORDER BY \"Id\" ASC";

            IEnumerable<CategoryOutput> result = await connection.QueryAsync<CategoryOutput>(query);

            return result.AsList();
        }
    }
}