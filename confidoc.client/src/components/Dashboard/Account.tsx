import { useState } from "react";
import type { JSX } from "react/jsx-runtime";
import axios from "axios";
import { getConfig } from "../../globals";
import Message from "../Message";
import { type IApiError } from "../../globals";
import { useNavigate } from "react-router-dom";

function Account() {
    const [message, setMessage] = useState<JSX.Element>();
    const navigate = useNavigate();

    async function passwordSubmit(form: FormData) {
        const password: string = form.get("password")?.toString()??"";
        const newPassword: string = form.get("newpassword")?.toString()??"";
        const newPassword2: string = form.get("newpassword2")?.toString()??"";

        try {
            await axios.post("/api/user/changepassword", {
                password,
                newPassword,
                newPassword2
            }, getConfig())
            setMessage(<Message color="info" text="password changed successfully" />);
            setTimeout(() => {
                navigate(`/logout`);
            }, 2000);
        } catch(error) {
            try {
                const body: IApiError = error.response.data;
                let message = "";
                for (const errorPair of Object.entries(body.errors))
                    message += `${errorPair[1].join("\n")}\n`
                setMessage(<Message color="danger" text={message} />);
            } catch {
                setMessage(<Message color="danger" text="something went wrong!" />);
            }
        }
    }

    async function deleteSubmit(form: FormData) {
        const password: string = form.get("password")?.toString() ?? "";

        try {
            await axios.post("/api/user/delete", {
                password
            }, getConfig())
            setMessage(<Message color="info" text="account deleted successfully" />);
            setTimeout(() => {
                navigate(`/logout`);
            }, 2000);
        } catch (error) {
            try {
                const body: IApiError = error.response.data;
                let message = "";
                for (const errorPair of Object.entries(body.errors))
                    message += `${errorPair[1].join("\n")}\n`
                setMessage(<Message color="danger" text={message} />);
            } catch {
                setMessage(<Message color="danger" text="something went wrong!" />);
            }
        }
    }

    return (
        <section className="flex justify-center mx-2">
            <div className="p-4 rounded-xl flex flex-col max-w-[800px] w-full gap-4 bg-[var(--same)]/10 backdrop-blur-[5px]
                                border-1 border-[var(--cont)]/15 mt-[5vh]">
                <form className="flex flex-col w-full gap-4" action={passwordSubmit}>
                    <h1 className="text-4xl uppercase font-semibold border-l-4 border-[var(--primary)] ps-2 ms-2 mb-2">Account</h1>
                    <input placeholder="current password" type="password" name="password"
                        className="border-2 border-[var(--primary)] outline-none focus:shadow-[0_0_5px_var(--primary)] 
                                        bg-[var(--same)]/25 text-xl p-2 rounded-xl"/>
                    <input placeholder="new password" type="password" name="newpassword"
                        className="border-2 border-[var(--primary)] outline-none focus:shadow-[0_0_5px_var(--primary)] 
                                        bg-[var(--same)]/25 text-xl p-2 rounded-xl"/>
                    <input placeholder="repeat new password" type="password" name="newpassword2"
                        className="border-2 border-[var(--primary)] outline-none focus:shadow-[0_0_5px_var(--primary)] 
                                        bg-[var(--same)]/25 text-xl p-2 rounded-xl"/>
                    <button className="w-full rounded-xl bg-[var(--primary)] p-3 uppercase font-semibold hover:shadow-[0_0_5px_var(--primary)]">Create</button>
                    {message}
                </form>
                <div className="mt-5 mb-5 h-[2px] w-full bg-white/10"></div>
                <form className="flex flex-col w-full gap-4" action={deleteSubmit}>
                    <input placeholder="current password" type="password" name="password"
                        className="border-2 border-[var(--danger)] outline-none focus:shadow-[0_0_5px_var(--danger)] 
                                        bg-[var(--same)]/25 text-xl p-2 rounded-xl"/>
                    <button className="w-full rounded-xl bg-[var(--danger)] p-3 uppercase font-semibold hover:shadow-[0_0_5px_var(--danger)]">Delete Account</button>
                    {message}
                </form>
            </div>
        </section>
    );
}

export default Account;