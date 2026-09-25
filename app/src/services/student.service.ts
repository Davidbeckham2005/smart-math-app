import { api } from './api';
import type { Student } from '../types';

export const studentService = {
  async getStudent(id: number) {
    const response = await api.get<Student>(`/students/${id}`);
    return response.data;
  },

  async getStudents() {
    const response = await api.get<Student[]>('/students');
    return response.data;
  },
};
