import { useEffect, useState } from "react";
import ErrorAlert from "../components/ErrorAlert";
import LoadingSpinner from "../components/LoadingSpinner";
import UsersTable from "../features/users/components/UsersTable";
import useUsers from "../features/users/hooks/useUsers";
import { User } from "../types";

function CustomersPage() {
  const [localUsers, setLocalUsers] = useState<User[]>([]);

  const { users, loading, error } = useUsers();

  useEffect(() => {
    if (users) {
      setLocalUsers(users);
    }
  }, [users]);

  return (
    <div className="container-fluid my-4">
      {error && <ErrorAlert message={error.concat("\nFailed to fetch users.")} />}

      {loading && <LoadingSpinner />}
      {localUsers && (
        <>
          <UsersTable users={localUsers} />
        </>
      )}
    </div>
  );
}

export default CustomersPage;