import { Pressable, StyleSheet, Text, View } from 'react-native';

import { FeatureCard } from '../../components/FeatureCard';
import { Screen } from '../../components/Screen';
import { useAuthStore } from '../../stores/auth.store';

export function StudentHomeScreen() {
  const user = useAuthStore((state) => state.user);
  const logout = useAuthStore((state) => state.logout);

  return (
    <Screen>
      <View style={styles.header}>
        <View>
          <Text style={styles.kicker}>HỌC SINH</Text>
          <Text style={styles.title}>Chào {user?.fullName.split(' ').pop()}!</Text>
        </View>
        <Pressable onPress={() => void logout()} style={styles.logoutButton}>
          <Text style={styles.logoutText}>Thoát</Text>
        </Pressable>
      </View>
      <Text style={styles.description}>Sẵn sàng chinh phục thử thách hôm nay?</Text>
      <FeatureCard eyebrow="Tiếp tục" title="Bài tập Toán tư duy" description="Hoàn thành 3 câu hỏi còn lại trong bài tập của bạn." />
      <FeatureCard eyebrow="Luyện tập" title="Phản xạ với đồng hồ" description="Rèn tốc độ trả lời và theo dõi thời gian của từng câu hỏi." />
      <FeatureCard eyebrow="Kết quả" title="Tiến bộ của bạn" description="Xem điểm số, thời gian phản hồi và các kỹ năng đã đạt được." />
    </Screen>
  );
}

const styles = StyleSheet.create({
  header: {
    alignItems: 'flex-start',
    flexDirection: 'row',
    justifyContent: 'space-between',
  },
  kicker: {
    color: '#ED6A3A',
    fontSize: 12,
    fontWeight: '800',
    letterSpacing: 1.2,
    marginBottom: 8,
  },
  title: {
    color: '#172033',
    fontSize: 28,
    fontWeight: '800',
  },
  description: {
    color: '#667085',
    fontSize: 16,
    lineHeight: 24,
    marginBottom: 24,
    marginTop: 12,
  },
  logoutButton: {
    paddingHorizontal: 8,
    paddingVertical: 6,
  },
  logoutText: {
    color: '#D92D20',
    fontSize: 14,
    fontWeight: '700',
  },
});
