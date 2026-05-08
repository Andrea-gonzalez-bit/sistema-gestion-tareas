import { createAsyncThunk, createSlice, type PayloadAction } from "@reduxjs/toolkit";
import type { RootState } from "../../store";
import { authenticate } from "./authApi";
import type { AuthState, LoginFormValues } from "./authTypes";

const storageKey = "task-management-session";

export const defaultEncryptedCredentials: LoginFormValues = {
  user: "9gk7sPAj9hE=",
  password: "NsX8xEav35+BvurRn3x2bANt7lnq2RJ6odp/zr3HQ+k="
};

type PersistedSession = Pick<AuthState, "token" | "currentUser">;

function loadSession(): PersistedSession {
  const session = localStorage.getItem(storageKey);

  if (!session) {
    return {
      token: null,
      currentUser: null
    };
  }

  try {
    return JSON.parse(session) as PersistedSession;
  } catch {
    return {
      token: null,
      currentUser: null
    };
  }
}

function persistSession(session: PersistedSession) {
  localStorage.setItem(storageKey, JSON.stringify(session));
}

const persistedSession = loadSession();

const initialState: AuthState = {
  token: persistedSession.token,
  currentUser: persistedSession.currentUser,
  status: persistedSession.token ? "authenticated" : "idle",
  errorMessage: null
};

export const login = createAsyncThunk<PersistedSession, LoginFormValues, { rejectValue: string }>(
  "auth/login",
  async (values, { rejectWithValue }) => {
    if (
      values.user !== defaultEncryptedCredentials.user ||
      values.password !== defaultEncryptedCredentials.password
    ) {
      return rejectWithValue("Credenciales incorrectas. Verifica el usuario y la contraseña.");
    }

    const response = await authenticate(values);

    const session = {
      token: response.token,
      currentUser: values.user
    };

    persistSession(session);
    return session;
  }
);

const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    logout(state) {
      state.token = null;
      state.currentUser = null;
      state.status = "idle";
      state.errorMessage = null;
      localStorage.removeItem(storageKey);
    },
    clearAuthError(state) {
      state.errorMessage = null;
      if (state.status === "error") {
        state.status = "idle";
      }
    }
  },
  extraReducers: (builder) => {
    builder
      .addCase(login.pending, (state) => {
        state.status = "loading";
        state.errorMessage = null;
      })
      .addCase(login.fulfilled, (state, action: PayloadAction<PersistedSession>) => {
        state.status = "authenticated";
        state.token = action.payload.token;
        state.currentUser = action.payload.currentUser;
      })
      .addCase(login.rejected, (state, action) => {
        state.status = "error";
        state.errorMessage =
          action.payload ?? action.error.message ?? "No fue posible iniciar sesión.";
      });
  }
});

export const { logout, clearAuthError } = authSlice.actions;
export const authReducer = authSlice.reducer;

export const selectAuthStatus = (state: RootState) => state.auth.status;
export const selectAuthError = (state: RootState) => state.auth.errorMessage;
export const selectIsAuthenticated = (state: RootState) => Boolean(state.auth.token);
export const selectCurrentUser = (state: RootState) => state.auth.currentUser;
export const selectToken = (state: RootState) => state.auth.token;
