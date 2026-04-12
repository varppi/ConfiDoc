import { useEffect } from "react";
import { logOut } from "../globals";
import { TailSpin } from "react-loader-spinner";

function LogOut() {
    useEffect(() => {
        logOut();
        setTimeout(() => {
            window.location.replace("/");
        }, 2000)
    }, []);
    return <div className="flex w-full justify-center mt-[25vh]">
        <div>
            <div className="flex justify-center mb-5">
                <TailSpin
                    height="150"
                    width="150"
                    color="var(--primary)"
                    radius="1"
                    strokeWidth="2"
                    wrapperStyle={{}}
                    wrapperClass=""
                />
            </div>
            <b className="text-xl">Logging you out...</b>
        </div>
    </div>;
}

export default LogOut;