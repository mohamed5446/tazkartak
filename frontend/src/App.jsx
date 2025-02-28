import { BrowserRouter, Navigate, Route, Routes } from "react-router";
import LoginPage from "./pages/LogIn";
import SignUpPage from "./pages/SignUp";
import MainLayout from "./components/MainLayout";
import Home from "./pages/Home";
import Profile from "./pages/user/profile";
import About from "./pages/About";
import ContactUs from "./pages/ContactUs";
import SearchResult from "./pages/searchResult";

import TripsManage from "./pages/companyPages/tripsManage";
import { useAuthStore } from "./store/authStore";
import EmailVerification from "./pages/EmailVerification";
import CompanySignUpPage from "./pages/companyPages/CompanyRegister";

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
          <Route
            path="/login"
            element={
              <RedirectAuthenticatedUser>
                <MainLayout Children={<LoginPage />} />
              </RedirectAuthenticatedUser>
            }
          />
          <Route
            path="/signup"
            element={
              <RedirectAuthenticatedUser>
                <MainLayout Children={<SignUpPage />} />
              </RedirectAuthenticatedUser>
            }
          />
          <Route
            path="/company-signup"
            element={
              <RedirectAuthenticatedUser>
                <MainLayout Children={<CompanySignUpPage />} />
              </RedirectAuthenticatedUser>
            }
          />

          <Route path="/" element={<MainLayout Children={<Home />} />} />
          <Route
            path="/user/profile"
            element={
              <ProtectedRoute>
                <MainLayout Children={<Profile />} />
              </ProtectedRoute>
            }
          ></Route>

          <Route path="/about" element={<MainLayout Children={<About />} />} />
          <Route
            path="/contact-us"
            element={
              <MainLayout>
                <ContactUs />
              </MainLayout>
            }
          />
          <Route
            path="/search-Result"
            element={<MainLayout Children={<SearchResult />} />}
          />
          <Route
            path="/contact-us"
            element={<MainLayout Children={<ContactUs />} />}
          />
          <Route
            path="/trips-manage"
            element={<MainLayout Children={<TripsManage />} />}
          />
          <Route
            path="/verify-email"
            element={
              <MainLayout
                Children={
                  <RedirectAuthenticatedUser>
                    <EmailVerification />
                  </RedirectAuthenticatedUser>
                }
              />
            }
          />
        </Routes>
      </BrowserRouter>
    </>
  );
}

export default App;
