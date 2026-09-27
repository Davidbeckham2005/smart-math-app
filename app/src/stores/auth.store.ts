import { create } from 'zustand';
import axios from 'axios';

import type { AuthResponse, User } from '../types';
import type { LoginPayload } from '../types/auth';
import { authService } from '../services/auth.service';
import { ACCESS_TOKEN_KEY, API_BASE_URL } from '../services/api';
import { deleteStorageItem, getStorageItem, setStorageItem } from '../services/storage';

const USER_KEY = 'learning_companion.user';

const DEMO_ACCOUNTS = {
  parent: {
    email: 'parent@nienluan.local',
    password: 'Parent@123',
  },
  student: {
    email: 'student@nienluan.local',
    password: 'Student@123',
  },
} as const;

async function persistSession(session: AuthResponse) {
  await setStorageItem(ACCESS_TOKEN_KEY, session.accessToken);
  await setStorageItem(USER_KEY, JSON.stringify(session.user));
}

function getLoginErrorMessage(error: unknown) {
  if (axios.isAxiosError(error)) {
    if (!error.response) {
      return `Không kết nối được API tại ${API_BASE_URL}.`;
    }

    if (error.response.status === 401) {
      return 'Email hoặc mật khẩu không đúng.';
    }

    return `API trả về lỗi ${error.response.status}.`;
  }

  return 'Đăng nhập thành công nhưng không thể lưu phiên trên thiết bị.';
}

interface AuthState {
  user: User | null;
  isHydrating: boolean;
  isLoading: boolean;
  error: string | null;
  hydrate: () => Promise<void>;
  login: (payload: LoginPayload) => Promise<boolean>;
  loginDemo: (role: 'parent' | 'student') => Promise<void>;
  logout: () => Promise<void>;
}

export const useAuthStore = create<AuthState>((set) => ({
  user: null,
  isHydrating: true,
  isLoading: false,
  error: null,
  hydrate: async () => {
    try {
      const storedUser = await getStorageItem(USER_KEY);

      if (storedUser) {
        set({ user: JSON.parse(storedUser) as User });
      }
    } catch {
      await deleteStorageItem(USER_KEY);
    } finally {
      set({ isHydrating: false });
    }
  },

  login: async (payload) => {
    set({ isLoading: true, error: null });

    try {
      const session = await authService.login(payload);
      await persistSession(session);
      set({ user: session.user, isLoading: false });
      return true;
    } catch (error) {
      set({ isLoading: false, error: getLoginErrorMessage(error) });
      return false;
    }
  },

  loginDemo: async (role) => {
    set({ isLoading: true, error: null });

    try {
      const session = await authService.login(DEMO_ACCOUNTS[role]);
      await persistSession(session);
      set({ user: session.user, isLoading: false });
    } catch (error) {
      set({ isLoading: false, error: getLoginErrorMessage(error) });
    }
  },

  logout: async () => {
    await deleteStorageItem(ACCESS_TOKEN_KEY);
    await deleteStorageItem(USER_KEY);
    set({ user: null, error: null });
  },
}));
