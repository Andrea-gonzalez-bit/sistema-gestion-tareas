import { useState } from "react";
import type { ReactNode } from "react";
import { NavLink, Outlet, useLocation } from "react-router-dom";
import { logout, selectCurrentUser } from "../../features/auth/authSlice";
import { useAppDispatch, useAppSelector } from "../../hooks";
import { ConfirmDialog } from "../components/ConfirmDialog";

type IconName = "dashboard" | "tasks" | "states" | "modules" | "logout" | "user";

const summaryItem = { to: "/dashboard", label: "Resumen", icon: "dashboard" as IconName };
const crudItems = [
  { to: "/tasks", label: "Tareas", icon: "tasks" as IconName },
  { to: "/states", label: "Estados", icon: "states" as IconName }
];

function MenuIcon({ name }: { name: IconName }) {
  const paths: Record<IconName, ReactNode> = {
    dashboard: (
      <>
        <path d="M4 13h6V4H4v9Z" />
        <path d="M14 20h6v-9h-6v9Z" />
        <path d="M4 20h6v-3H4v3Z" />
        <path d="M14 7h6V4h-6v3Z" />
      </>
    ),
    tasks: (
      <>
        <path d="M9 6h11" />
        <path d="M9 12h11" />
        <path d="M9 18h11" />
        <path d="m4 6 1 1 2-2" />
        <path d="m4 12 1 1 2-2" />
        <path d="m4 18 1 1 2-2" />
      </>
    ),
    states: (
      <>
        <path d="M12 3 4 7l8 4 8-4-8-4Z" />
        <path d="m4 12 8 4 8-4" />
        <path d="m4 17 8 4 8-4" />
      </>
    ),
    modules: (
      <>
        <path d="M4 4h7v7H4V4Z" />
        <path d="M13 4h7v7h-7V4Z" />
        <path d="M4 13h7v7H4v-7Z" />
        <path d="M13 13h7v7h-7v-7Z" />
      </>
    ),
    logout: (
      <>
        <path d="M10 6H6a2 2 0 0 0-2 2v8a2 2 0 0 0 2 2h4" />
        <path d="M15 16l4-4-4-4" />
        <path d="M19 12H9" />
      </>
    ),
    user: (
      <>
        <path d="M20 21a8 8 0 0 0-16 0" />
        <path d="M12 13a5 5 0 1 0 0-10 5 5 0 0 0 0 10Z" />
      </>
    )
  };

  return (
    <svg className="menu-svg" viewBox="0 0 24 24" aria-hidden="true">
      {paths[name]}
    </svg>
  );
}

export function AppShell() {
  const dispatch = useAppDispatch();
  const currentUser = useAppSelector(selectCurrentUser);
  const location = useLocation();
  const [isCrudMenuOpen, setIsCrudMenuOpen] = useState(true);
  const [isSidebarCollapsed, setIsSidebarCollapsed] = useState(false);
  const [isLogoutDialogOpen, setIsLogoutDialogOpen] = useState(false);

  const currentSection =
    [summaryItem, ...crudItems].find((item) => location.pathname.startsWith(item.to))?.label ??
    "Resumen";

  const isCrudSectionActive = crudItems.some((item) => location.pathname.startsWith(item.to));

  function handleLogoutConfirm() {
    setIsLogoutDialogOpen(false);
    dispatch(logout());
  }

  return (
    <div className={isSidebarCollapsed ? "app-shell app-shell--collapsed" : "app-shell"}>
      <aside className="sidebar">
        <div className="sidebar-brand">
          <div className="brand-mark" aria-hidden="true">
            TM
          </div>
          <div className="brand-copy">
            <strong>Task Manager</strong>
            <span>Gestión de tareas</span>
          </div>
          <button
            type="button"
            className="sidebar-toggle"
            onClick={() => setIsSidebarCollapsed((current) => !current)}
            aria-label={isSidebarCollapsed ? "Expandir menú lateral" : "Contraer menú lateral"}
          >
            {isSidebarCollapsed ? ">" : "<"}
          </button>
        </div>

        <nav className="sidebar-nav" aria-label="Navegación principal">
          <NavLink
            to={summaryItem.to}
            className={({ isActive }) => (isActive ? "nav-link nav-link--active" : "nav-link")}
            title={summaryItem.label}
          >
            <span className="nav-icon">
              <MenuIcon name={summaryItem.icon} />
            </span>
            <span className="nav-text">{summaryItem.label}</span>
          </NavLink>

          <div className="crud-menu">
            <button
              type="button"
              className={isCrudSectionActive ? "crud-toggle crud-toggle--active" : "crud-toggle"}
              onClick={() => setIsCrudMenuOpen((current) => !current)}
              aria-expanded={isCrudMenuOpen}
              title="Módulos"
            >
              <span className="nav-icon">
                <MenuIcon name="modules" />
              </span>
              <span className="nav-text">Módulos</span>
              <span className="crud-toggle__chevron">{isCrudMenuOpen ? "v" : ">"}</span>
            </button>

            {isCrudMenuOpen ? (
              <div className="crud-submenu">
                {crudItems.map((item) => (
                  <NavLink
                    key={item.to}
                    to={item.to}
                    className={({ isActive }) =>
                      isActive ? "nav-link nav-link--sub nav-link--active" : "nav-link nav-link--sub"
                    }
                    title={item.label}
                  >
                    <span className="nav-icon">
                      <MenuIcon name={item.icon} />
                    </span>
                    <span className="nav-text">{item.label}</span>
                  </NavLink>
                ))}
              </div>
            ) : null}
          </div>
        </nav>

        <div className="sidebar-session">
          <div className="session-avatar" aria-hidden="true">
            <MenuIcon name="user" />
          </div>
          <div className="session-copy">
            <span>Sesión activa</span>
            <strong>{currentUser ?? "Usuario autenticado"}</strong>
          </div>
          <button type="button" className="logout-button" onClick={() => setIsLogoutDialogOpen(true)}>
            <span className="logout-icon" aria-hidden="true">
              <MenuIcon name="logout" />
            </span>
            <span className="logout-text">Cerrar sesión</span>
          </button>
        </div>
      </aside>

      <div className="main-panel">
        <header className="topbar">
          <div className="topbar-copy">
            <span className="eyebrow">{currentSection}</span>
            <strong>Panel operativo</strong>
            <small>Vista conectada al backend y lista para pruebas funcionales.</small>
          </div>
        </header>

        <main className="content-panel">
          <Outlet />
        </main>
      </div>

      {isLogoutDialogOpen ? (
        <ConfirmDialog
          title="Cerrar sesión"
          message="¿Deseas cerrar la sesión actual?"
          confirmLabel="Sí, cerrar"
          cancelLabel="No, continuar"
          onConfirm={handleLogoutConfirm}
          onCancel={() => setIsLogoutDialogOpen(false)}
        />
      ) : null}
    </div>
  );
}
