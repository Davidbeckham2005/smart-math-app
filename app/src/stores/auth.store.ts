import { create } from 'zustand';

import type { LoginPayload } from '../types/auth';  
import { authService } from '../services/auth.service';
import { ACCESS_TOKEN_KEY } from '../services/api';
import { deleteStorageItem, getStorageItem, setStorageItem } from '../services/storage';
import type { User } from '../types';

const USER_KEY = 'learning_companion.user';

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
      await setStorageItem(ACCESS_TOKEN_KEY, session.accessToken);
      await setStorageItem(USER_KEY, JSON.stringify(session.user));
      set({ user: session.user, isLoading: false });
      return true;
    } catch {
      set({ isLoading: false, error: 'Không thể đăng nhập. Vui lòng kiểm tra tài khoản.' });
      return false;
    }
  },

  loginDemo: async (role) => {
    const user: User = {
      id: role === 'parent' ? 1 : 2,
      fullName: role === 'parent' ? 'Nguyễn Minh Anh' : 'Trần Gia Bảo',
      email: `${role}@demo.local`,
      role,
    };

    await setStorageItem(USER_KEY, JSON.stringify(user));
    set({ user, error: null });
  },

  logout: async () => {
    await deleteStorageItem(ACCESS_TOKEN_KEY);
    await deleteStorageItem(USER_KEY);
    set({ user: null, error: null });
  },
}));
