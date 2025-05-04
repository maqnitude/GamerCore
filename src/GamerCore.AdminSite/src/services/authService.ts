import { ApiResponse, LoginPayload, User } from "../types";

const baseApiEndpoint = "/api/auth";

interface LoginResponse {
  token: string;
  user: User;
}

const AUTH_TOKEN = "auth_token";
const AUTH_USER = "auth_user";

const authService = {
  login: async (payload: LoginPayload): Promise<LoginResponse> => {
    const apiEndpoint = `${baseApiEndpoint}/loginJwt`;

    const response = await fetch(apiEndpoint, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload)
    });

    if (!response.ok) {
      throw new Error(`${response.status} - ${response.statusText}`);
    }

    const apiResponse = await response.json() as ApiResponse;

    if (!apiResponse.succeeded || !apiResponse.data) {
      throw new Error(apiResponse.message || "Login failed");
    }

    const { token, user } = apiResponse.data as LoginResponse;
    localStorage.setItem(AUTH_TOKEN, token);
    localStorage.setItem(AUTH_USER, JSON.stringify(user));

    return apiResponse.data as LoginResponse;
  },

  logout: (): void => {
    const apiEndpoint = `${baseApiEndpoint}/logoutJwt`
    const token = localStorage.getItem(AUTH_TOKEN);

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

    localStorage.removeItem(AUTH_TOKEN);
    localStorage.removeItem(AUTH_USER);
  },

  isAuthenticated: (): boolean => {
    const token = localStorage.getItem(AUTH_TOKEN);
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
    return localStorage.getItem(AUTH_TOKEN);
  },

  getUser: (): User | null => {
    const json = localStorage.getItem(AUTH_USER);
    return json ? JSON.parse(json) as User : null;
  }
};

export default authService;