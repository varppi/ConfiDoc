import { useEffect, useState, type JSX } from "react";
import { convertTicksToJs, getConfig, type DocumentEntry, type GroupEntry } from "../../globals";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import { Users01, UsersPlus } from "@untitledui/icons";

export default function Groups() {
    const [message, setMessage] = useState<JSX.Element>(<></>);
    const [groups, setGroups] = useState<GroupEntry[]>([]);
    const navigate = useNavigate();

    async function getGroups() {
        try {
            const resp = await axios.get("/api/group", getConfig());
            return resp.data;
        } catch {
            setMessage(<Message color="danger" text="failed to load documents"/>)
            return null;
        }
    }

    useEffect(() => {
        getGroups().then(groups => {
            if (!Array.isArray(groups)) return;
            setGroups(groups);
        })
    }, []);

    return <>
            <div className="flex w-full justify-center">
                <div className="w-full max-w-[700px]">
                    <button className="flex-1 mt-[5vh] mb-4 text-[var(--primary)] text-2xl fw-bold font-bold w-fit 
                                    whitespace-nowrap cursor-pointer flex items-center gap-1 uppercase hover:text-[var(--cont)]"
                        onClick={() => { navigate("/dashboard/groups/new") }}><UsersPlus size="30px" /> New Group</button>

                    <div className="w-full bg-[var(--same)]/10 backdrop-blur-[5px]
                                    border-1 border-[var(--cont)]/15 rounded-2xl p-3">
                        <table className="w-full font-mono">
                            <tbody>
                                <tr>
                                    <th className="p-1 text-start text-[calc(15px+0.25vw)]">Name</th>
                                </tr>
                                {
                                    groups&&groups.map(group => 
                                        <tr>
                                            <td className="p-1">
                                                <button
                                                    className="bg-[var(--primary)] w-full max-w-[200px] rounded-4xl p-1 cursor-pointer
                                                            hover:shadow-[0_0_5px_var(--primary)] text-xl"
                                                    onClick={() => navigate(`/group/${group.id}`)}>
                                                    {group.displayName}
                                                </button>
                                            </td>
                                        </tr>
                                    )
                                }
                            </tbody>
                        </table>
                        {message}
                    </div>
                </div>
            </div>
    </>
}