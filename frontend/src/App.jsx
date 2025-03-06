import { BrowserRouter, Navigate, Route, Routes } from "react-router";
import LoginPage from "./pages/LogIn";
import SignUpPage from "./pages/SignUp";
import MainLayout from "./components/MainLayout";
import Home from "./pages/Home";
import Profile from "./pages/user/Profile";
import About from "./pages/About";
import ContactUs from "./pages/ContactUs";
import SearchResult from "./pages/searchResult";

import TripsManage from "./pages/companyPages/tripsManage";
import { useAuthStore } from "./store/authStore";
import EmailVerification from "./pages/EmailVerification";
import CompanySignUpPage from "./pages/companyPages/CompanyRegister";
import AdminProfile from "./pages/admin/Profile";
import Companies from "./pages/admin/Companies";
import Users from "./pages/admin/users";

const ProtectedRoute = ({ children }) => {
  const { isAuthenticated } = useAuthStore();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  // if (!user.isVerified) {
  //   return <Navigate to="/verify-email" replace />;
  // }

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
    <>
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
            <Route
              path="user/profile"
              element={
                <ProtectedRoute>
                  <Profile />
                </ProtectedRoute>
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

            <Route path="admin/profile" element={<AdminProfile />} />
            <Route path="admin/Companies" element={<Companies />} />
            <Route path="admin/users" element={<Users />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </>
  );
}

export default App;
