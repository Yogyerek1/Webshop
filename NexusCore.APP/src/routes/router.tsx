import { createBrowserRouter } from "react-router-dom";
import HomeLayout from "../layouts/HomeLayout";
import { HomePage } from "../pages/HomePage";
import { config } from "../config/config";

export const router = createBrowserRouter([
    {
        path: "/",
        element: <HomeLayout />,
        children: [
            { path: "/", element: <HomePage backgroundColor={config.homePage.props.backgroundColor} /> }
        ],
    },
    {
        path: "/auth",
        children: [
            { path: "login", /* element: <LoginPage /> */ },
            { path: "register", /* element: <RegisterPage /> */ },
            { path: "forgot-password", /* element: <ForgotPassword /> */ },
        ],
    },
    {
        path: "/checkout",
        /* element: <CheckoutPage />, */
    }
]);