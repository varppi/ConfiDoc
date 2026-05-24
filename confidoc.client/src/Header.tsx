import { Moon01, Sun } from "@untitledui/icons";
import { signalTheme, getTheme, isLoggedIn } from "./globals";
import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";

function Header() {
    const [theme, setTheme] = useState<string>(getTheme()??"light");
    const navigate = useNavigate();
    useEffect(() => {
        signalTheme(theme);
    }, [theme])

  
    return (
        <header className="flex justify-center w-full mb-[calc(75px_+_5vh)]">
            <nav
                className="
                w-[calc(100%_-_5vw)] mt-3 bg-[var(--same)]/10 backdrop-blur-[4px] max-w-[100vw]
                border-2 border-[var(--primary)] rounded-full fixed gap-1 px-2
                flex z-1 font-bold"
            >
                {
                    isLoggedIn()
                    ? <>
                        <button onClick={() => navigate("/dashboard")}        className="uppercase hover:cursor-pointer p-3 mt-2 mb-2 text-[18px] hover:bg-gray-500/10 rounded-4xl">Dashboard</button>
                        <button onClick={() => navigate("/dashboard/account")}className="uppercase hover:cursor-pointer p-3 mt-2 mb-2 text-[18px] hover:bg-gray-500/10 rounded-4xl">Account</button>
                        <button onClick={() => navigate("/logout")}           className="uppercase hover:cursor-pointer p-3 mt-2 mb-2 text-[18px] hover:bg-gray-500/10 rounded-4xl">Log Out</button>
                    </>
                    : <>
                        <button onClick={() => navigate("/")}         className="uppercase hover:cursor-pointer p-3 mt-2 mb-2 text-[18px] hover:bg-gray-500/10 rounded-4xl">Home</button>
                        <button onClick={() => navigate("/login")}    className="uppercase hover:cursor-pointer p-3 mt-2 mb-2 text-[18px] hover:bg-gray-500/10 rounded-4xl">Login</button>
                        <button onClick={() => navigate("/register")} className="uppercase hover:cursor-pointer p-3 mt-2 mb-2 text-[18px] hover:bg-gray-500/10 rounded-4xl">Register</button>
                    </>
  
                }
              <div className="grow-1" />
                {
                    theme == "light"
                    ? <button onClick={()=>setTheme(theme == "light" ? "dark" : "light")} className="uppercase hover:cursor-pointer p-3 m-2 text-[18px] hover:bg-gray-500/10 rounded-4xl"><Sun /></button>
                    : <button onClick={()=>setTheme(theme == "light" ? "dark" : "light")} className="uppercase hover:cursor-pointer p-3 m-2 text-[18px] hover:bg-gray-500/10 rounded-4xl"><Moon01 /></button>
                }
            </nav>
        </header>
    );
}

export default Header;