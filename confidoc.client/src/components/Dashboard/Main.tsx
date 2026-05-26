import {BarChart} from "@mui/x-charts/BarChart";
import axios from "axios";
import { useEffect, useMemo, useState, type JSX } from "react";
import { getConfig, convertTicksToJs, getTheme } from "../../globals";
import Message from "../Message";
import { useNavigate } from "react-router-dom";
import {type DocumentEntry} from "../../globals";
import { axisClasses } from '@mui/x-charts/ChartsAxis';

function Main() {
    const [message, setMessage] = useState<JSX.Element>(<></>);
    const [documents, setDocuments] = useState<DocumentEntry[]>([]);
    const [showActivity, setShowActivity] = useState<string[]>([]);
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

  //let steps = 50;
  //const timeSections: string[] = [];
  //const data: number[] = [...new Array(steps)].map(() => 0);
  //if (documents.length > 0) {
  //  const sortedCreated = documents.sort((x,y) => x.created - y.created);
  //  let sortedModified = documents.sort((x,y) => x.lastModified - y.lastModified);
  //  sortedModified = sortedModified.reverse();
  //  const start = sortedCreated[0].created;
  //  const stop = sortedModified[0].lastModified;
  //  steps = Math.floor((stop-start)/800000000000);
  //  if (steps > 20)
  //    steps = 20
  //  const step = (stop-start)/steps;
  //  for (let i = 0; i <= steps; i++) {
  //    timeSections.push(`${new Date(convertTicksToJs(start+i*step)).toLocaleDateString()} (${i})`)
  //  }
  //  console.log("start")
  //  for (const doc of documents) {
  //    for (const change of doc.changes) {
  //      const timestamp = change.timestamp-start;
  //      const nStep = Math.ceil(timestamp/step)
  //      console.log(doc.name, timestamp, nStep)
  //      data[nStep] = data[nStep]+1;
  //    }
  //  }
    //}
    function generateUniqueColor(string: string) {
        let hash = 111111111;
        let char;
        if (string.length == 0) return hash;

        for (let i = 0; i < string.length; i++) {
            char = string.charCodeAt(i);
            hash ^= Math.abs(char*hash);
        }

        const hashStr = `${Math.abs(hash)}`;
        const min = (getTheme() == "light" ? 0   : 50);
        const max = (getTheme() == "light" ? 175 : 255);
        const final = `${(min + (parseInt(hashStr.substring(0, 3))) % (max - min))},${(min + (parseInt(hashStr.substring(3, 6))) % (max - min))},${(min + (parseInt(hashStr.substring(6, 9))) % (max - min))}`;
        return final;
    }

    documents.sort((b,a) => a.lastModified - b.lastModified)

    return (
    <main className="flex w-full justify-center mt-[5vh]">
        <div className="w-full max-w-[1250px]">
            <h2 className="text-4xl text-[var(--cont)] font-bold max-md:mt-5 mb-5
                    uppercase underline decoration-[var(--primary)] decoration-[3px]">Latest activity</h2>
                <div className="flex flex-col gap-3 p-2 w-full bg-[var(--same)]/10 backdrop-blur-[5px]
                                border-1 border-[var(--cont)]/15 rounded-2xl p-3">
                <p className="italic text-sm">click document to show activity</p>
                {documents.map(doc =>
                    <>
                        <div className="grid min-xl:grid-cols-3  w-full max-w-[700px] flex items-center ">
                            <button className="min-xl:col-1 w-fit text-nowrap text-xl text-[var(--cont)] font-bold hover:cursor-pointer underline"
                                onClick={() => { setShowActivity(showActivity.includes(doc.id) ? showActivity.filter(d => d != doc.id) : showActivity.concat(doc.id)); } }>{doc.name}</button>
                            <span className="min-xl:col-2 w-fit text-nowrap text-[var(--cont)]">Last modified {convertTicksToJs(doc.lastModified).toLocaleString()}</span>
                            <span className="min-xl:col-4 w-fit text-nowrap text-[var(--cont)]">Modified by {doc.changes.length > 0 ? doc.changes[doc.changes.length - 1]["owner"] : doc.owner}</span>
                        </div>
                        <code className="max-h-[600px] flex-col flex gap-3 overflow-scroll text-sm">
                            {doc.events.map((event, i) => {
                                if (!showActivity.includes(event.action.split(":")[1])) return;
                                if (i >= 50) return;
                                const diff = Math.floor(
                                    (Date.now() - convertTicksToJs(event.timestamp).getTime()) / 60000
                                );
                                return <div style={{ color: `rgb(${generateUniqueColor(event.action.split("")[0])})`}}>
                                    <p>Action: <b>{event.action.split(":")[0]}</b></p>
                                    <p>User: <b>{event.user}</b></p>
                                    <p>IPv4: <b>{event.ip}</b></p>
                                    <p>Browser: <b>{event.userAgent}</b></p>
                                    <p>Timestamp: <b>{convertTicksToJs(event.timestamp).toLocaleString()} ({Math.floor(diff / 60)}h {Math.floor(diff) - (Math.floor(diff / 60) * 60)}m ago)</b></p >
                                </div>
                            })}
                        </code>
                    </>
                )}
            </div>
        </div>
        
        {/*<p className="mt-5 text-xl uppercase">Activity graph</p>*/}
        {/*<div className="w-full h-full bg-[var(--primary)]/5 rounded-2xl max-w-[1000px]">*/}
        {/*  <BarChart*/}
        {/*    sx={{*/}
        {/*      [`.${axisClasses.root}`]: {*/}
        {/*        [`.${axisClasses.tick}, .${axisClasses.line}`]: {*/}
        {/*          stroke: 'var(--primary)',*/}
        {/*          strokeWidth: 2,*/}
        {/*        },*/}
        {/*        [`.${axisClasses.tickLabel}`]: {*/}
        {/*          fill: 'var(--primary)',*/}
        {/*        },*/}
        {/*      },*/}
        {/*    }}*/}
        {/*    xAxis={*/}
        {/*      [*/}
        {/*        {*/}
        {/*          id: "bar chart", */}
        {/*          data: timeSections, */}
        {/*          categoryGapRatio: 0,*/}
        {/*          barGapRatio: 0,*/}
        {/*        }*/}
        {/*      ]*/}
        {/*    }*/}
        {/*    series={[*/}
        {/*      {*/}
        {/*        data: data,*/}
        {/*        color: "var(--primary)"*/}
        {/*      },*/}
        {/*    ]}*/}
        {/*    height={300}*/}
        {/*  />*/}
        {/* </div>*/}
        { message }
    </main>
    );
}

export default Main;