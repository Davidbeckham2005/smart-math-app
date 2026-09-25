import { NavigationContainer } from '@react-navigation/native';
import { StatusBar } from 'expo-status-bar';
import { useEffect } from 'react';

import { RootNavigator } from './src/navigation/RootNavigator';
import { useAuthStore } from './src/stores/auth.store';
import { configureNotifications } from './src/services/notification.service';

export default function App() {
  const hydrate = useAuthStore((state) => state.hydrate);

  useEffect(() => {
    void hydrate();
    void configureNotifications();
  }, [hydrate]);

  return (
    <NavigationContainer>
      <RootNavigator />
      <StatusBar style="dark" />
    </NavigationContainer>
  );
}
