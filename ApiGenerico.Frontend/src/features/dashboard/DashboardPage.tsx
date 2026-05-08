import { NavLink } from "react-router-dom";

export function DashboardPage() {
  return (
    <section className="dashboard-shell">
      <article className="surface-card dashboard-hero-card">
        <div className="dashboard-hero-copy">
          <span className="eyebrow">Resumen general</span>
          <h2>Sistema de gestión de tareas</h2>
          <p>
            Administra tareas, estados y seguimiento diario desde una vista limpia conectada
            con la API.
          </p>
        </div>

        <div className="dashboard-actions">
          <NavLink to="/tasks" className="dashboard-action">
            <span>Tareas</span>
            <strong>Gestionar tareas</strong>
          </NavLink>
          <NavLink to="/states" className="dashboard-action">
            <span>Estados</span>
            <strong>Administrar estados</strong>
          </NavLink>
        </div>
      </article>

      <div className="dashboard-metrics">
        <article className="surface-card">
          <span className="metric-icon">01</span>
          <h2>Login</h2>
          <p>Credenciales precargadas y flujo autenticado con JWT.</p>
        </article>

        <article className="surface-card">
          <span className="metric-icon">02</span>
          <h2>Tareas</h2>
          <p>Listado, filtros, paginación, creación, edición y eliminación.</p>
        </article>

        <article className="surface-card">
          <span className="metric-icon">03</span>
          <h2>Estados</h2>
          <p>Catálogo administrable con validaciones de negocio.</p>
        </article>
      </div>
    </section>
  );
}
