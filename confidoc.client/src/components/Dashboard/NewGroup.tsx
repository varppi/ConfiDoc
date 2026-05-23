import { useState } from "react";
import type { JSX } from "react/jsx-runtime";
import axios from "axios";
import { getConfig, setPassword } from "../../globals";
import Message from "../Message";
import { type IApiError } from "../../globals";
import { useNavigate } from "react-router-dom";

function NewGroup() {
    const [message, setMessage] = useState<JSX.Element>();
    const navigate = useNavigate();

    async function submit(form: FormData) {
        const name: string = form.get("name")?.toString()??"untitled group";
        try {
            const resp = await axios.post("/api/group/new", {
                name,
            }, getConfig())
            setMessage(<Message color="info" text="group created" />);
            setTimeout(() => {
                navigate(`/group/${resp.data.id}`);
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

    return (
        <section className="flex justify-center mx-2">
            <form className="p-4 rounded-xl flex flex-col max-w-[800px] w-full gap-4 bg-[var(--same)]/10 backdrop-blur-[5px]
                                border-1 border-[var(--cont)]/15 mt-[5vh]" action={submit}>
                <h1 className="text-4xl uppercase font-semibold border-l-4 border-[var(--primary)] ps-2 ms-2">New Group</h1>
                <input placeholder="group name" name="name"
                    className="border-2 border-[var(--primary)] outline-none focus:shadow-[0_0_5px_var(--primary)] 
                                    bg-[var(--same)]/25 text-xl p-2 rounded-xl"/>
                <button className="w-full rounded-xl bg-[var(--primary)] p-3 uppercase font-semibold hover:shadow-[0_0_5px_var(--primary)]">Create</button>
                {message}
            </form>
        </section>
    );
}

export default NewGroup;