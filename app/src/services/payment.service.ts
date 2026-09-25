import { api } from './api';

export interface PaymentRequest {
  invoiceId: number;
  returnUrl: string;
}

export const paymentService = {
  async createPayment(payload: PaymentRequest) {
    const response = await api.post<{ checkoutUrl: string }>('/payments/checkout', payload);
    return response.data;
  },
};
