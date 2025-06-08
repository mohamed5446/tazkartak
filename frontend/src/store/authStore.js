import { create } from "zustand";
import axios from "axios";
import { devtools, persist } from "zustand/middleware";
import Cookies from "js-cookie";
export const useAuthStore = create(
  devtools(
    persist((set) => ({
      user: null,
      User: {},
      isAuthenticated: false,
      error: null,
      isLoading: false,
      isCheckingAuth: true,
      role: null,
      isEmailConfirmed: false,
      id: null,
      setUser: (userData) => set({ User: userData }), // Function to update user state
      fetchUser: async (id) => {
        try {
          const JWTToken = Cookies.get("token");
          const res = await axios.get(
            `https://tazkartk-api.runasp.net/api/Users/${id}`,
            {
              headers: { Authorization: `Bearer ${JWTToken}` },
            }
          );
          set({ User: res.data });
        } catch (error) {
          console.error("Error fetching user data:", error);
        }
      },
      fetchCompany: async (id) => {
        try {
          const JWTToken = Cookies.get("token");
          const res = await axios.get(
            `https://tazkartk-api.runasp.net/api/Companies/${id}`,
            {
              headers: { Authorization: `Bearer ${JWTToken}` },
            }
          );
          set({ User: res.data });
        } catch (error) {
          console.error("Error fetching user data:", error);
        }
      },
      setuser: async (data) => {
        set({ user: data });
      },
      resetPassword: async (data) => {
        try {
          const response = await axios.put(
            "https://tazkartk-api.runasp.net/api/Account/Reset-Password",
            data
          );
          set({
            isAuthenticated: true,
            user: response.data.email,
            error: null,
            isLoading: false,
            role: response.data.roles[0],
            id: response.data.id,
            isEmailConfirmed: response.data.isEmailConfirmed,
          });
          return response.data;
        } catch (error) {
          console.log(error);
        }
      },
      userSignup: async (data) => {
        set({ isLoading: true, error: null });
        try {
          await axios.post(
            "https://tazkartk-api.runasp.net/api/Account/Register",
            data
          );
          set({
            user: data.email,
            isAuthenticated: false,
            isLoading: false,
            error: null,
            role: "User",
          });
          console.log(data);
        } catch (error) {
          set({
            error: error.response.data || "error signing up",
            isLoading: false,
          });
          throw error;
        }
      },
      companySignup: async (data) => {
        set({ isLoading: true, error: null });
        try {
          await axios.post(
            "https://tazkartk-api.runasp.net/api/Account/Company-Register",
            data
          );
          set({
            user: data.email,
            role: "Company",
            isAuthenticated: false,
            isLoading: false,
            error: null,
          });
          console.log(data);
        } catch (error) {
          set({
            error: error.response.data || "error signing up",
            isLoading: false,
          });
          throw error;
        }
      },
      adminSignup: async (data) => {
        set({ isLoading: true, error: null });
        try {
          await axios.post(
            "https://tazkartk-api.runasp.net/api/Account/Admin-Register",
            data
          );
          set({
            user: data.email,
            isAuthenticated: false,
            isLoading: false,
            error: null,
            role: "Admin",
          });
          console.log(data);
        } catch (error) {
          set({
            error: error.response.data || "error signing up",
            isLoading: false,
          });
          throw error;
        }
      },
      verifyEmail: async (data) => {
        set({ isLoading: true, error: null });
        try {
          const response = await axios.post(
            "https://tazkartk-api.runasp.net/api/Account/Verify-OTP",
            data
          );
          set({
            isAuthenticated: true,
            user: response.data.email,
            error: null,
            isLoading: false,
            role: response.data.roles[0],
            id: response.data.id,
            isEmailConfirmed: response.data.isEmailConfirmed,
          });
          return response.data;
        } catch (error) {
          set({
            error: error.response.data || "error verifying email",
            isLoading: false,
          });
          throw error;
        }
      },

      login: async (data) => {
        set({ isLoading: true, error: null });
        try {
          const response = await axios.post(
            "https://tazkartk-api.runasp.net/api/Account/Login",
            data
          );
          set({
            isAuthenticated: true,
            user: response.data.email,
            error: null,
            isLoading: false,
            role: response.data.roles[0],
            id: response.data.id,
            isEmailConfirmed: response.data.isEmailConfirmed,
          });
          return response.data;
        } catch (error) {
          set({
            error: error.response.data || "Error logging in",
            isLoading: false,
          });
          console.log(error);
          throw error;
        }
      },
      logout: async () => {
        set({
          user: null,
          isAuthenticated: false,
          error: null,
          isLoading: false,
          role: null,
          id: null,
          User: {},
          isEmailConfirmed: false,
        });
      },
      setdefaulte: async () => {
        set({
          id: null,
          user: null,
          isAuthenticated: false,
          error: null,
          isLoading: false,
          isCheckingAuth: true,
          role: null,
        });
      },
    }))
  )
);
