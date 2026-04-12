import { useNavigate } from "react-router-dom";

function NotFound() {
    const navigate = useNavigate();

    return (
        <main>
            <section className="flex justify-center mx-2">
                <div className="mt-[calc(50px+10vh)] mb-[calc(75px+10vh)]">
                    <h1 className="text-7xl font-bold text-center text-[var(--primary)]">404 Not Found</h1>
                    <p className="text-center">
                        This page has been either deleted or you do not have access to it. <a onClick={() => navigate("/")} className="underline cursor-pointer">Go home?</a>
                    </p>
                </div>
            </section>
        </main>
    );
}

export default NotFound;