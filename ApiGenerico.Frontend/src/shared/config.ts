export const appConfig = {
  apiBaseUrl: import.meta.env.VITE_API_BASE_URL ?? "https://localhost:7147",
  authEncryptionKey: import.meta.env.VITE_AUTH_ENCRYPTION_KEY ?? "ADNKey_2005_28_05",
  authEncryptionIv: import.meta.env.VITE_AUTH_ENCRYPTION_IV ?? "ADN_Vector"
};
