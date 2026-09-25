import { Platform } from 'react-native';
import * as SecureStore from 'expo-secure-store';

function isWebStorageAvailable() {
  return Platform.OS === 'web' && typeof globalThis.localStorage !== 'undefined';
}

export async function getStorageItem(key: string) {
  if (isWebStorageAvailable()) {
    return globalThis.localStorage.getItem(key);
  }

  return SecureStore.getItemAsync(key);
}

export async function setStorageItem(key: string, value: string) {
  if (isWebStorageAvailable()) {
    globalThis.localStorage.setItem(key, value);
    return;
  }

  await SecureStore.setItemAsync(key, value);
}

export async function deleteStorageItem(key: string) {
  if (isWebStorageAvailable()) {
    globalThis.localStorage.removeItem(key);
    return;
  }

  await SecureStore.deleteItemAsync(key);
}
