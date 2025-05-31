import { User } from "../types";
import authService from "./authService";

const baseApiEndpoint = "/api/users";

const UserService = {
  getUsers: async (): Promise<User[]> => {
    const apiEndpoint = baseApiEndpoint;

    const token = authService.getToken();
    if (!token) {
      throw new Error("Not authenticated");
    }

    const response = await fetch(apiEndpoint, {
      method: "GET",
      headers: {
        "Authorization": `Bearer ${token}`
      }
    });

    if (!response.ok) {
      throw new Error(`${response.status} - ${response.statusText}`);
    }

    return response.json();
  }
}

export default UserService;
