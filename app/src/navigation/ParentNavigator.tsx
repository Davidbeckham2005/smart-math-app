import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';

import { ModuleScreen } from '../screens/shared/ModuleScreen';
import { ParentHomeScreen } from '../screens/parent/ParentHomeScreen';
import type { ParentTabParamList } from './types';

const Tab = createBottomTabNavigator<ParentTabParamList>();

const ChildrenScreen = () => (
  <ModuleScreen title="Hồ sơ con" description="Quản lý các con và chuyển đổi hồ sơ đang theo dõi." items={['Danh sách con', 'Thông tin học tập']} />
);
const ScheduleScreen = () => (
  <ModuleScreen title="Lịch học" description="Lịch học và các buổi học sắp tới của con." items={['Lịch tuần', 'Chi tiết buổi học']} />
);
const ProgressScreen = () => (
  <ModuleScreen title="Tiến độ học tập" description="Tổng hợp kết quả và năng lực theo từng giai đoạn." items={['Tổng quan tiến độ', 'Kỹ năng cần cải thiện']} />
);
const TuitionScreen = () => (
  <ModuleScreen title="Học phí" description="Theo dõi công nợ và thanh toán học phí trực tuyến." items={['Hóa đơn hiện tại', 'Lịch sử thanh toán']} />
);
const NotificationsScreen = () => (
  <ModuleScreen title="Thông báo" description="Cập nhật mới nhất từ trung tâm và giáo viên." items={['Bài tập mới', 'Thay đổi lịch học']} />
);

export function ParentNavigator() {
  return (
    <Tab.Navigator
      screenOptions={{
        headerShown: false,
        tabBarActiveTintColor: '#2457C5',
        tabBarInactiveTintColor: '#98A2B3',
        tabBarLabelStyle: { fontSize: 10, fontWeight: '700' },
        tabBarStyle: { height: 64, paddingBottom: 8, paddingTop: 6 },
      }}
    >
      <Tab.Screen name="Home" component={ParentHomeScreen} options={{ tabBarLabel: 'Tổng quan' }} />
      <Tab.Screen name="Children" component={ChildrenScreen} options={{ tabBarLabel: 'Các con' }} />
      <Tab.Screen name="Schedule" component={ScheduleScreen} options={{ tabBarLabel: 'Lịch học' }} />
      <Tab.Screen name="Progress" component={ProgressScreen} options={{ tabBarLabel: 'Tiến độ' }} />
      <Tab.Screen name="Tuition" component={TuitionScreen} options={{ tabBarLabel: 'Học phí' }} />
      <Tab.Screen name="Notifications" component={NotificationsScreen} options={{ tabBarLabel: 'Tin mới' }} />
    </Tab.Navigator>
  );
}
