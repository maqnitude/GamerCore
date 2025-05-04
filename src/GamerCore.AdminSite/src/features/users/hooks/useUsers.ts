import { useCallback, useEffect, useState } from "react";
import { User } from "../../../types";
import UserService from "../../../services/userService";

function useUsers() {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const fetchUsers = useCallback(async () => {
    try {
      setLoading(true);
      const data = await UserService.getUsers();
      setUsers(data);
      setError(null);
    } catch (err) {
      console.error("Error fetching users: ", err);
      setError(err instanceof Error ? err.message : "Unknown error occured");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchUsers();
  }, [fetchUsers]);

  return { users, loading, error };
}

export default useUsers;