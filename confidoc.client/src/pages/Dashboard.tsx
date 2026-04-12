import { File02, LogOut01, Menu03, Users01 } from "@untitledui/icons";
import { JSX, useEffect, useState } from "react";
import DotBackground from "../components/DotBackground";
import Main from "../components/Dashboard/Main";
import Documents from "../components/Dashboard/Documents";
import NewDocument from "../components/Dashboard/NewDocument";
import { getTheme, getUsername, signalTheme } from "../globals";
import { BarChartSquare01 } from "@untitledui/icons";
import { User01 } from "@untitledui/icons";
import LogOut from "../components/LogOut";
import React from "react";
import { FilePlus02 } from "@untitledui/icons";
import { Outlet, useNavigate } from "react-router-dom";

export default function Dashboard() {
    const [showMenu, setShowMenu] = useState<boolean>(false);
    const [view, setView] = useState<JSX.Element>(<></>);
    const [theme, setTheme] = useState<string>(getTheme() ?? "light");
    const navigate = useNavigate();
    useEffect(() => {
        signalTheme(theme);
    }, [theme])

    function changeView(path: string) {
        navigate(path);
//        setView(element);
        setShowMenu(false);
    }

    return <>

        <DotBackground/>
        <main>
            <aside className="overflow-y-hidden">
                <div className="min-md:hidden ps-4 fixed top-[20px] z-2">
                    <button onClick={() => { setShowMenu(!showMenu) }}
                            className="">
                        <Menu03 size="35px"/>
                    </button>
                </div>
                {
                    showMenu &&
                    <div className="fixed top-0 pt-[75px] z-1 h-full w-full overflow-y-hidden backdrop-blur bg-[var(--same)]/50 min-md:hidden">
                        <ul className="flex flex-col h-full gap-5 flex flex-col items-start text-xl text-[var(--primary)] font-semibold uppercase">
                            <div className="ps-5 flex flex-col gap-5 text-2xl">
                                <button className="flex-1 w-fit whitespace-nowrap cursor-pointer flex items-center gap-1 uppercase hover:text-[var(--cont)]"
                                    onClick={() => { changeView("/dashboard") }}><BarChartSquare01 size="35px" /> Dashboard</button>
                                <button className="flex-1 w-fit whitespace-nowrap cursor-pointer flex items-center gap-1 uppercase hover:text-[var(--cont)]"
                                    onClick={() => { changeView("/dashboard/documents") }}><File02 size="35px" /> Documents</button>
                                <button className="flex-1 w-fit whitespace-nowrap cursor-pointer flex items-center gap-1 uppercase hover:text-[var(--cont)]"
                                    onClick={() => { changeView("/dashboard/groups") }}><Users01 size="35px" /> Groups</button>
                            </div>
                            <div className="grow-1"></div>
                            <div className="w-full p-3 justify-end flex gap-5">
                                <button className="p-1 w-full hover:text-[var(--cont)] text-[var(--danger)]  border-1 flex justify-center"
                                    onClick={() => { changeView("/logout") }}><LogOut01 size="50px" /></button>
                                <button className="p-1 w-full hover:text-[var(--cont)] text-[var(--primary)] border-1 flex justify-center"
                                    onClick={() => { changeView("/dashboard/account") }}><User01 size="50px" /></button>
                            </div>
                        </ul>
                    </div>
                }
                <div className="fixed top-0 backdrop-blur h-full w-[210px] flex flex-col overflow-xy-hidden max-md:hidden border-e-1 border-[var(--cont)]/10">
                    <p className="ps-3 mb-5 mt-5 uppercase text-[var(--cont)] break-all ">Logged in as<br></br>{getUsername()}</p>
                    <ul className="flex flex-col gap-[calc(20px+0.2vh)] flex flex-col items-start ps-2 text-xl text-[var(--primary)] font-semibold uppercase">
                        <button className="flex-1 w-fit whitespace-nowrap cursor-pointer flex items-center gap-1 uppercase hover:text-[var(--cont)]"
                            onClick={() => { changeView("/dashboard") }}><BarChartSquare01/> Dashboard</button>
                        <button className="flex-1 w-fit whitespace-nowrap cursor-pointer flex items-center gap-1 uppercase hover:text-[var(--cont)]"
                            onClick={() => { changeView("/dashboard/documents") }}><File02 /> Documents</button>
                        <button className="flex-1 w-fit whitespace-nowrap cursor-pointer flex items-center gap-1 uppercase hover:text-[var(--cont)]"
                            onClick={() => { changeView("/dashboard/groups") }}><Users01 /> Groups</button>
                    </ul>
                    <div className="grow-1"></div>
                    <div className="w-full p-3 flex justify-between gap-5">
                        <button className="p-1 w-full hover:text-[var(--cont)] text-[var(--danger)]  border-1 flex justify-center"
                            onClick={() => { changeView("/logout") }}><LogOut01 size="30px" /></button>
                        <button className="p-1 w-full hover:text-[var(--cont)] text-[var(--primary)] border-1 flex justify-center"
                            onClick={() => { changeView("/dashboard/account") }}><User01 size="30px" /></button>
                    </div>
                </div>
            </aside>
            <div className="min-md:ms-[210px] max-md:mt-[75px] p-4">
                <Outlet/>
            </div>
        </main>
    
    </>

    
}
