import { NavLink } from "react-router";

function Sidebar() {
  const activeStyle = ({ isActive }: { isActive: boolean }) => ({
    backgroundColor: isActive ? "#0d6efd" : "",
    color: "white",
    fontWeight: isActive ? "bold" : ""
  });

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
        </ul>
      </div>
    </>
  );
}

export default Sidebar;