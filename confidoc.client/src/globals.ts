import type { AxiosRequestConfig } from "axios";
import axios from "axios";

let theme: string | null = null;
let token: string | null = null;

// User preferences/settings/config
export function signalTheme(newTheme: string) {
    document.getElementById("html")
        ?.setAttribute("data-theme", newTheme)
    theme = newTheme;
    localStorage.setItem("theme", theme);
}

export function getTheme(): string | null {
    if (theme == null)
        signalTheme(localStorage.getItem("theme")??"dark");
    return theme;
}

export function setUsername(username: string) {
    localStorage.setItem("username", username);
}

export function getUsername() {
    return localStorage.getItem("username");
}

export function setToken(newToken: string) {
    token = newToken;
    localStorage.setItem("token", token);
}

export function getToken(): string | null {
    if (token == null)
        setToken(localStorage.getItem("token") ?? "")
    return token;
}

export function logOut() {
    localStorage.removeItem("token");
    localStorage.removeItem("passwords");
}

export function getPasswords(): DocPassword[] {
    return JSON.parse(localStorage.getItem("passwords") ?? "[]");
}

export function getPassword(id: string): string | null {
    const matches = getPasswords().filter(x => x.id == id);
    if (matches.length == 0) return null;
    return matches[0].password;
}

export async function setPassword(id: string, password: string) {
    const resp = await axios.post("/api/crypto/hash", {
        text: password
    }, getConfig());
    const hash = resp.data.hash;
    let passwords = getPasswords()
        .filter(x => x.id != id)
        .concat({id, password: hash});
    localStorage.setItem("passwords", JSON.stringify(passwords));
}

export function getConfig(): AxiosRequestConfig {
    const token = getToken();
    return {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    }
}

export function isLoggedIn(): boolean {
    return !(getToken() == null || getToken() == "");
}

// Helpers
export function convertTicksToJs(ticks: number): Date {
    const epochTicks = 621355968000000000,
        ticksPerMillisecond = 10000,
        input = ticks;
    const jsTicks = (input - epochTicks) / ticksPerMillisecond;
    const jsDate = new Date(jsTicks); 
    return jsDate;
}

// Interfaces
export interface DocPassword {
    id: string,
    password: string
}

export interface IApiError {
    errors: {
        [key: string]: string[]
    }
}

export interface GroupEntry {
    id: string,
    displayName: string,
    owner: string,
    members: string[]
}

export interface DocumentEntry {
    id: string,
    name: string,
    created: number,
    lastModified: number,
    size: number,
    owner: string,
    changes: unknown[],
    readAccessUsers: string[],
    writeAccessUsers: string[],
    readAccessGroups: string[],
    writeAccessGroups: string[]
}
