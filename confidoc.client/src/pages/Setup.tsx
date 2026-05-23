import axios from "axios";
import { useState, type JSX } from "react";
import { setToken, setUsername, type IApiError } from "../globals";
import Message from "../components/Message";
import { useNavigate } from "react-router-dom";

function Setup() {
    const [message, setMessage] = useState<JSX.Element>(<></>);

    async function submit(form: FormData) {
        const password = form.get('password');
        const password2 = form.get('password2');
        if (password != password2) {
            setMessage(<Message color="danger" text={"passwords do not match"} />)
            return
        }
        try {
            await axios.post("/api/setup", {
                password,
                password2
            });
            setMessage(<Message color="info" text="Account created! The username is 'admin'." />);
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
        <main>
            <section className="flex justify-center mx-2">
                <form className="p-4 rounded-xl flex flex-col max-w-[800px] w-full gap-4 bg-[var(--same)]/10 backdrop-blur-[5px]
                                border-1 border-[var(--cont)]/15 mt-[calc(50px+5vh)]" action={submit}>
                    <h1 className="text-4xl uppercase font-semibold border-l-4 border-[var(--primary)] ps-2 ms-2">Create admin password</h1>
                    <input placeholder="password" type="password" name="password"
                        className="border-2 border-[var(--primary)] outline-none focus:shadow-[0_0_5px_var(--primary)] 
                                    bg-[var(--same)]/25 text-xl p-2 rounded-xl"/>
                    <input placeholder="repeat password" type="password" name="password2"
                        className="border-2 border-[var(--primary)] outline-none focus:shadow-[0_0_5px_var(--primary)] 
                                    bg-[var(--same)]/25 text-xl p-2 rounded-xl"/>
                    <button className="w-full rounded-xl bg-[var(--primary)] p-3 uppercase font-semibold hover:shadow-[0_0_5px_var(--primary)]">Create</button>
                    {message}
                </form>
            </section>
        </main>
    );
}

export default Setup;