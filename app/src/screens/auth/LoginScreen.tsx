import { useState } from 'react';
import { ActivityIndicator, Pressable, StyleSheet, Text, TextInput, View } from 'react-native';

import { Screen } from '../../components/Screen';
import { useAuthStore } from '../../stores/auth.store';

export function LoginScreen() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const login = useAuthStore((state) => state.login);
  const loginDemo = useAuthStore((state) => state.loginDemo);
  const isLoading = useAuthStore((state) => state.isLoading);
  const error = useAuthStore((state) => state.error);

  async function handleLogin() {
    await login({ email, password });
  }
  return (
    <Screen>
      <View style={styles.brandMark}>
        <Text style={styles.brandMarkText}>LC</Text>
      </View>
      <Text style={styles.kicker}>LEARNING COMPANION</Text>
      <Text style={styles.title}>Cùng con tiến bộ mỗi ngày.</Text>
      <Text style={styles.subtitle}>
        Kết nối phụ huynh và học sinh với lịch học, bài tập và kết quả học tập.
      </Text>

      <View style={styles.form}>
        <Text style={styles.label}>Email</Text>
        <TextInput
          autoCapitalize="none"
          autoComplete="email"
          keyboardType="email-address"
          onChangeText={setEmail}
          placeholder="you@example.com"
          placeholderTextColor="#98A2B3"
          style={styles.input}
          value={email}
        />
        <Text style={styles.label}>Mật khẩu</Text>
        <TextInput
          autoCapitalize="none"
          onChangeText={setPassword}
          placeholder="Nhập mật khẩu"
          placeholderTextColor="#98A2B3"
          secureTextEntry
          style={styles.input}
          value={password}
        />
        {error ? <Text style={styles.error}>{error}</Text> : null}
        <Pressable disabled={isLoading} onPress={() => void handleLogin()} style={styles.primaryButton}>
          {isLoading ? <ActivityIndicator color="#FFFFFF" /> : <Text style={styles.primaryText}>Đăng nhập</Text>}
        </Pressable>
      </View>

      <View style={styles.demoSection}>
        <Text style={styles.demoLabel}>Chưa kết nối API? Mở bản demo</Text>
        <View style={styles.demoRow}>
          <Pressable onPress={() => void loginDemo('parent')} style={styles.demoButton}>
            <Text style={styles.demoText}>Phụ huynh</Text>
          </Pressable>
          <Pressable onPress={() => void loginDemo('student')} style={styles.demoButton}>
            <Text style={styles.demoText}>Học sinh</Text>
          </Pressable>
        </View>
      </View>
    </Screen>
  );
}

const styles = StyleSheet.create({
  brandMark: {
    alignItems: 'center',
    backgroundColor: '#2457C5',
    borderRadius: 16,
    height: 56,
    justifyContent: 'center',
    marginBottom: 28,
    width: 56,
  },
  brandMarkText: {
    color: '#FFFFFF',
    fontSize: 20,
    fontWeight: '800',
  },
  kicker: {
    color: '#2457C5',
    fontSize: 12,
    fontWeight: '800',
    letterSpacing: 1.5,
    marginBottom: 12,
  },
  title: {
    color: '#172033',
    fontSize: 34,
    fontWeight: '800',
    lineHeight: 40,
    maxWidth: 450,
  },
  subtitle: {
    color: '#667085',
    fontSize: 16,
    lineHeight: 24,
    marginTop: 14,
    maxWidth: 500,
  },
  form: {
    marginTop: 34,
  },
  label: {
    color: '#344054',
    fontSize: 13,
    fontWeight: '700',
    marginBottom: 8,
  },
  input: {
    backgroundColor: '#FFFFFF',
    borderColor: '#D0D5DD',
    borderRadius: 12,
    borderWidth: 1,
    color: '#172033',
    fontSize: 16,
    height: 52,
    marginBottom: 16,
    paddingHorizontal: 16,
  },
  error: {
    color: '#D92D20',
    fontSize: 13,
    marginBottom: 14,
  },
  primaryButton: {
    alignItems: 'center',
    backgroundColor: '#2457C5',
    borderRadius: 12,
    height: 52,
    justifyContent: 'center',
  },
  primaryText: {
    color: '#FFFFFF',
    fontSize: 16,
    fontWeight: '700',
  },
  demoSection: {
    marginTop: 32,
  },
  demoLabel: {
    color: '#667085',
    fontSize: 13,
    marginBottom: 12,
  },
  demoRow: {
    flexDirection: 'row',
    gap: 10,
  },
  demoButton: {
    alignItems: 'center',
    backgroundColor: '#E9EFFD',
    borderRadius: 10,
    flex: 1,
    padding: 14,
  },
  demoText: {
    color: '#2457C5',
    fontSize: 14,
    fontWeight: '700',
  },
});
