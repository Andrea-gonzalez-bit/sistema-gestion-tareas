import { apiRequest } from "../../shared/api/httpClient";
import type { PagedTaskResult, SaveTaskPayload, TaskItem, TaskState } from "./taskTypes";

type GetTasksParams = {
  token: string;
  pageNumber: number;
  pageSize: number;
  search?: string;
  stateId?: number;
  dueDateFrom?: string;
  dueDateTo?: string;
  sortBy?: string;
  sortDirection?: string;
};

function buildQuery(params: Record<string, string | number | undefined>) {
  const searchParams = new URLSearchParams();

  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== "") {
      searchParams.set(key, String(value));
    }
  });

  return searchParams.toString();
}

export function getTasks(params: GetTasksParams) {
  const query = buildQuery({
    PageNumber: params.pageNumber,
    PageSize: params.pageSize,
    Search: params.search,
    StateId: params.stateId,
    DueDateFrom: params.dueDateFrom,
    DueDateTo: params.dueDateTo,
    SortBy: params.sortBy,
    SortDirection: params.sortDirection
  });

  return apiRequest<PagedTaskResult>(`/api/tasks?${query}`, {
    method: "GET",
    token: params.token
  });
}

export function getTaskStates(token: string) {
  return apiRequest<TaskState[]>("/api/tasks/states", {
    method: "GET",
    token
  });
}

export function createTask(token: string, payload: SaveTaskPayload) {
  return apiRequest<TaskItem>("/api/tasks", {
    method: "POST",
    token,
    body: JSON.stringify(payload)
  });
}

export function updateTask(token: string, taskId: number, payload: SaveTaskPayload) {
  return apiRequest<TaskItem>(`/api/tasks/${taskId}`, {
    method: "PUT",
    token,
    body: JSON.stringify(payload)
  });
}

export function deleteTask(token: string, taskId: number) {
  return apiRequest<void>(`/api/tasks/${taskId}`, {
    method: "DELETE",
    token
  });
}
