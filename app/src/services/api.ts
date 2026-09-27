import axios from 'axios';
import { Platform } from 'react-native';
import { deleteStorageItem, getStorageItem } from './storage';

export const ACCESS_TOKEN_KEY = 'learning_companion.access_token';

const defaultApiUrl = Platform.OS === 'android'
  ? 'http://10.0.2.2:5000/api'
  : 'http://localhost:5000/api';

const configuredApiUrl = process.env.EXPO_PUBLIC_API_URL?.trim();
const configuredWebApiUrl = process.env.EXPO_PUBLIC_WEB_API_URL?.trim();

export const API_BASE_URL = Platform.OS === 'web'
  ? configuredWebApiUrl || 'http://localhost:5000/api'
  : configuredApiUrl || defaultApiUrl;

export const api = axios.create({
  baseURL: API_BASE_URL,
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
