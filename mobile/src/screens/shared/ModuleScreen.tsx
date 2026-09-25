import { StyleSheet, Text } from 'react-native';

import { FeatureCard } from '../../components/FeatureCard';
import { Screen } from '../../components/Screen';

interface ModuleScreenProps {
  title: string;
  description: string;
  items: string[];
}

export function ModuleScreen({ title, description, items }: ModuleScreenProps) {
  return (
    <Screen>
      <Text style={styles.title}>{title}</Text>
      <Text style={styles.description}>{description}</Text>
      {items.map((item, index) => (
        <FeatureCard key={item} eyebrow={`Bước ${index + 1}`} title={item} description="Module đang sẵn sàng để kết nối REST API." />
      ))}
    </Screen>
  );
}

const styles = StyleSheet.create({
  title: {
    color: '#172033',
    fontSize: 30,
    fontWeight: '800',
    marginBottom: 10,
  },
  description: {
    color: '#667085',
    fontSize: 16,
    lineHeight: 24,
    marginBottom: 24,
  },
});
