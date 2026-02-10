import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import * as taskService from '../services/taskService';

// 异步 actions
export const fetchTasks = createAsyncThunk(
  'task/fetchTasks',
  async (params = {}, { rejectWithValue }) => {
    try {
      const response = await taskService.getTasks(params);
      return response.data;
    } catch (error) {
      return rejectWithValue(error.response?.data?.message || '获取任务列表失败');
    }
  }
);

export const fetchTaskById = createAsyncThunk(
  'task/fetchTaskById',
  async (taskId, { rejectWithValue }) => {
    try {
      const response = await taskService.getTaskById(taskId);
      return response.data;
    } catch (error) {
      return rejectWithValue(error.response?.data?.message || '获取任务详情失败');
    }
  }
);

export const createTask = createAsyncThunk(
  'task/createTask',
  async (taskData, { rejectWithValue }) => {
    try {
      const response = await taskService.createTask(taskData);
      return response.data;
    } catch (error) {
      return rejectWithValue(error.response?.data?.message || '创建任务失败');
    }
  }
);

export const updateTask = createAsyncThunk(
  'task/updateTask',
  async ({ id, ...taskData }, { rejectWithValue }) => {
    try {
      const response = await taskService.updateTask(id, taskData);
      return response.data;
    } catch (error) {
      return rejectWithValue(error.response?.data?.message || '更新任务失败');
    }
  }
);

export const deleteTask = createAsyncThunk(
  'task/deleteTask',
  async (taskId, { rejectWithValue }) => {
    try {
      await taskService.deleteTask(taskId);
      return taskId;
    } catch (error) {
      return rejectWithValue(error.response?.data?.message || '删除任务失败');
    }
  }
);

export const updateTaskStatus = createAsyncThunk(
  'task/updateTaskStatus',
  async ({ taskId, status }, { rejectWithValue }) => {
    try {
      const response = await taskService.updateTaskStatus(taskId, status);
      return response.data;
    } catch (error) {
      return rejectWithValue(error.response?.data?.message || '更新任务状态失败');
    }
  }
);

export const assignTask = createAsyncThunk(
  'task/assignTask',
  async ({ taskId, assigneeId }, { rejectWithValue }) => {
    try {
      const response = await taskService.assignTask(taskId, assigneeId);
      return response.data;
    } catch (error) {
      return rejectWithValue(error.response?.data?.message || '分配任务失败');
    }
  }
);

export const unassignTask = createAsyncThunk(
  'task/unassignTask',
  async (taskId, { rejectWithValue }) => {
    try {
      const response = await taskService.unassignTask(taskId);
      return response.data;
    } catch (error) {
      return rejectWithValue(error.response?.data?.message || '取消分配任务失败');
    }
  }
);

// 初始状态
const initialState = {
  tasks: [],
  currentTask: null,
  loading: false,
  currentTaskLoading: false,
  error: null,
  pagination: {
    total: 0,
    page: 1,
    limit: 20,
    totalPages: 1
  },
  filters: {
    status: null,
    priority: null,
    type: null,
    projectId: null,
    assignedToMe: false,
    createdByMe: false,
    search: ''
  }
};

// Slice
const taskSlice = createSlice({
  name: 'task',
  initialState,
  reducers: {
    clearError: (state) => {
      state.error = null;
    },
    setCurrentTask: (state, action) => {
      state.currentTask = action.payload;
    },
    clearCurrentTask: (state) => {
      state.currentTask = null;
    },
    setFilters: (state, action) => {
      state.filters = { ...state.filters, ...action.payload };
    },
    clearFilters: (state) => {
      state.filters = initialState.filters;
    },
    updateTaskInList: (state, action) => {
      const index = state.tasks.findIndex(t => t.id === action.payload.id);
      if (index !== -1) {
        state.tasks[index] = action.payload;
      }
      if (state.currentTask?.id === action.payload.id) {
        state.currentTask = action.payload;
      }
    },
    removeTaskFromList: (state, action) => {
      state.tasks = state.tasks.filter(t => t.id !== action.payload);
      if (state.currentTask?.id === action.payload) {
        state.currentTask = null;
      }
    }
  },
  extraReducers: (builder) => {
    builder
      // Fetch tasks
      .addCase(fetchTasks.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchTasks.fulfilled, (state, action) => {
        state.loading = false;
        state.tasks = action.payload.data || [];
        state.pagination = action.payload.pagination || initialState.pagination;
        state.error = null;
      })
      .addCase(fetchTasks.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      // Fetch task by ID
      .addCase(fetchTaskById.pending, (state) => {
        state.currentTaskLoading = true;
        state.error = null;
      })
      .addCase(fetchTaskById.fulfilled, (state, action) => {
        state.currentTaskLoading = false;
        state.currentTask = action.payload;
        state.error = null;
      })
      .addCase(fetchTaskById.rejected, (state, action) => {
        state.currentTaskLoading = false;
        state.error = action.payload;
      })
      // Create task
      .addCase(createTask.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(createTask.fulfilled, (state, action) => {
        state.loading = false;
        state.tasks.unshift(action.payload);
        state.pagination.total += 1;
        state.error = null;
      })
      .addCase(createTask.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      // Update task
      .addCase(updateTask.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(updateTask.fulfilled, (state, action) => {
        state.loading = false;
        const index = state.tasks.findIndex(t => t.id === action.payload.id);
        if (index !== -1) {
          state.tasks[index] = action.payload;
        }
        if (state.currentTask?.id === action.payload.id) {
          state.currentTask = action.payload;
        }
        state.error = null;
      })
      .addCase(updateTask.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      // Delete task
      .addCase(deleteTask.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(deleteTask.fulfilled, (state, action) => {
        state.loading = false;
        state.tasks = state.tasks.filter(t => t.id !== action.payload);
        state.pagination.total = Math.max(0, state.pagination.total - 1);
        if (state.currentTask?.id === action.payload) {
          state.currentTask = null;
        }
        state.error = null;
      })
      .addCase(deleteTask.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      // Update task status
      .addCase(updateTaskStatus.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(updateTaskStatus.fulfilled, (state, action) => {
        state.loading = false;
        const index = state.tasks.findIndex(t => t.id === action.payload.id);
        if (index !== -1) {
          state.tasks[index] = action.payload;
        }
        if (state.currentTask?.id === action.payload.id) {
          state.currentTask = action.payload;
        }
        state.error = null;
      })
      .addCase(updateTaskStatus.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      // Assign task
      .addCase(assignTask.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(assignTask.fulfilled, (state, action) => {
        state.loading = false;
        const index = state.tasks.findIndex(t => t.id === action.payload.id);
        if (index !== -1) {
          state.tasks[index] = action.payload;
        }
        if (state.currentTask?.id === action.payload.id) {
          state.currentTask = action.payload;
        }
        state.error = null;
      })
      .addCase(assignTask.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      // Unassign task
      .addCase(unassignTask.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(unassignTask.fulfilled, (state, action) => {
        state.loading = false;
        const index = state.tasks.findIndex(t => t.id === action.payload.id);
        if (index !== -1) {
          state.tasks[index] = action.payload;
        }
        if (state.currentTask?.id === action.payload.id) {
          state.currentTask = action.payload;
        }
        state.error = null;
      })
      .addCase(unassignTask.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      });
  }
});

// 导出 actions
export const {
  clearError,
  setCurrentTask,
  clearCurrentTask,
  setFilters,
  clearFilters,
  updateTaskInList,
  removeTaskFromList
} = taskSlice.actions;

// 导出 selectors
export const selectTasks = (state) => state.task.tasks;
export const selectCurrentTask = (state) => state.task.currentTask;
export const selectTaskLoading = (state) => state.task.loading;
export const selectCurrentTaskLoading = (state) => state.task.currentTaskLoading;
export const selectTaskError = (state) => state.task.error;
export const selectTaskPagination = (state) => state.task.pagination;
export const selectTaskFilters = (state) => state.task.filters;
export const selectTaskById = (state, taskId) => state.task.tasks.find(t => t.id === taskId);

// 导出 reducer
export default taskSlice.reducer;
