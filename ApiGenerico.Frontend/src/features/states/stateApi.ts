import { apiRequest } from "../../shared/api/httpClient";
import type { SaveStatePayload, StateItem } from "./stateTypes";

export function getStates(token: string) {
  return apiRequest<StateItem[]>("/api/states", {
    method: "GET",
    token
  });
}

export function createState(token: string, payload: SaveStatePayload) {
  return apiRequest<StateItem>("/api/states", {
    method: "POST",
    token,
    body: JSON.stringify(payload)
  });
}

export function updateState(token: string, stateId: number, payload: SaveStatePayload) {
  return apiRequest<StateItem>(`/api/states/${stateId}`, {
    method: "PUT",
    token,
    body: JSON.stringify(payload)
  });
}

export function deleteState(token: string, stateId: number) {
  return apiRequest<void>(`/api/states/${stateId}`, {
    method: "DELETE",
    token
  });
}
