using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PharmacyInventoryManagement.Models;
using PharmacyInventoryManagement.Repositories;

namespace PharmacyInventoryManagement.Services
{
    public class PaymentService
    {
        private readonly PaymentRepository _paymentRepository;
        private readonly SaleRepository _saleRepository;

        public PaymentService(
            PaymentRepository paymentRepository,
            SaleRepository saleRepository)
        {
            _paymentRepository = paymentRepository;
            _saleRepository = saleRepository;
        }

        // 1. Add a payment for a completed sale
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

            // Get the total amount of the completed sale
            decimal? saleTotal =
                await _saleRepository.GetSaleTotalAsync(payment.SaleId);

            if (saleTotal == null)
            {
                throw new InvalidOperationException(
                    "Completed sale was not found.");
            }

            // Get the amount already paid for this sale
            decimal alreadyPaid =
                await _paymentRepository.GetPaidAmountBySaleIdAsync(
                    payment.SaleId);

            // Prevent payment from exceeding the sale total
            if (alreadyPaid + payment.Amount > saleTotal.Value)
            {
                throw new InvalidOperationException(
                    "Payment exceeds the remaining amount due.");
            }

            payment.PaymentStatus = "Paid";
            payment.PaidAt = DateTime.Now;

            return await _paymentRepository.AddAsync(payment);
        }

        // 2. Get all payments
        public async Task<List<Payment>> GetAllPaymentsAsync()
        {
            return await _paymentRepository.GetAllAsync();
        }

        // 3. Get a payment by ID
        public async Task<Payment?> GetPaymentByIdAsync(int paymentId)
        {
            if (paymentId <= 0)
            {
                throw new ArgumentException(
                    "Payment ID must be greater than zero.");
            }

            return await _paymentRepository.GetByIdAsync(paymentId);
        }

        // 4. Search payments by payment method
        public async Task<List<Payment>> SearchPaymentsByMethodAsync(
            string paymentMethod)
        {
            if (string.IsNullOrWhiteSpace(paymentMethod))
            {
                throw new ArgumentException(
                    "Payment method cannot be empty.");
            }

            return await _paymentRepository.SearchByMethodAsync(
                paymentMethod.Trim());
        }

        // 5. Get total paid amount across all payments
        public async Task<decimal> GetTotalPaymentsAsync()
        {
            return await _paymentRepository.GetTotalPaymentsAsync();
        }

        // 6. Get payments for a particular sale
        public async Task<List<Payment>> GetPaymentsBySaleIdAsync(
            int saleId)
        {
            if (saleId <= 0)
            {
                throw new ArgumentException(
                    "Sale ID must be greater than zero.");
            }

            return await _paymentRepository.GetPaymentsBySaleIdAsync(
                saleId);
        }

        // 7. Update a payment's status
        public async Task<bool> UpdatePaymentStatusAsync(
            int paymentId,
            string paymentStatus)
        {
            if (paymentId <= 0)
            {
                throw new ArgumentException(
                    "Payment ID must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(paymentStatus))
            {
                throw new ArgumentException(
                    "Payment status is required.");
            }

            string status = paymentStatus.Trim();

            if (status != "Pending" &&
                status != "Paid" &&
                status != "Failed" &&
                status != "Refunded")
            {
                throw new ArgumentException(
                    "Invalid payment status.");
            }

            return await _paymentRepository.UpdateStatusAsync(
                paymentId, status);
        }

        // 8. Get the amount already paid for a sale
        public async Task<decimal> GetPaidAmountBySaleIdAsync(int saleId)
        {
            if (saleId <= 0)
            {
                throw new ArgumentException(
                    "Sale ID must be greater than zero.");
            }

            return await _paymentRepository.GetPaidAmountBySaleIdAsync(
                saleId);
        }

        // 9. Mark a payment as failed
        public async Task<bool> MarkPaymentAsFailedAsync(int paymentId)
        {
            if (paymentId <= 0)
            {
                throw new ArgumentException(
                    "Payment ID must be greater than zero.");
            }

            return await _paymentRepository.UpdateStatusAsync(
                paymentId, "Failed");
        }

        public async Task<decimal> GetRemainingAmountAsync(int saleId)
        {
            if (saleId <= 0)
            {
                throw new ArgumentException(
                    "Sale ID must be greater than zero.");
            }

            decimal? saleTotal =
                await _saleRepository.GetSaleTotalAsync(saleId);

            if (saleTotal == null)
            {
                throw new InvalidOperationException(
                    "Completed sale was not found.");
            }

            decimal paidAmount =
                await _paymentRepository.GetPaidAmountBySaleIdAsync(saleId);

            decimal remainingAmount = saleTotal.Value - paidAmount;

            return Math.Max(0, remainingAmount);
        }

    }
}