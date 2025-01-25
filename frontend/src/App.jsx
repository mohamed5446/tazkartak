import { BrowserRouter, Route, Routes } from "react-router";
import LoginPage from "./pages/login";
import SignUpPage from "./pages/signUp";
import MainLayout from "./components/MainLayout";

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
        </Routes>
      </BrowserRouter>
    </>
  );
}

export default App;
