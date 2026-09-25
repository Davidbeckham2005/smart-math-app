import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';

import { ModuleScreen } from '../screens/shared/ModuleScreen';
import { StudentHomeScreen } from '../screens/student/StudentHomeScreen';
import type { StudentTabParamList } from './types';

const Tab = createBottomTabNavigator<StudentTabParamList>();

const AssignmentsScreen = () => (
  <ModuleScreen title="Bài tập" description="Các bài tập được giao và thời hạn cần hoàn thành." items={['Bài tập đang làm', 'Bài tập đã hoàn thành']} />
);
const PracticeScreen = () => (
  <ModuleScreen title="Luyện tập" description="Luyện phản xạ với timer chạy trực tiếp trên thiết bị." items={['Toán tư duy', 'Đồng hồ phản xạ']} />
);
const TestsScreen = () => (
  <ModuleScreen title="Bài kiểm tra" description="Thực hiện các bài kiểm tra năng lực theo hướng dẫn." items={['Bài kiểm tra sắp tới', 'Lịch sử kiểm tra']} />
);
const ResultsScreen = () => (
  <ModuleScreen title="Kết quả" description="Xem điểm số và thời gian phản hồi của từng bài." items={['Kết quả gần đây', 'Chi tiết câu trả lời']} />
);
const ProgressScreen = () => (
  <ModuleScreen title="Tiến bộ" description="Theo dõi năng lực và mục tiêu học tập cá nhân." items={['Biểu đồ tiến bộ', 'Mục tiêu tuần này']} />
);

export function StudentNavigator() {
  return (
    <Tab.Navigator
      screenOptions={{
        headerShown: false,
        tabBarActiveTintColor: '#ED6A3A',
        tabBarInactiveTintColor: '#98A2B3',
        tabBarLabelStyle: { fontSize: 10, fontWeight: '700' },
        tabBarStyle: { height: 64, paddingBottom: 8, paddingTop: 6 },
      }}
    >
      <Tab.Screen name="Home" component={StudentHomeScreen} options={{ tabBarLabel: 'Tổng quan' }} />
      <Tab.Screen name="Assignments" component={AssignmentsScreen} options={{ tabBarLabel: 'Bài tập' }} />
      <Tab.Screen name="Practice" component={PracticeScreen} options={{ tabBarLabel: 'Luyện tập' }} />
      <Tab.Screen name="Tests" component={TestsScreen} options={{ tabBarLabel: 'Kiểm tra' }} />
      <Tab.Screen name="Results" component={ResultsScreen} options={{ tabBarLabel: 'Kết quả' }} />
      <Tab.Screen name="Progress" component={ProgressScreen} options={{ tabBarLabel: 'Tiến bộ' }} />
    </Tab.Navigator>
  );
}
