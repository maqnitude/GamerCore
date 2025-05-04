import { User } from "../../../types";

interface UsersTableProps {
  users: User[];
}

function UsersTable({ users }: UsersTableProps) {
  return (
    <div className="table-responsive">
      <table className="table table-striped table-hover">
        <thead>
          <tr>
            <th>ID</th>
            <th>Email</th>
            <th>First Name</th>
            <th>Last Name</th>
            <th>Role</th>
          </tr>
        </thead>
        <tbody>
          {users.map((user) => (
            <tr key={user.id}>
              <td>{user.id}</td>
              <td>{user.email}</td>
              <td>{user.firstName}</td>
              <td>{user.lastName}</td>
              <td className="d-flex flex-row justify-content-center align-items-center">
                {user.roles.map((role) => (
                  <span className="badge rounded-pill text-bg-primary">{role}</span>
                ))}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default UsersTable;