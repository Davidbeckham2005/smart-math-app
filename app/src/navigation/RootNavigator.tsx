import { ActivityIndicator, StyleSheet, View } from 'react-native';

import { AuthNavigator } from './AuthNavigator';
import { ParentNavigator } from './ParentNavigator';
import { StudentNavigator } from './StudentNavigator';
import { useAuthStore } from '../stores/auth.store';

export function RootNavigator() {
  const user = useAuthStore((state) => state.user);
  const isHydrating = useAuthStore((state) => state.isHydrating);

  if (isHydrating) {
    return (
      <View style={styles.loading}>
        <ActivityIndicator color="#2457C5" size="large" />
      </View>
    );
  }

  if (!user) {
    return <AuthNavigator />;
  }

  return user.role === 'student' ? <StudentNavigator /> : <ParentNavigator />;
}

const styles = StyleSheet.create({
  loading: {
    alignItems: 'center',
    backgroundColor: '#F5F7FB',
    flex: 1,
    justifyContent: 'center',
  },
});
