import { Outlet } from "react-router";
import Sidebar from "../components/Sidebar";

function MainLayout() {
  return (
    <>
      <div className="container-fluid">
        <div className="row">
          <div className="col-2 m-0 p-0">
            <Sidebar />
          </div>
          <div className="col-10 m-0 p-0">
            {/* Alert container */}
            <div id="alertContainer">
            </div>

            {/* Main page content */}
            <main>
              <Outlet />
            </main>
          </div>
        </div>
      </div>
    </>
  );
}

export default MainLayout;