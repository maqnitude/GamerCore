import { NavLink, useNavigate } from "react-router";
import { useAuth } from "../contexts/AuthContext";

function Sidebar() {
  const { logout } = useAuth();

  const navigate = useNavigate();

  const activeStyle = ({ isActive }: { isActive: boolean }) => ({
    backgroundColor: isActive ? "#0d6efd" : "",
    color: "white",
    fontWeight: isActive ? "bold" : ""
  });

  const handleLogout = () => {
    logout();
    // Set replace true to prevent navigating back to the protected pages
    navigate("/login", { replace: true });
  }

  return (
    <>
      <div className="d-flex flex-column bg-dark vh-100 sticky-top">
        <NavLink
          to="/"
          className="d-flex justify-content-center align-items-center my-2 link-light text-decoration-none"
        >
          <span className="fw-bold fs-5">GAMERCORE.ADMIN</span>
        </NavLink>

        <hr className="border-white my-0" />

        <ul className="nav nav-pills flex-column">
          <li className="nav-item">
            <NavLink
              to="/"
              className="nav-link d-flex align-items-center"
              style={activeStyle}
              end
            >
              <i className="bi bi-speedometer2 me-2"></i>
              Dashboard
            </NavLink>
          </li>
          <li>
            <NavLink
              to="/products"
              className="nav-link d-flex align-items-center"
              style={activeStyle}
            >
              <i className="bi bi-box me-2"></i>
              Products
            </NavLink>
          </li>
          <li>
            <NavLink
              to="/categories"
              className="nav-link d-flex align-items-center"
              style={activeStyle}
            >
              <i className="bi bi-tags me-2"></i>
              Categories
            </NavLink>
          </li>
          <li>
            <NavLink
              to="/customers"
              className="nav-link d-flex align-items-center"
              style={activeStyle}
            >
              <i className="bi bi-people me-2"></i>
              Customers
            </NavLink>
          </li>
        </ul>

        <hr className="border-white my-0" />

        <button
          onClick={handleLogout}
          className="btn btn-outline-light m-3 mt-auto"
        >
          <i className="bi bi-box-arrow-right me-2"></i>
          Log out
        </button>
      </div>
    </>
  );
}

export default Sidebar;