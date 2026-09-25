import { api } from './api';
import type { Assignment, QuizResult } from '../types';

export const assignmentService = {
  async getAssignments(studentId: number) {
    const response = await api.get<Assignment[]>(`/students/${studentId}/assignments`);
    return response.data;
  },

  async submitQuizResult(result: QuizResult) {
    const response = await api.post('/quiz-results', result);
    return response.data;
  },
};
