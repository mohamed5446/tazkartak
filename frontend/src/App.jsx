import { BrowserRouter, Navigate, Route, Routes } from "react-router";
import LoginPage from "./pages/LogIn";
import SignUpPage from "./pages/SignUp";
import MainLayout from "./components/MainLayout";
import Home from "./pages/Home";
import Profile from "./pages/user/Profile";
import About from "./pages/About";
import ContactUs from "./pages/ContactUs";
import SearchResult from "./pages/SearchResult";
import { useAuthStore } from "./store/authStore";
import EmailVerification from "./pages/EmailVerification";
import AdminProfile from "./pages/admin/Profile";
import Companies from "./pages/admin/Companies";
import Users from "./pages/admin/Users";
import AdminLayout from "./components/AdminLayout";
import TripDetails from "./pages/TripDetails";
import Tickets from "./pages/user/Tickets";
import InfoForm from "./components/InfoForm";
import ChangePassword from "./components/ChangePassword";
import CompanyTrips from "./pages/admin/CompanyTrips";
import AdminInfoForm from "./pages/admin/InfoForm";
import UserTickets from "./pages/admin/UserTickets";
import Companyprofile from "./pages/company/Profile";
import CompanyInfo from "./pages/company/InfoForm";
import ShowCompanyTrips from "./pages/company/Trips";
import ForgetPassword from "./pages/ForgetPassword";
import ResetPassword from "./pages/ResetPassword";
import Trip from "./pages/admin/Trip";
import Earnings from "./pages/company/Earnings";
const AdminPages = ({ children }) => {
  const { isAuthenticated, role } = useAuthStore();

  if (isAuthenticated && role === "Admin") {
    return children;
  } else {
    return <Navigate to={"/"} />;
  }
};
export const UserOnly = ({ children }) => {
  const { isAuthenticated, role } = useAuthStore();
  return isAuthenticated && role === "User" ? children : <Navigate to="/" />;
};

// Only for logged-in companies
export const CompanyOnly = ({ children }) => {
  const { isAuthenticated, role } = useAuthStore();
  return isAuthenticated && role === "Company" ? children : <Navigate to="/" />;
};
const RoleRedirect = () => {
  const { isAuthenticated, role } = useAuthStore();

  if (!isAuthenticated) return <Home />;

  if (role === "Admin") return <Navigate to="/admin/profile" />;
  if (role === "Company") return <Navigate to="/company/profile" />;

  return <Home />; // Keep regular users on `/`
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
            <Route path="" element={<RoleRedirect />} />
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

            <Route path="about" element={<About />} />
            <Route path="contact-us" element={<ContactUs />} />
            <Route path="search-Result" element={<SearchResult />} />
            <Route path="contact-us" element={<ContactUs />} />
            <Route
              path="verify-email"
              element={
                <RedirectAuthenticatedUser>
                  <EmailVerification />
                </RedirectAuthenticatedUser>
              }
            />
            <Route path="forget-password" element={<ForgetPassword />} />
            <Route path="reset-password" element={<ResetPassword />} />

            <Route
              path="profile"
              element={
                <UserOnly>
                  <Profile />
                </UserOnly>
              }
            >
              <Route path="" element={<InfoForm />} />
              <Route path="tickets" element={<Tickets />} />
              <Route path="change-password" element={<ChangePassword />} />
            </Route>
            <Route path="/tickets" element={<Tickets />} />
            <Route path="trip-details/:id" element={<TripDetails />} />
          </Route>
          <Route
            path="/admin"
            element={
              <AdminPages>
                <AdminLayout />
              </AdminPages>
            }
          >
            <Route path="profile" element={<AdminProfile />}>
              <Route path="" element={<AdminInfoForm />} />
              <Route path="change-password" element={<ChangePassword />} />
              <Route path="Companies" element={<Companies />} />
              <Route path="Users" element={<Users />} />
            </Route>
            <Route path="user/:id" element={<UserTickets />} />

            <Route path=":id" element={<CompanyTrips />} />

            <Route path="trip/:tripId" element={<Trip />} />
          </Route>
          <Route
            path="/company"
            element={
              <CompanyOnly>
                <AdminLayout />
              </CompanyOnly>
            }
          >
            <Route path="profile" element={<Companyprofile />}>
              <Route path="change-password" element={<ChangePassword />} />
              <Route path="" element={<CompanyInfo />} />
              <Route path="trips" element={<ShowCompanyTrips />} />
              <Route path="earnings" element={<Earnings />} />
            </Route>
            <Route path="trip/:tripId" element={<Trip />} />
          </Route>
          <Route path="*" element={<Navigate to={"/"} />} />
        </Routes>
      </BrowserRouter>
    </div>
  );
}

export default App;
