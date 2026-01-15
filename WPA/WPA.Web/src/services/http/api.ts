export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || "http://localhost:5000/api";

type RequestOptions = RequestInit & {
    headers?: Record<string, string>;
};

// Helper to get headers with Auth and Terminal ID
const getHeaders = (): Record<string, string> => {
    const headers: Record<string, string> = {
        "Content-Type": "application/json",
        // Placeholder: Terminal ID should ideally come from local storage or context
        "X-Terminal-Id": "T-08",
    };

    // Get token from storage (if we were using JWTs, mocked for now as 'mock-token' in auth service)
    // In a real app, this would be:
    // const token = localStorage.getItem('token');
    // if (token) headers["Authorization"] = `Bearer ${token}`;

    return headers;
};

export const api = {
    get: async <T>(endpoint: string, options: RequestOptions = {}): Promise<T> => {
        const response = await fetch(`${API_BASE_URL}${endpoint}`, {
            ...options,
            method: 'GET',
            headers: { ...getHeaders(), ...options.headers },
        });

        if (!response.ok) {
            throw new Error(`API Error: ${response.status} ${response.statusText}`);
        }

        return response.json();
    },

    post: async <T>(endpoint: string, body: any, options: RequestOptions = {}): Promise<T> => {
        const response = await fetch(`${API_BASE_URL}${endpoint}`, {
            ...options,
            method: 'POST',
            headers: { ...getHeaders(), ...options.headers },
            body: JSON.stringify(body),
        });

        if (!response.ok) {
            throw new Error(`API Error: ${response.status} ${response.statusText}`);
        }

        // Handle 204 No Content
        if (response.status === 204) return {} as T;

        return response.json();
    },

    // Add put/delete if needed later
};
