import axios from 'axios';
import { deleteStorageItem, getStorageItem } from './storage';

export const ACCESS_TOKEN_KEY = 'learning_companion.access_token';

export const api = axios.create({
  baseURL: process.env.EXPO_PUBLIC_API_URL ?? 'http://localhost:5000/api',
  timeout: 15000,
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use(async (config) => {
  const token = await getStorageItem(ACCESS_TOKEN_KEY);

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      await deleteStorageItem(ACCESS_TOKEN_KEY);
    }

    return Promise.reject(error);
  },
);
