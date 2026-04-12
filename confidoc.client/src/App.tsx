import { BrowserRouter, Route, Routes } from "react-router-dom"
import Home from "./pages/Home";
import Layout from "./Layout";
import Login from "./pages/Login";
import Register from "./pages/Register";
import NotFound from "./NotFound";
import Dashboard from "./pages/Dashboard";
import DocumentLander from "./components/Document/Lander";
import GroupLander from "./components/Group/Lander";
import Editor from "./components/Document/Editor";
import LogOut from "./components/LogOut";
import Account from "./components/Dashboard/Account";
import NewDocument from "./components/Dashboard/NewDocument";
import Documents from "./components/Dashboard/Documents";
import Groups from "./components/Dashboard/Groups";
import NewGroup from "./components/Dashboard/NewGroup";
import Main from "./components/Dashboard/Main";

function App() {

  return (
    <BrowserRouter>
        <Routes>
            <Route path="/" element={<Layout />} >
                <Route index element={<Home />} />
                <Route path="/login" element={<Login />} /> 
                <Route path="/register" element={<Register />} />
                <Route path="/logout" element={<LogOut/>} />
                <Route
                    path="/document/:id"
                    element={<DocumentLander/>}
                />
                <Route
                    path="/document/:id/modify"
                    element={<Editor />}
                />
                <Route 
                    path="/group/:id"
                    element={<GroupLander/>}
                />
                <Route path="*" element={<NotFound />} />
            </Route>
            <Route path="/dashboard" element={<Dashboard /> }>
                <Route index element={<Main/>}/>
                <Route path="account" element={<Account/>} />
                <Route path="documents">
                    <Route index element={<Documents/>}/>    
                    <Route path="new" element={<NewDocument/>}/>    
                </Route>
                <Route path="groups">
                    <Route index element={<Groups/>}/>
                    <Route path="new" element={<NewGroup/>}/>
                </Route>
            </Route>
        </Routes>
    </BrowserRouter>
  )
}

export default App
