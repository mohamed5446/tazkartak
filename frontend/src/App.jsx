import { BrowserRouter, Route, Routes } from "react-router";
import LoginPage from "./pages/login";
import SignUpPage from "./pages/signUp";
import MainLayout from "./components/MainLayout";
import About from "./pages/About";
import ContactUs from "./pages/ContactUs";

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
          <Route
            path="/about"
            element={<MainLayout Children={<About />} />}

          />
          <Route
          path="/contact-us"
          element={
            <MainLayout>
              <ContactUs />
            </MainLayout>
          }
        />
          
          
        </Routes>
      </BrowserRouter>
    </>
  );
}

export default App;
