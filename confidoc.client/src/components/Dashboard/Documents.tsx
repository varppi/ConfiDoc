import axios from "axios";
import { useEffect, useMemo, useState, type JSX } from "react";
import { getConfig, convertTicksToJs } from "../../globals";
import Message from "../Message";
import { useNavigate } from "react-router-dom";
import {type DocumentEntry} from "../../globals";
import { FilePlus02 } from "@untitledui/icons";
function Documents() {
    const [message, setMessage] = useState<JSX.Element>(<></>);
    const [documents, setDocuments] = useState<DocumentEntry[]>([]);
    const navigate = useNavigate();

    async function getDocuments() {
        try {
            const resp = await axios.get("/api/document", getConfig());
            return resp.data;
        } catch {
            setMessage(<Message color="danger" text="failed to load documents"/>)
            return null;
        }
    }

    useEffect(() => {
        getDocuments().then(docs => {
            setDocuments(docs);
        })
    }, []);
  

    return (
        <div className="flex w-full justify-center">
            <div className="w-full max-w-[1250px]">
                <button className="flex-1 mt-[5vh] mb-4 text-[var(--primary)] text-2xl fw-bold font-bold w-fit 
                                   whitespace-nowrap cursor-pointer flex items-center gap-1 uppercase hover:text-[var(--cont)]"
                    onClick={() => { navigate("/dashboard/documents/new") }}><FilePlus02 size="30px" /> New Document</button>

                <div className="w-full bg-[var(--same)]/10 backdrop-blur-[5px]
                                border-1 border-[var(--cont)]/15 rounded-2xl p-3">
                    <table className="w-full font-mono">
                        <tbody>
                            <tr>
                                <th className="p-1 text-start text-[calc(15px+0.25vw)]">Name</th>
                                <th className="p-1 text-start text-[calc(15px+0.25vw)]">Created</th>
                                <th className="p-1 text-start text-[calc(15px+0.25vw)]">Last Modified</th>
                                <th className="p-1 text-start text-[calc(15px+0.25vw)]">Changes Made</th>
                            </tr>
                            {
                                documents&&documents.sort((x,y) => y.lastModified-x.lastModified).map(doc =>
                                    <tr key={doc.created}>
                                        <td className="p-1">
                                            <button
                                                className="bg-[var(--primary)] w-full rounded-4xl p-1 cursor-pointer
                                                        hover:shadow-[0_0_5px_var(--primary)] text-xl "
                                                onClick={() => navigate(`/document/${doc.id}`)}>
                                                {doc.name}
                                            </button>
                                        </td>
                                        <td className="p-1">{convertTicksToJs(doc.created).toLocaleString()}</td>
                                        <td className="p-1">{convertTicksToJs(doc.lastModified).toLocaleString()}</td>
                                        <td className="p-1">{doc.changes.length}</td>
                                    </tr>
                                )
                            }
                        </tbody>
                    </table>
                    {message}
                </div>
            </div>

        </div>
    );
}

export default Documents;