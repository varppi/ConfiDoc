import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState, type JSX } from "react";
import axios from "axios";
import { getConfig, getPassword, convertTicksToJs, type DocumentEntry, setPassword, type GroupEntry, getToken } from "../../globals";
import Message from "../../components/Message";
import NotFound from "../../NotFound";

function Lander() {
    const [message, setMessage]   = useState<JSX.Element   | null>(null);
    const [document, setDocument] = useState<DocumentEntry | null>(null);
    const [groups, setGroups]     = useState<GroupEntry[]  | null>(null);
    const [verify, setVerify]     = useState<number>(-1);
    const [reloadDocument, setReloadDocument]     = useState<boolean>(false);
    const [passwordRequired, setPasswordRequired] = useState<boolean>(false);
    const { id }   = useParams();
    const navigate = useNavigate();

    async function getDocument() {
        try {
            const resp = await axios.post(`/api/document/${id}`, {
                password: getPassword(id??"")
            }, getConfig());
            setPasswordRequired(false);
            return resp.data;
        } catch(error) {
            try {
                const resp = error.response;
                if (resp.status == 403) {
                    setPasswordRequired(true);
                    return;
                }
                setMessage(<Message size="30" color="danger" text="failed to load document" />)
                return null;
            } catch {
                setMessage(<Message color="danger" text="something went wrong!" />);
            }
        }
    }

    async function getGroups() {
        try {
            const resp = await axios.get(`/api/group`, getConfig());
            return resp.data;
        } catch(error) {
            try {
                setMessage(<Message color="danger" text="failed to load group data" />)
                return null;
            } catch {
                setMessage(<Message color="danger" text="something went wrong!" />);
            }
        }
    }

    async function removeUserAccess(name: string) {
        try {
            const resp = await axios.post(`/api/document/${id}/remove/user`, {
                name
            }, getConfig());
            setReloadDocument(!reloadDocument);
            return resp.data;
        } catch(error) {
            try {
                setMessage(<Message color="danger" text="failed to remove access!" />)
                return null;
            } catch {
                setMessage(<Message color="danger" text="something went wrong!" />);
            }
        }
    }

    async function removeGroupAccess(name: string) {
        try {
            const resp = await axios.post(`/api/document/${id}/remove/group`, {
                name
            }, getConfig());
            setReloadDocument(!reloadDocument);
            return resp.data;
        } catch(error) {
            try {
                setMessage(<Message color="danger" text="failed to remove access!" />)
                return null;
            } catch {
                setMessage(<Message color="danger" text="something went wrong!" />);
            }
        }
    }

    async function AddUserAccess(form: FormData) {
        const name = form.get("name");
        const level = form.get("level");
        const duration = form.get("duration");
        try {
            const resp = await axios.post(`/api/document/${id}/add/user`, {
                name,
                level,
                duration
            }, getConfig());
            setReloadDocument(!reloadDocument);
            return resp.data;
        } catch(error) {
            try {
                setMessage(<Message color="danger" text="failed to give access!" />)
                return null;
            } catch {
                setMessage(<Message color="danger" text="something went wrong!" />);
            }
        }
    }

    async function AddGroupAccess(form: FormData) {
        const name = form.get("name");
        const level = form.get("level");
        const duration = form.get("duration");
        try {
            const resp = await axios.post(`/api/document/${id}/add/group`, {
                name,
                level,
                duration
            }, getConfig());
            setReloadDocument(!reloadDocument);
            return resp.data;
        } catch(error) {
            try {
                setMessage(<Message color="danger" text="failed to give access!" />)
                return null;
            } catch {
                setMessage(<Message color="danger" text="something went wrong!" />);
            }
        }
    }

    async function deleteDocument() {
        try {
            await axios.delete(`/api/document/${id}/delete`, getConfig());
            setDocument({
                id: "<deleted>",
                name: "<deleted>",
                size: 0,
                changes: [],
                created: -5,
                lastModified: -5,
                readAccessUsers: [],
                writeAccessUsers: [],
                readAccessGroups: [],
                writeAccessGroups: [],
                owner: "<deleted>"
            });
            setMessage(<Message color="info" text="document deleted successfully" />);
        } catch {
            setMessage(<Message color="danger" text="failed to delete document" />)
        }
    }

    async function unlockDocument(form: FormData) {
        const password = form.get("password");
        if (password == null) return;
        await setPassword(id??"", password.toString());
        setReloadDocument(!reloadDocument);
    }

    function openDoc() {
        navigate(`/document/${id}/modify`);
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
        getGroups().then(groups => {
            setGroups(groups);
        })
        getDocument().then(doc => {
            setDocument(doc);
        });
    }, [reloadDocument])

    return (
        <main>
            {
                passwordRequired &&
                <section className="flex justify-center mx-2">
                    <form className="p-4 rounded-xl flex flex-col max-w-[800px] w-full gap-2 bg-[var(--same)]/5 backdrop-blur-[5px]
                                    border-1 border-[var(--cont)]/15 mt-[calc(50px+5vh)]"
                          action={unlockDocument}>
                        <h1 className="text-4xl uppercase font-semibold border-l-4 
                                        border-[var(--primary)] ps-2 ms-2 mb-4"
                        >Password required</h1>
                        <input placeholder="password" type="password" name="password"
                        className="border-2 border-[var(--primary)] outline-none focus:shadow-[0_0_5px_var(--primary)] 
                                    bg-[var(--same)]/25 text-xl p-2 rounded-xl"/>                        
                        
                        <div className="flex w-full gap-2 text-white font-bold mt-5"> 
                            <button className="w-full rounded-xl bg-[var(--primary)] p-3 uppercase w-[90%]
                                                font-semibold hover:shadow-[0_0_5px_var(--primary)]"
                            >Unlock</button>
                        {
                            verify == 0
                            ?
                            <button className="p-2 cursor-pointer hover:shadow-[0_0_5px_var(--danger)] 
                                                uppercase bg-[var(--danger)] w-full rounded-xl"
                                    onClick={() => {deleteDocument(); navigate("/dashboard/documents")}}
                                    type="button"
                            >Confirm Delete</button>
                            :
                            <button className="p-2 cursor-pointer hover:shadow-[0_0_5px_var(--danger)] 
                                                uppercase bg-[var(--danger)] w-full rounded-xl"
                                    onClick={() => setVerify(3) } type="button"
                            >Delete{verify > 0 ? ` (wait ${verify}s)` : ""}</button>

                        }
                        </div>
                        {message}
                    </form>
                </section>
            }
            {
                (document == null && message == null && !passwordRequired) &&
                <NotFound/>
            }
            {
                (document == null && message != null && !passwordRequired) &&
                <div className="flex justify-center items-center h-[calc(20vh+100px)]">
                    <div className="bg-[var(--danger)]/5 border border-[var(--danger)]/25
                                    p-4 rounded-[200px] h-fit w-fit uppercase backdrop-blur">
                        {message}
                    </div>
                </div>
            }
            {
                document && 
                <section className="flex justify-center mx-2">
                    <div className="p-4 rounded-xl flex flex-col max-w-[800px] w-full gap-2 bg-[var(--same)]/5 backdrop-blur-[5px]
                                    border-1 border-[var(--cont)]/15 mt-[1vh]">
                        <h1 className="text-4xl uppercase font-semibold border-l-4 
                                        border-[var(--primary)] ps-2 ms-2 mb-4"
                        >{document.name}</h1>
                        <div className="flex w-full gap-2">
                            <span>ID</span>
                            <p className="bg-[var(--cont)]/5 rounded  px-1">{document.id}</p>
                        </div>
                        <div className="flex w-full gap-2">
                            <span>Created</span>
                            <p className="bg-[var(--cont)]/5 rounded  px-1">{convertTicksToJs(document.created).toLocaleString()}</p>
                        </div>
                        <div className="flex w-full gap-2">
                            <span>Last Modified</span>
                            <p className="bg-[var(--cont)]/5 rounded  px-1">{convertTicksToJs(document.lastModified).toLocaleString()}</p>
                        </div>
                        <div className="flex w-full gap-2">
                            <span>Changes Made</span>
                            <p className="bg-[var(--cont)]/5 rounded  px-1">{document.changes.length}</p>
                        </div>
                        <div className="p-3 shadow-[0_0_2px_var(--contTransp)] mb-2 mt-1 rounded-2xl">
                            <h2 className="text-2xl uppercase font-semibold">Access Controls</h2>
                            <h3 className="text-xl uppercase font-semibold mt-3 mb-1">User Access</h3>
                            <div className="max-h-[300px] overflow-y-scroll w-full backdrop-blur border 
                                        border-[var(--cont)]/10 p-2 gap-2 flex flex-col rounded-xl mb-2 mt-1">
                                <div className="flex text-xl border-b border-[var(--cont)]/25 pb-2 pt-3
                                                flex items-center gap-2 pe-3">
                                    <span>{document.owner}</span>
                                    <span className="text-[10px]">(OWNER)</span>
                                    <div className="flex-grow"/>
                                </div>
                                {
                                    document.writeAccessUsers.map(user => 
                                        <div className="flex text-xl border-b border-[var(--cont)]/25 pb-2 pt-3
                                                        flex items-center gap-2 pe-3">
                                            <span>{user}</span>
                                            <span className="text-[10px]">(READ & WRITE)</span>
                                            <span className="text-[10px]">
                                                {Math.floor((document.grants.find(g => g.receiver == user) ?? { minutesLeft: 0 }).minutesLeft / 60)}h {(document.grants.find(g => g.receiver == user) ?? { minutesLeft: 0 }).minutesLeft}m left
                                            </span>
                                            <div className="flex-grow"/>
                                            <button className="p-2 cursor-pointer hover:shadow-[0_0_5px_var(--danger)] 
                                                            uppercase bg-[var(--danger)] w-fit rounded-3xl text-white
                                                            text-[15px] font-bold px-5"
                                                    onClick={()=>removeUserAccess(user)}
                                            >Remove</button>
                                        </div>
                                    )
                                }

                                {
                                    document.readAccessUsers.map(user => 
                                        (!document.writeAccessUsers.includes(user)) &&
                                        <div className="flex text-xl border-b border-[var(--cont)]/25 pb-2 pt-3
                                                        flex items-center gap-2 pe-3">
                                            <span>{user}</span>
                                            <span className="text-[10px]">(READ)</span>
                                            <span className="text-[10px]">
                                                {Math.floor((document.grants.find(g => g.receiver == user) ?? { minutesLeft: 0 }).minutesLeft / 60)}h {(document.grants.find(g => g.receiver == user) ?? { minutesLeft: 0 }).minutesLeft}m left
                                            </span>
                                            <div className="flex-grow"/>
                                            <button className="p-2 cursor-pointer hover:shadow-[0_0_5px_var(--danger)] 
                                                            uppercase bg-[var(--danger)] w-fit rounded-3xl text-white
                                                            text-[15px] font-bold px-5"
                                                    onClick={()=>removeUserAccess(user)}
                                            >Remove</button>
                                        </div>
                                    )
                                }
                            </div>
                            <form className="flex max-md:flex-col gap-2 rounded-2xl"
                                action={AddUserAccess}>
                                <input 
                                    placeholder="username" 
                                    name="name"
                                    className="border-1 border-[var(--primary)] outline-none 
                                            focus:shadow-[0_0_5px_var(--primary)] bg-[var(--same)]/55 text-xl p-2 
                                            rounded-xl w-full"/>
                                <select className="border-1 border-[var(--primary)] outline-none 
                                            focus:shadow-[0_0_5px_var(--primary)] bg-[var(--same)]/55 text-xl p-1
                                            rounded-xl min-md:w-fit font-semibold"
                                        name="level">
                                        <option value={"read"}>READ</option>
                                        <option value={"write"}>READ & WRITE</option>
                                </select>
                                <select className="border-1 border-[var(--primary)] outline-none
                                        focus:shadow-[0_0_5px_var(--primary)] bg-[var(--same)]/55 text-xl p-1
                                        rounded-xl min-md:w-fit font-semibold"
                                    name="duration">
                                        <option value={"1"}>1h</option>
                                        <option value={"8"}>8h</option>
                                        <option value={"24"}>1d</option>
                                        <option value={24 * 7}>7d</option>
                                        <option value={24 * 30}>30d</option>
                                        <option value={24 * 180}>180 days</option>
                                        <option value={24 * 360}>year</option>
                                </select>
                                <button className="rounded-xl bg-[var(--primary)] p-3 uppercase min-w-[calc(150px+0.25vw)]
                                                font-semibold hover:shadow-[0_0_5px_var(--primary)]">Add</button>
                                
                            </form>


                            <h3 className="text-xl uppercase font-semibold mt-3 mb-1">Group Access</h3>
                            <div className="max-h-[300px] overflow-y-scroll w-full backdrop-blur border 
                                        border-[var(--cont)]/10 p-2 gap-2 flex flex-col rounded-xl mb-2 mt-1">
                                {
                                    document.writeAccessGroups.map(group => 
                                        <div className="flex text-xl border-b border-[var(--cont)]/25 pb-2 pt-3
                                                        flex items-center gap-2 pe-3">
                                            <span>{groups&&groups.find(g => g.id == group)?.displayName}</span>
                                            <span className="text-[10px]">(READ & WRITE)</span>
                                            <span className="text-[10px]">
                                                {Math.floor((document.grants.find(g => g.receiver == group) ?? { minutesLeft: 0 }).minutesLeft / 60)}h {(document.grants.find(g => g.receiver == group) ?? { minutesLeft: 0 }).minutesLeft}m left
                                            </span>
                                            <div className="flex-grow"/>
                                            <button className="p-2 cursor-pointer hover:shadow-[0_0_5px_var(--danger)] 
                                                            uppercase bg-[var(--danger)] w-fit rounded-3xl text-white
                                                            text-[15px] font-bold px-5"
                                                    onClick={()=>removeGroupAccess(group)}
                                            >Remove</button>
                                        </div>
                                    )
                                }

                                {
                                    document.readAccessGroups.map(group =>
                                        (!document.writeAccessGroups.includes(group)) &&
                                        <div className="flex text-xl border-b border-[var(--cont)]/25 pb-2 pt-3
                                                        flex items-center gap-2 pe-3">
                                            <span>{groups&&groups.find(g => g.id == group)?.displayName}</span>
                                            <span className="text-[10px]">(READ)</span>
                                            <span className="text-[10px]">
                                                {Math.floor((document.grants.find(g => g.receiver == group) ?? { minutesLeft: 0 }).minutesLeft / 60)}h {(document.grants.find(g => g.receiver == group) ?? { minutesLeft: 0 }).minutesLeft}m left
                                            </span>
                                            <div className="flex-grow"/>
                                            <button className="p-2 cursor-pointer hover:shadow-[0_0_5px_var(--danger)] 
                                                            uppercase bg-[var(--danger)] w-fit rounded-3xl text-white
                                                            text-[15px] font-bold px-5"
                                                    onClick={()=>removeGroupAccess(group)}
                                            >Remove</button>
                                        </div>
                                    )
                                }
                            </div>
                            <form className="flex max-md:flex-col gap-2 rounded-2xl"
                                action={AddGroupAccess}>
                                <select className="border-1 border-[var(--primary)] outline-none 
                                            focus:shadow-[0_0_5px_var(--primary)] bg-[var(--same)]/55 text-xl p-2 
                                            rounded-xl w-full"
                                        name="name">
                                        {
                                            groups&&
                                            groups.map(group => <option value={group.id}>{group.displayName}</option>)
                                        }
                                </select>
                                <select className="border-1 border-[var(--primary)] outline-none 
                                            focus:shadow-[0_0_5px_var(--primary)] bg-[var(--same)]/55 text-xl p-1
                                            rounded-xl min-md:w-fit font-semibold"
                                        name="level">
                                        <option value={"read"}>READ</option>
                                        <option value={"write"}>READ & WRITE</option>
                                </select>
                                <select className="border-1 border-[var(--primary)] outline-none
                                    focus:shadow-[0_0_5px_var(--primary)] bg-[var(--same)]/55 text-xl p-1
                                    rounded-xl min-md:w-fit font-semibold"
                                    name="duration">
                                    <option value={"1"}>1h</option>
                                    <option value={"8"}>8h</option>
                                    <option value={"24"}>1d</option>
                                    <option value={24 * 7}>7d</option>
                                    <option value={24 * 30}>30d</option>
                                    <option value={24 * 180}>180 days</option>
                                    <option value={24 * 360}>year</option>
                                </select>
                                <button className="rounded-xl bg-[var(--primary)] p-3 uppercase min-w-[calc(150px+0.25vw)]
                                                font-semibold hover:shadow-[0_0_5px_var(--primary)]">Add</button>
                            </form>
                        </div>
                        <div className="flex w-full max-w-[400px] gap-2 text-white font-bold mt-5"> 
                            {
                                verify == 0
                                ?
                                <button className="p-2 cursor-pointer hover:shadow-[0_0_5px_var(--danger)] 
                                                    uppercase bg-[var(--danger)] w-full rounded-xl"
                                        onClick={() => deleteDocument()}
                                >Confirm Delete</button>
                                :
                                <button className="p-2 cursor-pointer hover:shadow-[0_0_5px_var(--danger)] 
                                                    uppercase bg-[var(--danger)] w-full rounded-xl"
                                        onClick={() => setVerify(3) }
                                >Delete{verify > 0 ? ` (wait ${verify}s)` : ""}</button>

                            }
                            {
                                    document.level == 1 ?
                                        <form className="w-full" method="POST" action={`/api/document/${document.id}/pdf`}>
                                            <input hidden name="token" value={getToken()??""}></input>
                                            <input hidden name="password" value=""></input>
                                            <button className="text-center p-2 cursor-pointer hover:shadow-[0_0_5px_var(--primary)]
                                                        uppercase bg-[var(--primary)] w-full rounded-xl"
                                                    type="submit"
                                            >Download</button>
                                        </form>
                                : 
                                <button className="p-2 cursor-pointer hover:shadow-[0_0_5px_var(--primary)]
                                                    uppercase bg-[var(--primary)] w-full rounded-xl"
                                        onClick={() => openDoc()}
                                >Open/Modify</button>
                            }
                        </div>
                        {message}
                    </div>
                </section>
            }
        </main>
    );
}

export default Lander;