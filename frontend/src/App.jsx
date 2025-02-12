import { BrowserRouter, Route, Routes } from "react-router";
import LoginPage from "./pages/login";
import SignUpPage from "./pages/signUp";
import MainLayout from "./components/MainLayout";
import Home from "./pages/Home";
import Profile from "./pages/user/profile";
import About from "./pages/About";
import ContactUs from "./pages/ContactUs";

import TripsManage from "./pages/companyPages/tripsManage";


function App() {
  return (
    <>
      <BrowserRouter>
        <Routes>
          <Route
            path="/login"
            element={<MainLayout Children={<LoginPage />}></MainLayout>}
          />
          <Route
            path="/signup"
            element={<MainLayout Children={<SignUpPage />} />}
          />

          <Route path="/" element={<MainLayout Children={<Home />} />} />
          <Route
            path="/user/profile"
            element={<MainLayout Children={<Profile />} />}
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
          path="/contact-us"
          element={<MainLayout Children={<ContactUs />} />}
        />
          <Route
          path="/trips-manage"
          element={<MainLayout Children={<TripsManage />} />}
        />
          
          


        </Routes>
      </BrowserRouter>
    </>
  );
}

export default App;
