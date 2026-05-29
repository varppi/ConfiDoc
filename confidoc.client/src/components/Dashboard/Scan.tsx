import axios from "axios";
import { useEffect, useMemo, useState, type JSX } from "react";
import { getConfig, convertTicksToJs, getConfigMultipart, type Event, generateUniqueColor } from "../../globals";
import Message from "../Message";
import { useNavigate } from "react-router-dom";
import { type DocumentEntry } from "../../globals";
import { FilePlus02 } from "@untitledui/icons";
function Scan() {
    const [message, setMessage]     = useState<JSX.Element>(<></>);
    const [datamarks, setDatamarks] = useState<string[] | null>(null);
    const [events, setEvents]       = useState<{ [key: string]: Event } | null>(null);
    const [documents, setDocuments] = useState<DocumentEntry[]>([]);
    const navigate = useNavigate();

    async function submit(form: FormData) {
        try {
            const resp = await axios.post("/api/scan", form, getConfigMultipart());
            setDatamarks(resp.data.strings);
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
        return;
    }

    async function getEvent(id: string) {
        return (await axios.get(`/api/event/${id}`, getConfig())).data;
    }

    useEffect(() => {
        async function fetchEvents() {
            const eventsTemp: { [key: string]: Event } = {};
            for (const id of (datamarks??[])) {
                const data = await getEvent(id);
                eventsTemp[id] = data;
            }
            setEvents(eventsTemp);
        }
        fetchEvents();
    }, [datamarks])


    return (
        <div className="flex w-full justify-center">
            <div className="w-full max-w-[1250px]">
                <div className="w-full flex justify-center mt-[7vh]">
                    <form className="w-fit bg-[var(--same)]/10 backdrop-blur-[5px]
                                border-1 border-[var(--cont)]/15 rounded-2xl p-3 flex flex-col gap-3"
                          action={submit}>
                        <input name="image" type="file" className="border-l-2 w-fit border-[var(--primary)] ps-2 hover:cursor-pointer mt-3 mb-2 text-xl"></input>    
                        <sub>Make sure the photo is high resolution enough to capture every vertical line accurately.</sub>
                        <button className="bg-[var(--primary)] w-full rounded-4xl p-1 cursor-pointer
                                           uppercase p-2 mt-5 hover:shadow-[0_0_5px_var(--primary)] text-xl "
                        >scan it</button>
                        {
                            datamarks == null ? <></>
                            :
                                datamarks?.length == 0
                                ? <Message color="danger" text="No datamarks found in the picture" />
                                :
                                <div className="flex flex-col ps-2 gap-2">
                                        {datamarks.map(str =>
                                            <div className="flex flex-col">
                                                <b className="text-xl">{str}</b>
                                                {
                                                    (events == null || !(str in events)) ? <p>Event does not exist</p>
                                                        : (() => {
                                                            const diff = Math.floor(
                                                                (Date.now() - convertTicksToJs(events[str].timestamp).getTime()) / 60000
                                                            );

                                                            return <div style={{ color: `rgb(${generateUniqueColor(events[str].action.split("")[0])})` }}>
                                                                <p>Action: <b>{events[str].action.split(":")[0]}</b></p>
                                                                <p>User: <b>{events[str].user}</b></p>
                                                                <p>IPv4: <b>{events[str].ip}</b></p>
                                                                <p>Browser: <b>{events[str].userAgent}</b></p>
                                                                <p>Timestamp: <b>{convertTicksToJs(events[str].timestamp).toLocaleString()} ({Math.floor(diff / 60)}h {Math.floor(diff) - (Math.floor(diff / 60) * 60)}m ago)</b></p >
                                                            </div>
                                                        })()
                                                }
                                            </div>)}
                                </div>
                        }
                        {message}
                    </form>
                </div>
            </div>

        </div>
    );
}

export default Scan;