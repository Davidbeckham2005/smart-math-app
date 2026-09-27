import { api } from './api';
import type { AuthResponse } from '../types';
import type { LoginPayload } from '../types/auth';

export const authService = {
  async login(payload: LoginPayload) {
    const response = await api.post<AuthResponse>('/auth/login', payload);
    return response.data;
  },
};
