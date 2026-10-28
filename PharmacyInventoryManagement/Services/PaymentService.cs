
using System;
using System.Threading.Tasks;
using PharmacyInventoryManagement.Models;
using PharmacyInventoryManagement.Repositories;

namespace PharmacyInventoryManagement.Services
{
    public class PaymentService
    {
        private readonly PaymentRepository _paymentRepository;

        public PaymentService(PaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<int> AddPaymentAsync(Payment payment)
        {
            if (payment == null)
            {
                throw new ArgumentNullException(nameof(payment));
            }

            if (payment.SaleId <= 0)
            {
                throw new ArgumentException(
                    "Sale ID must be greater than zero.");
            }

            if (payment.Amount <= 0)
            {
                throw new ArgumentException(
                    "Payment amount must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(payment.PaymentMethod))
            {
                throw new ArgumentException(
                    "Payment method is required.");
            }

            payment.PaymentStatus = "Paid";
            payment.PaidAt = DateTime.Now;

            return await _paymentRepository.AddAsync(payment);
        }
    }
}
