import { BrowserRouter, Navigate, Route, Routes } from "react-router";
import LoginPage from "./pages/LogIn";
import SignUpPage from "./pages/SignUp";
import MainLayout from "./components/MainLayout";
import Home from "./pages/Home";
import Profile from "./pages/user/Profile";
import About from "./pages/About";
import ContactUs from "./pages/ContactUs";
import SearchResult from "./pages/SearchResult";
import TripsManage from "./pages/companyPages/tripsManage";
import { useAuthStore } from "./store/authStore";
import EmailVerification from "./pages/EmailVerification";
import CompanySignUpPage from "./pages/companyPages/CompanyRegister";
import AdminProfile from "./pages/admin/Profile";
import Companies from "./pages/admin/Companies";
import Users from "./pages/admin/Users";
import AdminLayout from "./components/AdminLayout";
import TripDetails from "./pages/TripDetails";
import Tickets from "./pages/user/Tickets";
import InfoForm from "./components/InfoForm";
import ChangePassword from "./pages/user/ChangePassword";
const AdminPages = ({ children }) => {
  const { isAuthenticated, role } = useAuthStore();

  if (isAuthenticated && role === "Admin") {
    return children;
  } else {
    return <Navigate to={"/"} />;
  }
};
// const CompanyPages = ({ children }) => {
//   const { isAuthenticated, role } = useAuthStore();

//   if (isAuthenticated && role === "Company") {
//     return children;
//   } else {
//     return <Navigate to={"/"} />;
//   }
// };
const ProtectedRoute = ({ children }) => {
  const { isAuthenticated } = useAuthStore();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return children;
};

const RedirectAuthenticatedUser = ({ children }) => {
  const { isAuthenticated } = useAuthStore();

  if (isAuthenticated) {
    return <Navigate to="/" replace />;
  }

  return children;
};

function App() {
  return (
    <div>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<MainLayout />}>
            <Route path="" element={<Home />} />
            <Route
              path="login"
              element={
                <RedirectAuthenticatedUser>
                  <LoginPage />
                </RedirectAuthenticatedUser>
              }
            />
            <Route
              path="signup"
              element={
                <RedirectAuthenticatedUser>
                  <SignUpPage />
                </RedirectAuthenticatedUser>
              }
            />
            <Route
              path="company-signup"
              element={
                <RedirectAuthenticatedUser>
                  <CompanySignUpPage />
                </RedirectAuthenticatedUser>
              }
            />

            <Route path="about" element={<About />} />
            <Route path="contact-us" element={<ContactUs />} />
            <Route path="search-Result" element={<SearchResult />} />
            <Route path="contact-us" element={<ContactUs />} />
            <Route path="trips-manage" element={<TripsManage />} />
            <Route
              path="verify-email"
              element={
                <RedirectAuthenticatedUser>
                  <EmailVerification />
                </RedirectAuthenticatedUser>
              }
            />
            <Route path="profile" element={<Profile />}>
              <Route path="" element={<InfoForm />} />
              <Route path="tickets" element={<Tickets />} />
              <Route path="change-password" element={<ChangePassword />} />
            </Route>
            <Route path="/tickets" element={<Tickets />} />
            <Route path="trip-details/:id" element={<TripDetails />} />
          </Route>
          <Route path="/admin" element={<AdminLayout />}>
            <Route
              path="profile"
              element={
                <AdminPages>
                  <AdminProfile />
                </AdminPages>
              }
            />
            <Route
              path="Companies"
              element={
                <AdminPages>
                  <Companies />
                </AdminPages>
              }
            />
            <Route
              path="users"
              element={
                <AdminPages>
                  <Users />
                </AdminPages>
              }
            />
          </Route>
          <Route path="*" element={<Navigate to={"/"} />} />
        </Routes>
      </BrowserRouter>
    </div>
  );
}

export default App;
