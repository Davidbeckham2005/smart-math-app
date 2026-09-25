import { Pressable, StyleSheet, Text, View } from 'react-native';

import { FeatureCard } from '../../components/FeatureCard';
import { Screen } from '../../components/Screen';
import { useAuthStore } from '../../stores/auth.store';

export function ParentHomeScreen() {
  const user = useAuthStore((state) => state.user);
  const logout = useAuthStore((state) => state.logout);

  return (
    <Screen>
      <View style={styles.header}>
        <View>
          <Text style={styles.kicker}>PHỤ HUYNH</Text>
          <Text style={styles.title}>Xin chào, {user?.fullName.split(' ').pop()}.</Text>
        </View>
        <Pressable onPress={() => void logout()} style={styles.logoutButton}>
          <Text style={styles.logoutText}>Thoát</Text>
        </Pressable>
      </View>
      <Text style={styles.description}>Theo dõi hành trình học tập của con trong một nơi.</Text>
      <FeatureCard eyebrow="Hôm nay" title="Lịch học sắp tới" description="Xem lớp học, giáo viên và các mốc quan trọng trong tuần." />
      <FeatureCard eyebrow="Tiến độ" title="Báo cáo học tập" description="Theo dõi kết quả và những kỹ năng con đang rèn luyện." />
      <FeatureCard eyebrow="Thông báo" title="2 thông báo mới" description="Bài tập mới và cập nhật từ trung tâm đang chờ bạn." />
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
    color: '#2457C5',
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
