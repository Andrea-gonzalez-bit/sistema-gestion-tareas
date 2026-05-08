import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../../hooks";
import {
  clearAuthError,
  defaultEncryptedCredentials,
  login,
  selectAuthError,
  selectAuthStatus
} from "./authSlice";
import type { LoginFormValues } from "./authTypes";

export function LoginPage() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const status = useAppSelector(selectAuthStatus);
  const errorMessage = useAppSelector(selectAuthError);

  const {
    register,
    handleSubmit,
    formState: { errors }
  } = useForm<LoginFormValues>({
    defaultValues: defaultEncryptedCredentials
  });

  useEffect(() => {
    return () => {
      dispatch(clearAuthError());
    };
  }, [dispatch]);

  const onSubmit = handleSubmit(async (values) => {
    const result = await dispatch(login(values));
    if (login.fulfilled.match(result)) {
      navigate("/dashboard", { replace: true });
    }
  });

  return (
    <div className="login-page">
      <section className="login-hero">
        <div className="hero-badge">Control centralizado</div>
        <h1>Sistema de gestión de tareas</h1>
        <p>
          Un espacio claro para operar tareas, estados y seguimiento diario con una
          experiencia más ordenada y lista para demostración.
        </p>
      </section>

      <section className="login-card">
        <div className="card-heading">
          <span className="eyebrow">Acceso</span>
          <h2>Iniciar sesión</h2>
          <p>Las credenciales de acceso para la prueba ya quedaron precargadas en el formulario.</p>
        </div>

        <form className="login-form" onSubmit={onSubmit}>
          <label className="field">
            <span>User encriptado</span>
            <input
              type="text"
              placeholder="Credencial de acceso precargada"
              {...register("user", {
                required: "El usuario es obligatorio."
              })}
            />
            {errors.user ? <small>{errors.user.message}</small> : null}
          </label>

          <label className="field">
            <span>Password encriptado</span>
            <input
              type="text"
              placeholder="Credencial de acceso precargada"
              {...register("password", {
                required: "La contraseña es obligatoria."
              })}
            />
            {errors.password ? <small>{errors.password.message}</small> : null}
          </label>

          {errorMessage ? <div className="alert alert--error">{errorMessage}</div> : null}

          <div className="login-note">
            Las credenciales ya están listas. Solo valida que la API esté ejecutándose y presiona el botón de acceso.
          </div>

          <button type="submit" className="primary-button" disabled={status === "loading"}>
            {status === "loading" ? "Validando acceso..." : "Acceder "}
          </button>
        </form>
      </section>
    </div>
  );
}
