import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState, useRef, type JSX } from "react";
import axios from "axios";
import { getConfig, convertTicksToJs, type DocumentEntry, type IApiError, getPassword } from "../../globals";
import Message from "../../components/Message";
import '@mdxeditor/editor/style.css';
import {
    MDXEditor, headingsPlugin, listsPlugin,
    tablePlugin, imagePlugin, codeBlockPlugin, linkPlugin,
    UndoRedo, BoldItalicUnderlineToggles, toolbarPlugin,
    InsertImage, InsertTable, CreateLink, CodeToggle,
    BlockTypeSelect, quotePlugin,
    type MDXEditorMethods
} from '@mdxeditor/editor'
import { AlertCircle, ArrowBlockLeft, Check, CheckCircle, CheckCircleBroken, CheckVerified01, CheckVerified02, CheckVerified03 } from "@untitledui/icons";

function Editor() {
    const [message, setMessage]     = useState<JSX.Element>(<></>);
    const [document, setDocument]   = useState<DocumentEntry | null>(null);
    const [verify, setVerify]       = useState<number>(-1);
    const [data, setData]           = useState<string>("");
    const [updateDoc, setUpdateDoc] = useState<boolean>(false);
    const editorRef = useRef<MDXEditorMethods>(null);
    const { id }   = useParams();
    const navigate = useNavigate();

    async function save() {
        const data = editorRef.current?.getMarkdown();
        try {
            await axios.post(`/api/document/${id}/modify`, {
                data,
                password: getPassword(id??"")
            }, getConfig());
            setMessage(<Message color="info" text="saved successfully" />)
            setUpdateDoc(!updateDoc);
            setTimeout(() => {
                setMessage(<></>)
            }, 2000)
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

    async function getDocument() {
        try {
            const resp = await axios.post(`/api/document/${id}`,
                {
                    password: getPassword(id??"")
                }, getConfig());
            setData(resp.data.data);
            return resp.data;
        } catch(error) {
            if (error.response.status == 403)
                navigate(`/document/${id}`);
            setMessage(<Message size="30" color="danger" text="failed to load document" />)
            return null;
        }
    }

    useEffect(() => {
        getDocument().then(doc => {
            setDocument(doc);
        })
    }, [updateDoc])
 
    return (
        <main className="mt-5 mb-[-37vh]">
            {
                (message && document == null) &&
                <div className="flex justify-center items-center h-[calc(20vh+100px)]">
                    <div className="bg-[var(--danger)]/5 border border-[var(--danger)]/25
                                    p-4 rounded-[200px] h-fit w-fit uppercase backdrop-blur">
                        {message}
                    </div>
                </div>
            }
            {
                document &&
                <>
                    <section className="flex justify-center mx-2 max-xl:flex-col gap-4">
                        <div className="p-4 rounded-xl flex flex-col max-w-[1500px] w-full gap-2 bg-[var(--same)]/10 backdrop-blur-[5px]
                                    border-1 border-[var(--cont)]/5 prose prose-slate trasition-[0] justify-between">
                            <MDXEditor ref={editorRef} spellCheck={false} className="editor" markdown={data} plugins={[
                                headingsPlugin(),
                                listsPlugin(),
                                imagePlugin(),
                                tablePlugin(),
                                codeBlockPlugin(),
                                linkPlugin(),
                                quotePlugin(),
                                toolbarPlugin({
                                    toolbarClassName: 'toolbar',
                                    toolbarContents: () => (
                                        <>
                                            <UndoRedo />
                                            <BoldItalicUnderlineToggles />
                                            <InsertImage/>
                                            <InsertTable/>
                                            <CreateLink/>
                                            <CodeToggle/>
                                            <BlockTypeSelect/>
                                        </>
                                    )
                                })

                            ]} />
                            <button className="w-full rounded-xl bg-[var(--primary)] p-1
                                                max-w-[100px] uppercase font-semibold
                                                hover:shadow-[0_0_5px_var(--primary)] text-white"
                                    onClick={()=>save()}
                            >Save</button>
                            {message }
                        </div>
                        <section className="w-full min-xl:max-w-[450px] w-full bg-[var(--same)]/10 rounded-2xl border-1 
                                            border-[var(--cont)]/15 backdrop-blur-[5px] max-h-[60vh]">

                            <h1 className="text-[var(--cont)] text-4xl flex items-center gap-2
                                        uppercase w-min m-2"
                            ><span className="text-[var(--primary)]"><CheckCircleBroken size="30px"/></span> Changes</h1>

                            <div className="ps-5 pt-3 border-[var(--cont)]/5 flex flex-col border-t-2
                                            gap-5 min-h-[200px] overflow-y-scroll mt-2 max-h-[calc(60vh_-_75px)] ">
                                {
                                    document.changes
                                    .sort((a,b) => b.timestamp - a.timestamp)
                                    .map((doc,i) => i < 10 &&
                                    <div className="flex flex-col gap-2">
                                        <div className="flex w-full gap-2 backdrop-blur border border-[var(--cont)]/10 
                                                        p-2 pe-4 ps-3 rounded-3xl ms-[-15px]">
                                            <p className="w-full flex justify-between">
                                                <div>
                                                    {
                                                    doc.isValidSignature 
                                                    ? <span className="text-[var(--info)]   flex gap-1 uppercase"><CheckCircle/> signature is valid</span>
                                                    : <span className="text-[var(--danger)] flex gap-1 uppercase"><AlertCircle/> signature is invalid</span>
                                                    }
                                                </div>
                                                <span>{convertTicksToJs(doc.timestamp).toLocaleString()}</span>
                                            </p>
                                        </div>
                                        <div className="flex w-full gap-2">
                                            <span>ID</span>
                                            <p className="bg-[var(--cont)]/5 rounded  px-1">{doc.id}</p>
                                        </div>
                                        <div className="flex w-full gap-2">
                                            <span>User</span>
                                            <p className="bg-[var(--cont)]/5 rounded  px-1">{doc.owner}</p>
                                        </div>
                                    </div>
                                    )
                                }
                
                            </div>
                        </section>
                    </section>
                    <button className="w-full flex gap-2 rounded-xl bg-[var(--primary)] p-1 m-3
                            max-w-[140px] uppercase font-semibold
                            hover:shadow-[0_0_5px_var(--primary)] text-white"
                    onClick={()=>navigate(`/document/${id}`)}
                    ><ArrowBlockLeft/> document</button>
                </>
            }
        </main>
    );
}

export default Editor;