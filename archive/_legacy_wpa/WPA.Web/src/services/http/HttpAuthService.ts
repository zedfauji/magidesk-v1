import { api } from "./api";
import type { IAuthService } from "../interfaces";
import type { AuthSession, User } from "../../types";

// Types matching Backend DTOs
interface LoginRequest {
    pin: string;
}

interface UserDto {
    id: string;
    username: string;
    firstName: string;
    lastName: string;
    role: string;
}

interface AuthSessionDto {
    token: string;
    terminalId: string;
    startedAt: string;
    // Note: Backend might need to include user info here or we fetch separately
}

export class HttpAuthService implements IAuthService {
    async login(pin: string): Promise<User> {
        const userDto = await api.post<UserDto>('/auth/login', { pin } as LoginRequest);

        return {
            id: userDto.id,
            username: userDto.username,
            firstName: userDto.firstName,
            lastName: userDto.lastName,
            role: userDto.role as any // logical cast, validation needed in real app
        };
    }

    async logout(): Promise<void> {
        await api.post('/auth/logout', {});
    }

    async getCurrentSession(): Promise<AuthSession | null> {
        try {
            const sessionDto = await api.get<AuthSessionDto>('/auth/session');

            // Gap: Backend session endpoint doesn't return full user object in DTO currently
            // defaulting to a "session user" placeholder or needing a second call.
            // For now, mapping what we have.

            return {
                token: sessionDto.token,
                terminalId: sessionDto.terminalId,
                startedAt: sessionDto.startedAt,
                user: { id: 'unknown', username: 'Recovering...', role: 'Server', firstName: 'Resumed', lastName: 'User' } // Placeholder until fixed
            };
        } catch (e) {
            return null;
        }
    }
}
