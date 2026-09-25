import { Platform } from 'react-native';
import Constants from 'expo-constants';
import * as Notifications from 'expo-notifications';

let handlerConfigured = false;

function configureNotificationHandler() {
  if (handlerConfigured) {
    return;
  }

  Notifications.setNotificationHandler({
    handleNotification: async () => ({
      shouldPlaySound: true,
      shouldSetBadge: true,
      shouldShowBanner: true,
      shouldShowList: true,
    }),
  });

  handlerConfigured = true;
}

export async function configureNotifications() {
  try {
    // Android remote push is not available in Expo Go. It requires a development
    // build or a standalone app with the native notifications module included.
    if (Platform.OS === 'web' || Constants.appOwnership === 'expo') {
      return null;
    }

    // Configure this after the app runtime is ready, not while the module loads.
    // Calling it at module scope can trigger "runtime not ready" on Android.
    configureNotificationHandler();

    if (Platform.OS === 'android') {
      await Notifications.setNotificationChannelAsync('default', {
        name: 'default',
        importance: Notifications.AndroidImportance.MAX,
        vibrationPattern: [0, 250, 250, 250],
        lightColor: '#2457C5',
      });
    }

    const permission = await Notifications.getPermissionsAsync();
    let finalStatus = permission.status;

    if (finalStatus !== 'granted') {
      const requested = await Notifications.requestPermissionsAsync();
      finalStatus = requested.status;
    }

    if (finalStatus !== 'granted') {
      return null;
    }

    // The native device token is the token sent to Firebase Cloud Messaging.
    const deviceToken = await Notifications.getDevicePushTokenAsync();
    return deviceToken.data;
  } catch {
    // Notification registration can be unavailable on simulators and Expo Go.
    return null;
  }
}
