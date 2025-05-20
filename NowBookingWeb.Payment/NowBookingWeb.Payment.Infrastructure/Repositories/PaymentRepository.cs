using Dapper;
using NowBookingWeb.Payment.Application.Dto.Output.Payment;
using NowBookingWeb.Payment.Application.Interfaces.Repositories;
using NowBookingWeb.Payment.Infrastructure.Data;
using System.Data;

namespace NowBookingWeb.Payment.Infrastructure.Repositories
{
    /// <summary>
    /// Класс реализует методы репозитория платажей.
    /// </summary>
    public class PaymentRepository : IPaymentRepository
    {
        private readonly DapperDbContext _db;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="db">Класс контекста.</param>
        public PaymentRepository(DapperDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task<PaymentOutput> RestorePaymentAsync(string? transactionId, string? paymentMethodName,
            string? paymentCurrentName, int bookingId)
        {
            using IDbConnection connection = await _db.GetConnectionAsync();

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@transactionId", transactionId);
            parameters.Add("@paymentMethodName", paymentMethodName);
            parameters.Add("@paymentCurrentName", paymentCurrentName);
            parameters.Add("@bookingId", bookingId);

            string query = "INSERT INTO \"Payments\" (\"TransactionId\", " +
                           "\"PaymentMethodName\", " +
                           "\"PaymentCurrentName\" , " +
                           "\"BookingId\") " +
                           "VALUES (@transactionId, " +
                           "@paymentMethodName, " +
                           "@paymentCurrentName, " +
                           "@bookingId) " +
                              "RETURNING \"Id\", " +
                              "\"TransactionId\", " +
                              "\"PaymentMethodName\" AS PaymentMethod, " +
                              "\"PaymentCurrentName\" AS PaymentCurrent, " +
                              "\"BookingId\"";

            PaymentOutput? result = await connection.QuerySingleAsync<PaymentOutput>(query, parameters);

            if (result is null)
            {
                throw new InvalidOperationException("Ошибка при восстановлении платежа. " +
                                                    $"TransactionId: {transactionId}. " +
                                                    $"PaymentMethodName: {paymentMethodName}. " +
                                                    $"PaymentCurrentName: {paymentCurrentName}. " +
                                                    $"BookingId: {bookingId}.");
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<bool> CheckPaymentByBookingIdAsync(int bookingId)
        {
            using IDbConnection connection = await _db.GetConnectionAsync();

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@bookingId", bookingId);

            string query = "SELECT EXISTS (SELECT \"Id\" " +
                           "FROM \"Payments\" " +
                           "WHERE \"BookingId\" = @bookingId)";

            bool result = await connection.ExecuteScalarAsync<bool>(query, parameters);

            return result;
        }

        /// <inheritdoc />
        public async Task<bool> RemovePaymentAsync(long paymentId, string transactionId)
        {
            using IDbConnection connection = await _db.GetConnectionAsync();

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@paymentId", paymentId);
            parameters.Add("@transactionId", transactionId);

            string query = "DELETE FROM \"Payments\" " +
                           "WHERE \"Id\" = @paymentId " +
                           "AND \"TransactionId\" = @transactionId";

            int row = await connection.ExecuteAsync(query, parameters);

            if (row <= 0)
            {
                throw new InvalidOperationException("Ошибка при удалении платежа. " +
                                                    $"PaymentId: {paymentId}. " +
                                                    $"TransactionId: {transactionId}");
            }

            return row > 0;
        }
    }
}
