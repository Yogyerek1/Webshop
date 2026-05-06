import logoIcon from "../../public/logo.svg";
import cartIcon from "../../public/cart.svg";
import userIcon from "../../public/user.svg";
import type { AppConfig } from "./types/config.types";

export const config: AppConfig = {
    navBar: {
        props: {
            bgColor: "bg-white",
            border: "rounded-xl",
            shadow: "shadow-lg shadow-blue-500",
            position: "absolute top-4 left-4 right-4",
            padding: "p-2",
            cartIcon: {
                logoIcon: cartIcon,
                logoAlt: "cart",
                width: 55,
                height: 55,
                class: "mr-3",
            },
            userIcon: {
                logoIcon: userIcon,
                logoAlt: "user",
                width: 55,
                height: 55,
                class: "mr-3",
            },
            logo: {
                logoIcon: logoIcon,
                logoAlt: "logo",
                logoText: {
                    text: "NexusCore",
                    textColor: "text-[#2496ED]",
                    fontFamily: "font-sans",
                    fontWeight: "font-bold",
                    fontSize: "text-xl"
                }
            },
            navButtons: {
                bgColor: "bg-blue-400",
                selectedClass: "bg-blue-500 shadow-lg",
                border: "rounded-lg",
                padding: "p-3",
                textColor: "text-black",
                fontFamily: "font-sans",
                fontWeight: "font-medium",
                fontSize: "text-base",
                buttons: [
                    {
                        text: "home",
                        path: "/",
                    },
                    {
                        text: "products",
                        path: "/products",
                    },
                    {
                        text: "contact",
                        path: "/contact",
                    },
                ],
            },
        }
    },
    theme: {
        primaryColor: "text-blue-700"
    },
    homePage: {
        props: {
            backgroundColor: "bg-slate-100"
        }
    }
};