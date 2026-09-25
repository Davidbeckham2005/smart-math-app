import { create } from 'zustand';

import type { Assignment, QuizResult } from '../types';

interface StudentState {
  assignments: Assignment[];
  currentResult: QuizResult | null;
  timerStartedAt: number | null;
  setAssignments: (assignments: Assignment[]) => void;
  startTimer: () => void;
  stopTimer: () => number | null;
  setResult: (result: QuizResult) => void;
}

export const useStudentStore = create<StudentState>((set, get) => ({
  assignments: [],
  currentResult: null,
  timerStartedAt: null,
  setAssignments: (assignments) => set({ assignments }),
  startTimer: () => set({ timerStartedAt: Date.now() }),
  stopTimer: () => {
    const startedAt = get().timerStartedAt;
    set({ timerStartedAt: null });
    return startedAt ? (Date.now() - startedAt) / 1000 : null;
  },
  setResult: (currentResult) => set({ currentResult }),
}));
