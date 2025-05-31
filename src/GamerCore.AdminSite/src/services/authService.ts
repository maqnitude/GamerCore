import { ApiResponse, LoginPayload } from "../types";

const baseApiEndpoint = "/api/auth";

const ACCESS_TOKEN = "access_token";

const authService = {
  login: async (payload: LoginPayload): Promise<string> => {
    const apiEndpoint = `${baseApiEndpoint}/admin/login`;

    const response = await fetch(apiEndpoint, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload)
    });

    if (!response.ok) {
      const errorResponse = await response.json() as ApiResponse<object>;
      throw new Error(`${response.status} - ${response.statusText}: ${errorResponse.error?.message}`);
    }

    const successResponse = await response.json() as ApiResponse<string>;

    const token = successResponse.data as string;
    localStorage.setItem(ACCESS_TOKEN, token);

    return token;
  },

  logout: (): void => {
    const apiEndpoint = `${baseApiEndpoint}/admin/logout`
    const token = localStorage.getItem(ACCESS_TOKEN);

    // Call server to log the event
    if (token) {
      fetch(apiEndpoint, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`
        }
      }).catch(() => { /* Silent fail */ })
    }

    localStorage.removeItem(ACCESS_TOKEN);
  },

  isAuthenticated: (): boolean => {
    const token = localStorage.getItem(ACCESS_TOKEN);
    if (!token) {
      return false;
    }

    try {
      const jwtPayload = JSON.parse(atob(token.split('.')[1]));
      // Convert to miliseconds
      const expiry = jwtPayload.exp * 1000;

      return Date.now() < expiry;
    } catch {
      return false;
    }
  },

  getToken: (): string | null => {
    return localStorage.getItem(ACCESS_TOKEN);
  }
};

export default authService;