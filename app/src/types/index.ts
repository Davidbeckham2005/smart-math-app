export type UserRole = 'admin' | 'teacher' | 'parent' | 'student';

export interface User {
  id: number;
  fullName: string;
  email: string;
  role: UserRole;
}

export interface AuthResponse {
  accessToken: string;
  user: User;
}

export interface Student {
  id: number;
  fullName: string;
  dateOfBirth: string;
  level: string;
}

export interface Assignment {
  id: number;
  title: string;
  subject: string;
  dueDate: string;
  completed: boolean;
}

export interface QuizResult {
  questionId: number;
  answer: number;
  isCorrect: boolean;
  responseTime: number;
}
