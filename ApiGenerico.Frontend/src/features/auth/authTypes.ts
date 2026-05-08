export type LoginFormValues = {
  user: string;
  password: string;
};

export type AuthState = {
  token: string | null;
  currentUser: string | null;
  status: "idle" | "loading" | "authenticated" | "error";
  errorMessage: string | null;
};
