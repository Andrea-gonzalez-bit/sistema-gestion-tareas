export type TaskItem = {
  Id: number;
  Title: string;
  Description: string | null;
  DueDate: string | null;
  StateId: number;
  StateName: string;
  CreatedAt: string;
  UpdatedAt: string;
};

export type TaskState = {
  Id: number;
  Name: string;
  CreatedAt: string;
  UpdatedAt: string;
  TasksCount: number;
};

export type PagedTaskResult = {
  Items: TaskItem[];
  PageNumber: number;
  PageSize: number;
  TotalCount: number;
  TotalPages: number;
};

export type TaskFilters = {
  Search: string;
  StateId: string;
  DueDateFrom: string;
  DueDateTo: string;
  SortBy: string;
  SortDirection: string;
};

export type TaskFormValues = {
  Title: string;
  Description: string;
  DueDate: string;
  StateId: string;
};

export type SaveTaskPayload = {
  Title: string;
  Description: string | null;
  DueDate: string | null;
  StateId: number;
};
