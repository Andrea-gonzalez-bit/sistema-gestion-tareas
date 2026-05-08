import { appConfig } from "../config";

type RequestOptions = RequestInit & {
  token?: string | null;
};

export async function apiRequest<T>(path: string, options: RequestOptions = {}): Promise<T> {
  const headers = new Headers(options.headers);
  headers.set("Content-Type", "application/json");

  if (options.token) {
    headers.set("Authorization", `Bearer ${options.token}`);
  }

  let response: Response;

  try {
    response = await fetch(`${appConfig.apiBaseUrl}${path}`, {
      ...options,
      headers
    });
  } catch (error) {
    if (error instanceof TypeError) {
      throw new Error(
        "No fue posible conectar con la API. Verifica que el backend esté encendido, que la URL configurada sea correcta y que el certificado HTTPS esté confiado."
      );
    }

    throw new Error("Ocurrió un error inesperado al intentar comunicarse con la API.");
  }

  if (!response.ok) {
    const payload = await response.json().catch(() => null);
    const message =
      payload?.Message ??
      payload?.errorMessage ??
      "No fue posible completar la solicitud al servidor.";

    throw new Error(message);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return response.json() as Promise<T>;
}
