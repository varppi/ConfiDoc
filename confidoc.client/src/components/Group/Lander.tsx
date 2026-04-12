import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState, type JSX } from "react";
import axios from "axios";
import { getConfig, getPassword, convertTicksToJs, type GroupEntry, type IApiError } from "../../globals";
import Message from "../../components/Message";
import NotFound from "../../NotFound";
import { Diamond01, Tool01, Tool02 } from "@untitledui/icons";

function Lander() {
    const [message, setMessage]   = useState<JSX.Element|null>(null);
    const [group, setGroup] = useState<GroupEntry|null>(null);
    const [verify, setVerify]     = useState<number>(-1);
    const [reloadGroup, setReloadGroup]     = useState<boolean>(false);
    const { id }   = useParams();
    const navigate = useNavigate();

    async function getGroup() {
        try {
            const resp = await axios.get(`/api/group/${id}`, getConfig());
            return resp.data;
        } catch(error) {
            try {
                const resp = error.response;
                if (resp.status == 403) {
                    return;
                }
                setMessage(<Message size="30" color="danger" text="failed to load group" />)
                return null;
            } catch {
                setMessage(<Message color="danger" text="something went wrong!" />);
            }
        }
    }


    async function submit(form: FormData) {
        const userName: string = form.get("username")?.toString()??"somebody";
        try {
            await axios.post(`/api/group/${id}`, {
                userName,
            }, getConfig())
            setMessage(<Message color="info" text="user added!" />);
            setReloadGroup(!reloadGroup);
        } catch(error) {
            try {
                const body: IApiError = error.response.data;
                if (error.response.status == 404) {
                    setMessage(<Message color="danger" text="user does not exist or permission denied!"/>)
                    return;
                }
                let message = "";
                for (const errorPair of Object.entries(body.errors))
                    message += `${errorPair[1].join("\n")}\n`
                setMessage(<Message color="danger" text={message} />);
            } catch {
                setMessage(<Message color="danger" text="something went wrong!" />);
            }
        }
    }

    async function removeUser(userName: string) 
    {
        try {
            await axios.post(`/api/group/${id}/remove`, {
                userName,
            }, getConfig())
            setMessage(<Message color="info" text="user removed successfully!" />);
            setReloadGroup(!reloadGroup);
        } catch(error) {
            try {
                const body: IApiError = error.response.data;
                let message = "";
                for (const errorPair of Object.entries(body.errors))
                    message += `${errorPair[1].join("\n")}\n`
                setMessage(<Message color="danger" text={message} />);
            } catch {
                setMessage(<Message color="danger" text="something went wrong! you might not have permissions to do this." />);
            }
        }
    }

    async function deleteGroup() {
        try {
            await axios.delete(`/api/group/${id}/delete`, getConfig());
            setGroup({
                id: "<deleted>",
                displayName: "<deleted>",
                members: [],
                owner: "",
            });
            setMessage(<Message color="info" text="group deleted successfully" />);
        } catch {
            setMessage(<Message color="danger" text="failed to delete group" />)
        }
    }

    useEffect(() => {
        if (verify > 0) 
            setTimeout(() => {
                setVerify(verify - 1);
            }, 1000)
        if (verify == 0)
            setTimeout(() => {
                setVerify(-1);
            }, 5000)
        
    }, [verify]);

    useEffect(() => {
        getGroup().then(group => {
            setGroup(group);
        })
    }, [reloadGroup])

    return (
        <main>
            {
                (group == null && message == null) &&
                <NotFound/>
            }
            {
                (group == null && message != null) &&
                <div className="flex justify-center items-center h-[calc(20vh+100px)]">
                    <div className="bg-[var(--cont)]/5 border border-[var(--cont)]/10
                                    p-4 rounded-[200px] h-fit w-fit uppercase backdrop-blur">
                        {message}
                    </div>
                </div>
            }
            {
                group && 
                <section className="flex justify-center mx-2">
                    <div className="p-4 rounded-xl flex flex-col max-w-[800px] w-full gap-2 bg-[var(--same)]/5 backdrop-blur-[5px]
                                    border-1 border-[var(--cont)]/15 mt-[calc(50px+5vh)]">
                        <h1 className="text-4xl uppercase font-semibold border-l-4 
                                        border-[var(--primary)] ps-2 ms-2 mb-4"
                        >{group.displayName}</h1>
                        <h2 className="text-xl uppercase font-semibold">Members</h2>
                        <div className="max-h-[300px] overflow-y-scroll w-full backdrop-blur border 
                                        border-[var(--cont)]/10 p-2 gap-2 flex flex-col rounded-xl">

                            {
                                group.members.map(user => 
                                    <div className="flex text-xl border-b border-[var(--cont)]/25 pb-2 pt-3
                                                    flex items-center gap-2 pe-3">
                                        <span>{user}</span>
                                        {group.owner == user && <span className="text-[10px]">(ADMIN)</span>}
                                        <div className="flex-grow"/>
                                        <button className="p-2 cursor-pointer hover:shadow-[0_0_5px_var(--danger)] 
                                                           uppercase bg-[var(--danger)] w-fit rounded-3xl text-white
                                                           text-[15px] font-bold px-5"
                                                onClick={()=>removeUser(user)}
                                        >Remove</button>
                                    </div>
                                )
                            }
                        </div>
                        <h2 className="text-xl uppercase font-semibold mt-3">Add Member</h2>
                        <form className="flex gap-3 shadow-[0_0_2px_var(--contTransp)] rounded-2xl p-3"
                              action={submit}>
                            <input 
                                placeholder="username" 
                                name="username"
                                className="border-2 border-[var(--primary)] outline-none 
                                           focus:shadow-[0_0_5px_var(--primary)] bg-[var(--same)]/25 text-xl p-2 
                                           rounded-xl w-full"/>
                            <button className="rounded-xl bg-[var(--primary)] p-3 uppercase min-w-[calc(200px+0.5vw)]
                                               font-semibold hover:shadow-[0_0_5px_var(--primary)]">Add</button>
                        </form>
                        <div className="flex w-full gap-2 text-white font-bold mt-5 justify-start"> 
                            <div className="max-w-[200px] w-full">
                                {
                                    verify == 0
                                    ?
                                    <button className="p-2 cursor-pointer hover:shadow-[0_0_5px_var(--danger)] 
                                                        uppercase bg-[var(--danger)] w-full rounded-xl"
                                            onClick={() => deleteGroup()}
                                    >Confirm Delete</button>
                                    :
                                    <button className="p-2 cursor-pointer hover:shadow-[0_0_5px_var(--danger)] 
                                                        uppercase bg-[var(--danger)] w-full rounded-xl"
                                            onClick={() => setVerify(3) }
                                    >Delete{verify > 0 ? ` (wait ${verify}s)` : ""}</button>
                                }
                            </div>
                        </div>
                        {message}
                    </div>
                </section>
            }
        </main>
    );
}

export default Lander;