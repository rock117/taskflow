import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import * as projectService from '../services/projectService';

// 异步 actions
export const fetchProjects = createAsyncThunk(
  'project/fetchProjects',
  async (params = {}, { rejectWithValue }) => {
    try {
      const response = await projectService.getProjects(params);
      return response.data;
    } catch (error) {
      return rejectWithValue(error.response?.data?.message || '获取项目列表失败');
    }
  }
);

export const createProject = createAsyncThunk(
  'project/createProject',
  async (projectData, { rejectWithValue }) => {
    try {
      const response = await projectService.createProject(projectData);
      return response.data;
    } catch (error) {
      return rejectWithValue(error.response?.data?.message || '创建项目失败');
    }
  }
);

export const updateProject = createAsyncThunk(
  'project/updateProject',
  async ({ id, ...projectData }, { rejectWithValue }) => {
    try {
      const response = await projectService.updateProject(id, projectData);
      return response.data;
    } catch (error) {
      return rejectWithValue(error.response?.data?.message || '更新项目失败');
    }
  }
);

export const deleteProject = createAsyncThunk(
  'project/deleteProject',
  async (projectId, { rejectWithValue }) => {
    try {
      await projectService.deleteProject(projectId);
      return projectId;
    } catch (error) {
      return rejectWithValue(error.response?.data?.message || '删除项目失败');
    }
  }
);

export const fetchProjectById = createAsyncThunk(
  'project/fetchProjectById',
  async (projectId, { rejectWithValue }) => {
    try {
      const response = await projectService.getProjectById(projectId);
      return response.data;
    } catch (error) {
      return rejectWithValue(error.response?.data?.message || '获取项目详情失败');
    }
  }
);

export const archiveProject = createAsyncThunk(
  'project/archiveProject',
  async (projectId, { rejectWithValue }) => {
    try {
      const response = await projectService.archiveProject(projectId);
      return response.data;
    } catch (error) {
      return rejectWithValue(error.response?.data?.message || '归档项目失败');
    }
  }
);

export const activateProject = createAsyncThunk(
  'project/activateProject',
  async (projectId, { rejectWithValue }) => {
    try {
      const response = await projectService.activateProject(projectId);
      return response.data;
    } catch (error) {
      return rejectWithValue(error.response?.data?.message || '激活项目失败');
    }
  }
);

export const fetchProjectTasks = createAsyncThunk(
  'project/fetchProjectTasks',
  async ({ projectId, params = {} }, { rejectWithValue }) => {
    try {
      const response = await projectService.getProjectTasks(projectId, params);
      return {
        projectId,
        ...response.data
      };
    } catch (error) {
      return rejectWithValue(error.response?.data?.message || '获取项目任务失败');
    }
  }
);

export const fetchProjectStatistics = createAsyncThunk(
  'project/fetchProjectStatistics',
  async (projectId, { rejectWithValue }) => {
    try {
      const response = await projectService.getProjectStatistics(projectId);
      return {
        projectId,
        statistics: response.data
      };
    } catch (error) {
      return rejectWithValue(error.response?.data?.message || '获取项目统计失败');
    }
  }
);

// 初始状态
const initialState = {
  projects: [],
  currentProject: null,
  projectTasks: {},
  projectStatistics: {},
  loading: false,
  currentProjectLoading: false,
  tasksLoading: {},
  error: null,
  pagination: {
    total: 0,
    page: 1,
    limit: 10,
    totalPages: 1
  },
  filters: {
    status: null,
    search: ''
  }
};

// Slice
const projectSlice = createSlice({
  name: 'project',
  initialState,
  reducers: {
    clearError: (state) => {
      state.error = null;
    },
    setCurrentProject: (state, action) => {
      state.currentProject = action.payload;
    },
    clearCurrentProject: (state) => {
      state.currentProject = null;
    },
    setFilters: (state, action) => {
      state.filters = { ...state.filters, ...action.payload };
    },
    clearFilters: (state) => {
      state.filters = {
        status: null,
        search: ''
      };
    },
    clearProjectTasks: (state, action) => {
      if (action.payload) {
        delete state.projectTasks[action.payload];
      } else {
        state.projectTasks = {};
      }
    }
  },
  extraReducers: (builder) => {
    builder
      // Fetch projects
      .addCase(fetchProjects.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchProjects.fulfilled, (state, action) => {
        state.loading = false;
        state.projects = action.payload.data || [];
        state.pagination = action.payload.pagination || initialState.pagination;
        state.error = null;
      })
      .addCase(fetchProjects.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      // Create project
      .addCase(createProject.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(createProject.fulfilled, (state, action) => {
        state.loading = false;
        state.projects.unshift(action.payload);
        state.error = null;
      })
      .addCase(createProject.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      // Update project
      .addCase(updateProject.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(updateProject.fulfilled, (state, action) => {
        state.loading = false;
        const index = state.projects.findIndex(p => p.id === action.payload.id);
        if (index !== -1) {
          state.projects[index] = action.payload;
        }
        if (state.currentProject?.id === action.payload.id) {
          state.currentProject = action.payload;
        }
        state.error = null;
      })
      .addCase(updateProject.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      // Delete project
      .addCase(deleteProject.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(deleteProject.fulfilled, (state, action) => {
        state.loading = false;
        state.projects = state.projects.filter(p => p.id !== action.payload);
        if (state.currentProject?.id === action.payload) {
          state.currentProject = null;
        }
        state.error = null;
      })
      .addCase(deleteProject.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      // Fetch project by ID
      .addCase(fetchProjectById.pending, (state) => {
        state.currentProjectLoading = true;
        state.error = null;
      })
      .addCase(fetchProjectById.fulfilled, (state, action) => {
        state.currentProjectLoading = false;
        state.currentProject = action.payload;
        state.error = null;
      })
      .addCase(fetchProjectById.rejected, (state, action) => {
        state.currentProjectLoading = false;
        state.error = action.payload;
      })
      // Archive project
      .addCase(archiveProject.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(archiveProject.fulfilled, (state, action) => {
        state.loading = false;
        const index = state.projects.findIndex(p => p.id === action.payload.id);
        if (index !== -1) {
          state.projects[index] = action.payload;
        }
        if (state.currentProject?.id === action.payload.id) {
          state.currentProject = action.payload;
        }
        state.error = null;
      })
      .addCase(archiveProject.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      // Activate project
      .addCase(activateProject.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(activateProject.fulfilled, (state, action) => {
        state.loading = false;
        const index = state.projects.findIndex(p => p.id === action.payload.id);
        if (index !== -1) {
          state.projects[index] = action.payload;
        }
        if (state.currentProject?.id === action.payload.id) {
          state.currentProject = action.payload;
        }
        state.error = null;
      })
      .addCase(activateProject.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      // Fetch project tasks
      .addCase(fetchProjectTasks.pending, (state, action) => {
        const projectId = action.meta.arg.projectId;
        state.tasksLoading[projectId] = true;
        state.error = null;
      })
      .addCase(fetchProjectTasks.fulfilled, (state, action) => {
        const { projectId, data, pagination } = action.payload;
        state.tasksLoading[projectId] = false;
        state.projectTasks[projectId] = {
          data,
          pagination
        };
        state.error = null;
      })
      .addCase(fetchProjectTasks.rejected, (state, action) => {
        const projectId = action.meta.arg.projectId;
        state.tasksLoading[projectId] = false;
        state.error = action.payload;
      })
      // Fetch project statistics
      .addCase(fetchProjectStatistics.fulfilled, (state, action) => {
        const { projectId, statistics } = action.payload;
        state.projectStatistics[projectId] = statistics;
      })
      .addCase(fetchProjectStatistics.rejected, (state, action) => {
        console.error('Failed to fetch project statistics:', action.payload);
      });
  }
});

// 导出 actions
export const {
  clearError,
  setCurrentProject,
  clearCurrentProject,
  setFilters,
  clearFilters,
  clearProjectTasks
} = projectSlice.actions;

// 导出 selectors
export const selectProjects = (state) => state.project.projects;
export const selectCurrentProject = (state) => state.project.currentProject;
export const selectProjectTasks = (state, projectId) => state.project.projectTasks[projectId];
export const selectProjectStatistics = (state, projectId) => state.project.projectStatistics[projectId];
export const selectProjectLoading = (state) => state.project.loading;
export const selectCurrentProjectLoading = (state) => state.project.currentProjectLoading;
export const selectTasksLoading = (state, projectId) => state.project.tasksLoading[projectId];
export const selectProjectError = (state) => state.project.error;
export const selectProjectPagination = (state) => state.project.pagination;
export const selectProjectFilters = (state) => state.project.filters;

// 导出 reducer
export default projectSlice.reducer;
