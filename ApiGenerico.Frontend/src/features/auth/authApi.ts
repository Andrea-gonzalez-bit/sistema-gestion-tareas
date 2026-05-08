import { apiRequest } from "../../shared/api/httpClient";
import type { LoginFormValues } from "./authTypes";

type LoginResponse = {
  token: string;
};

export async function authenticate(values: LoginFormValues): Promise<LoginResponse> {
  return apiRequest<LoginResponse>("/api/token/authentication", {
    method: "POST",
    body: JSON.stringify({
      user: values.user,
      password: values.password
    })
  });
}
