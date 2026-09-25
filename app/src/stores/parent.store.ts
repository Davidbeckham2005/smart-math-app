import { create } from 'zustand';

import type { Student } from '../types';

interface ParentState {
  children: Student[];
  selectedChildId: number | null;
  setChildren: (children: Student[]) => void;
  selectChild: (id: number) => void;
}

export const useParentStore = create<ParentState>((set) => ({
  children: [],
  selectedChildId: null,
  setChildren: (children) => set({ children }),
  selectChild: (selectedChildId) => set({ selectedChildId }),
}));
