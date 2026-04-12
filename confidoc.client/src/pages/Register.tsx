import axios, { AxiosError } from "axios";
import { useState, type JSX } from "react";
import { setToken, type IApiError } from "../globals";
import Message from "../components/Message";
import { useNavigate } from "react-router-dom";


function Register() {
    const [message, setMessage] = useState<JSX.Element>(<></>);
    const navigate = useNavigate();

    async function submit(form: FormData) {
        const username  = form.get('username');
        const password  = form.get('password');
        const password2 = form.get('password2');
        if (password != password2) {
            setMessage(<Message color="danger" text="passwords must match"/>)
            return;
        }

        try {
            const resp = await axios.post("/api/register", {
                username,
                password
            });
            setToken(resp.data.token ?? "");
            setMessage(<Message color="info" text="Registration successful!" />);
            setTimeout(() => { navigate("/login") }, 2500)
        } catch (error: AxiosError) {
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
        <main>
            <section className="flex justify-center mx-2">
                <form className="p-4 rounded-xl flex flex-col max-w-[800px] w-full gap-4 bg-[var(--same)]/10 backdrop-blur-[5px]
                                border-1 border-[var(--cont)]/15 mt-[calc(50px+5vh)]" action={submit}>
                    <h1 className="text-4xl uppercase font-semibold border-l-4 border-[var(--primary)] ps-2 ms-2">Register an account</h1>
                    <input placeholder="username" name="username"
                        className="border-2 border-[var(--primary)] outline-none focus:shadow-[0_0_5px_var(--primary)] 
                                    bg-[var(--same)]/25 text-xl p-2 rounded-xl"/>
                    <input placeholder="password" type="password" name="password"
                        className="border-2 border-[var(--primary)] outline-none focus:shadow-[0_0_5px_var(--primary)] 
                                    bg-[var(--same)]/25 text-xl p-2 rounded-xl"/>
                    <input placeholder="repeat password" type="password" name="password2"
                        className="border-2 border-[var(--primary)] outline-none focus:shadow-[0_0_5px_var(--primary)] 
                                    bg-[var(--same)]/25 text-xl p-2 rounded-xl"/>
                    <button className="w-full rounded-xl bg-[var(--primary)] p-3 uppercase font-semibold hover:shadow-[0_0_5px_var(--primary)]">Complete Registration</button>
                    {message}
                    <a onClick={()=>navigate("/login")} className="text-[var(--primary)] underline cursor-pointer w-fit">I already have an account</a>
                </form>
            </section>
        </main>
    );
}

export default Register;