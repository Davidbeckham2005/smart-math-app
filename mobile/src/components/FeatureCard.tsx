import { StyleSheet, Text, View } from 'react-native';

interface FeatureCardProps {
  eyebrow: string;
  title: string;
  description: string;
}

export function FeatureCard({ eyebrow, title, description }: FeatureCardProps) {
  return (
    <View style={styles.card}>
      <Text style={styles.eyebrow}>{eyebrow.toUpperCase()}</Text>
      <Text style={styles.title}>{title}</Text>
      <Text style={styles.description}>{description}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  card: {
    padding: 18,
    borderRadius: 18,
    backgroundColor: '#FFFFFF',
    borderWidth: 1,
    borderColor: '#E7EBF3',
    marginBottom: 12,
  },
  eyebrow: {
    color: '#2457C5',
    fontSize: 11,
    fontWeight: '700',
    letterSpacing: 1.2,
    marginBottom: 8,
  },
  title: {
    color: '#172033',
    fontSize: 18,
    fontWeight: '700',
    marginBottom: 6,
  },
  description: {
    color: '#667085',
    fontSize: 14,
    lineHeight: 21,
  },
});
