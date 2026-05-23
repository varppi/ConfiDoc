import {BarChart} from "@mui/x-charts/BarChart";
import axios from "axios";
import { useEffect, useMemo, useState, type JSX } from "react";
import { getConfig, convertTicksToJs } from "../../globals";
import Message from "../Message";
import { useNavigate } from "react-router-dom";
import {type DocumentEntry} from "../../globals";
import { axisClasses } from '@mui/x-charts/ChartsAxis';

function Main() {
  const [message, setMessage] = useState<JSX.Element>(<></>);
  const [documents, setDocuments] = useState<DocumentEntry[]>([]);

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

  let steps = 50;
  const timeSections: string[] = [];
  const data: number[] = [...new Array(steps)].map(() => 0);
  if (documents.length > 0) {
    const sortedCreated = documents.sort((x,y) => x.created - y.created);
    let sortedModified = documents.sort((x,y) => x.lastModified - y.lastModified);
    sortedModified = sortedModified.reverse();
    const start = sortedCreated[0].created;
    const stop = sortedModified[0].lastModified;
    steps = Math.floor((stop-start)/800000000000);
    if (steps > 20) 
      steps = 20
    const step = (stop-start)/steps;
    for (let i = 0; i <= steps; i++) {
      timeSections.push(`${new Date(convertTicksToJs(start+i*step)).toLocaleDateString()} (${i})`)
    }
    console.log("start")
    for (const doc of documents) {
      for (const change of doc.changes) {
        const timestamp = change.timestamp-start;
        const nStep = Math.ceil(timestamp/step)
        console.log(doc.name, timestamp, nStep)
        data[nStep] = data[nStep]+1;
      }
    }
  }
  

  return (
    <main>
      <h1 className="text-4xl text-[var(--cont)] font-bold max-md:mt-5 mb-5
                      uppercase underline decoration-[var(--primary)] decoration-[3px]"
          >Dashboard</h1>

      <p className="mt-5 text-xl uppercase">Activity graph</p>
      <div className="w-full h-full bg-[var(--primary)]/5 rounded-2xl max-w-[1000px]">
        <BarChart
          sx={{
            [`.${axisClasses.root}`]: {
              [`.${axisClasses.tick}, .${axisClasses.line}`]: {
                stroke: 'var(--primary)',
                strokeWidth: 2,
              },
              [`.${axisClasses.tickLabel}`]: {
                fill: 'var(--primary)',
              },
            },
          }}
          xAxis={
            [
              {
                id: "bar chart", 
                data: timeSections, 
                categoryGapRatio: 0,
                barGapRatio: 0,
              }
            ]
          }
          series={[
            {
              data: data,
              color: "var(--primary)"
            },
          ]}
          height={300}
        />
       </div>
       { message }
    </main>
  );
}

export default Main;