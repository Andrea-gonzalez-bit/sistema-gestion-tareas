import { createBrowserRouter, Navigate } from "react-router-dom";
import { AppShell } from "./shared/layout/AppShell";
import { ProtectedRoute } from "./shared/layout/ProtectedRoute";
import { DashboardPage } from "./features/dashboard/DashboardPage";
import { LoginPage } from "./features/auth/LoginPage";
import { TasksPage } from "./features/tasks/TasksPage";
import { StatesPage } from "./features/states/StatesPage";

export const router = createBrowserRouter([
  {
    path: "/login",
    element: <LoginPage />
  },
  {
    path: "/",
    element: <Navigate to="/login" replace />
  },
  {
    element: (
      <ProtectedRoute>
        <AppShell />
      </ProtectedRoute>
    ),
    children: [
      {
        path: "dashboard",
        element: <DashboardPage />
      },
      {
        path: "tasks",
        element: <TasksPage />
      },
      {
        path: "states",
        element: <StatesPage />
      }
    ]
  },
  {
    path: "*",
    element: <Navigate to="/login" replace />
  }
]);
