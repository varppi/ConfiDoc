import { Flag01 } from "@untitledui/icons";
import { getToken, isSetup } from "../globals";
import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
 
function Home() {
    const navigate = useNavigate();

    useEffect(() => {

        isSetup().then(hasAdmin => {
            if (!hasAdmin) navigate("/setup");
        })

        const token = getToken();
        if (token != null && token.length > 5) {
            navigate("/dashboard")
        }
    }, [])

    return (
        <main className="min-md:px-5 px-2">
            <section className="h-[500px] w-full flex min-xl:justify-evenly max-md:flex-col ">
                <div className="w-full flex items-center max-w-[90vw] max-xl:ms-5 min-xl:justify-center max-xl:w-full">
                    <div className="max-w-[900px] mb-[5vh]">
                        <h1 className="text-5xl text-[var(--cont)] font-bold max-md:mt-5
                                       uppercase underline decoration-[var(--primary)] decoration-[3px]"
                        >Introducing Confidoc</h1>
                        <p className="text-[calc(0.3vw_+_20px)] mt-2">
                            Confidoc is a batteries included document management service allowing you to upload, modify and delete
                            documents securely reducing the risk of them falling into the wrong hands!
                        </p>
                        <button className="bg-[var(--primary)] w-full p-3 text-2xl text-white
                                           uppercase rounded-4xl mt-5 font-semibold
                                           hover:cursor-pointer hover:shadow-[0_0_10px_var(--primary)]"
                                onClick={()=>navigate("/register")}
                        >Get Started</button>
                    </div>
                </div>
                <div className="max-xl:hidden w-[70%] flex justify-start">
                    <img className="
                        object-cover max-h-[600px] h-[calc(30vw_+_50px)] image" src="/images/frontpage_1.png"></img>
                </div>
            </section>

            <section className="mt-[15vw]">
                <div className="flex w-full justify-center">
                    <h1 className="text-4xl text-[var(--cont)] font-bold max-md:text-3xl
                                   uppercase underline decoration-[var(--primary)] decoration-[3px]"
                    >Why is this better than &lt;insert service&gt;?</h1>
                </div>
                <div className="flex justify-center w-full mt-5">
                    <div className="flex max-md:flex-col gap-5 mt-3">
                        <div className="border border-[var(--primary)]/25 bg-[var(--primary)]/5 rounded-2xl max-w-[400px] min-h-[200px]">
                            <div className="flex justify-start">
                                <svg className="h-full max-h-[200px] absolute text-[var(--primary)]/20 z-[-1]" viewBox="0 0 24 24" fill="none">
                                    <path
                                        d="M17 10V8C17 5.23858 14.7614 3 12 3C9.23858 3 7 5.23858 7 8V10M12 14.5V16.5M8.8 21H15.2C16.8802 21 17.7202 21 18.362 20.673C18.9265 20.3854 19.3854 19.9265 19.673 19.362C20 18.7202 20 17.8802 20 16.2V14.8C20 13.1198 20 12.2798 19.673 11.638C19.3854 11.0735 18.9265 10.6146 18.362 10.327C17.7202 10 16.8802 10 15.2 10H8.8C7.11984 10 6.27976 10 5.63803 10.327C5.07354 10.6146 4.6146 11.0735 4.32698 11.638C4 12.2798 4 13.1198 4 14.8V16.2C4 17.8802 4 18.7202 4.32698 19.362C4.6146 19.9265 5.07354 20.3854 5.63803 20.673C6.27976 21 7.11984 21 8.8 21Z"
                                        stroke="currentColor"
                                        strokeWidth={2}
                                        strokeLinecap="round"
                                        strokeLinejoin="round"
                                    />
                                </svg>
                            </div>
                            <div className="p-3">
                                <h3 className="font-bold uppercase text-center text-3xl">Access Control</h3>
                                <p className="text-[calc(17px+0.25vw)]">With Confidoc, you can accurately track down each individual to a document and who all has access to set document.</p>
                            </div>
                        </div>
                        <div className="border border-[var(--primary)]/25 bg-[var(--primary)]/5 rounded-2xl max-w-[400px] min-h-[200px]">
                            <div className="flex justify-start">
                                <svg className="h-full max-h-[200px] absolute text-[var(--primary)]/20 z-[-1]" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                    <path
                                        d="M11.9998 8.99999V13M11.9998 17H12.0098M10.6151 3.89171L2.39019 18.0983C1.93398 18.8863 1.70588 19.2803 1.73959 19.6037C1.769 19.8857 1.91677 20.142 2.14613 20.3088C2.40908 20.5 2.86435 20.5 3.77487 20.5H20.2246C21.1352 20.5 21.5904 20.5 21.8534 20.3088C22.0827 20.142 22.2305 19.8857 22.2599 19.6037C22.2936 19.2803 22.0655 18.8863 21.6093 18.0983L13.3844 3.89171C12.9299 3.10654 12.7026 2.71396 12.4061 2.58211C12.1474 2.4671 11.8521 2.4671 11.5935 2.58211C11.2969 2.71396 11.0696 3.10655 10.6151 3.89171Z"
                                        stroke="currentColor"
                                        stroke-width="2"
                                        stroke-linecap="round"
                                        stroke-linejoin="round"
                                    />
                                </svg>

                            </div>
                            <div className="p-3">
                                <h3 className="font-bold uppercase text-center text-3xl">Tamper proof</h3>
                                <p className="text-[calc(17px+0.25vw)]">All modifications are cryptographically signed, making it impossible to modify the data without having access to a person's account directly.</p>
                            </div>
                        </div>

                        <div className="border border-[var(--primary)]/25 bg-[var(--primary)]/5 rounded-2xl max-w-[400px] min-h-[200px]">
                            <div className="flex justify-start">
                                <svg className="h-full max-h-[200px] absolute text-[var(--primary)]/20 z-[-1]" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                    <path
                                        d="M20 21C20 19.6044 20 18.9067 19.8278 18.3389C19.44 17.0605 18.4395 16.06 17.1611 15.6722C16.5933 15.5 15.8956 15.5 14.5 15.5H9.5C8.10444 15.5 7.40665 15.5 6.83886 15.6722C5.56045 16.06 4.56004 17.0605 4.17224 18.3389C4 18.9067 4 19.6044 4 21M16.5 7.5C16.5 9.98528 14.4853 12 12 12C9.51472 12 7.5 9.98528 7.5 7.5C7.5 5.01472 9.51472 3 12 3C14.4853 3 16.5 5.01472 16.5 7.5Z"
                                        stroke="currentColor"
                                        stroke-width="2"
                                        stroke-linecap="round"
                                        stroke-linejoin="round"
                                    />
                                </svg>
                            </div>
                            <div className="p-3">
                                <h3 className="font-bold uppercase text-center text-3xl">Mistake resistent</h3>
                                <p className="text-[calc(17px+0.25vw)]">You give only a limited time access to documents. After the time is up, the user can no longer view the document reducing the risk of prolonged access</p>
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            <section className="mt-[calc(100px+10vw)] flex justify-center">
                <div className="border-l-3 border-[var(--primary)] ms-5">
                    <h1 className="text-4xl text-[var(--cont)] font-bold  relative bottom-[40px] right-[3px]
                                   uppercase underline decoration-[var(--primary)] decoration-[3px]"
                    >How it works</h1>
                    <div className="flex flex-col gap-[50px] mt-5">
                        <div className="ps-5">
                            <div className="h-[3px] w-[50px] bg-[var(--primary)] relative top-[19px] left-[4px]">
                                <div className="h-[50px] w-[50px] bg-[var(--primary)] rounded-full text-white
                                                flex justify-center items-center relative right-[50px] bottom-[22.5px]">
                                    <b className="text-3xl">1</b>
                                </div>
                            </div>
                            <div className="ps-[60px]">
                                <h3 className="text-2xl uppercase text-[var(--primary)]">Register</h3>
                            </div>
                            <p className="max-w-[800px] text-xl text-[calc(15px+0.25vw)]">First you simply make an account. This does not require any personal information.</p>
                        </div>
                        <div className="ps-5">
                            <div className="h-[3px] w-[50px] bg-[var(--primary)] relative top-[19px] left-[4px]">
                                <div className="h-[50px] w-[50px] bg-[var(--primary)] rounded-full  text-white
                                                flex justify-center items-center relative right-[50px] bottom-[22.5px]">
                                    <b className="text-3xl">2</b>
                                </div>
                            </div>
                            <div className="ps-[60px]">
                                <h3 className="text-2xl uppercase text-[var(--primary)]">Keys</h3>
                            </div>
                            <p className="max-w-[800px] text-[calc(15px+0.25vw)]">When your account is created, your account will be assigned a set of cryptographic keys. Those keys are then encrypted with your password such that without your password, nobody can pretend to be you.</p>
                        </div>
                        <div className="ps-5">
                            <div className="h-[3px] w-[50px] bg-[var(--primary)] relative top-[19px] left-[4px]">
                                <div className="h-[50px] w-[50px] bg-[var(--primary)] rounded-full text-white
                                                flex justify-center items-center relative right-[50px] bottom-[22.5px]">
                                    <b className="text-3xl">3</b>
                                </div>
                            </div>
                            <div className="ps-[60px]">
                                <h3 className="text-2xl uppercase text-[var(--primary)]">document management</h3>
                            </div>
                            <p className="max-w-[800px] text-[calc(15px+0.25vw)]">When everything is set up, you can start uploading documents and exploring features like digital signing, collaborators etc...</p>
                        </div>
                        <div className="ps-5 mb-[-50px]">
                            <div className="h-[50px] w-[50px] bg-[var(--primary)] rounded-full text-white
                                            flex justify-center items-center relative right-[46px] bottom-[22.5px]">
                                <Flag01/>
                            </div>
                        </div>
                    </div>
                </div>
            </section>


        </main>
    );
}

export default Home;