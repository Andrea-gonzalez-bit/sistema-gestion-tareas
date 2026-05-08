export type StateItem = {
  Id: number;
  Name: string;
  CreatedAt: string;
  UpdatedAt: string;
  TasksCount: number;
};

export type StateFormValues = {
  Name: string;
};

export type SaveStatePayload = {
  Name: string;
};
