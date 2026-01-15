
import type { AuthSession, User } from "../../types";
import type { IAuthService } from "../interfaces";

export class MockAuthService implements IAuthService {
    private _currentUser: User | null = null;

    async login(pin: string): Promise<User> {
        // Mock login - treat '1234' as success
        if (pin === '1234') {
            this._currentUser = {
                id: 'u1',
                username: 'alex.g',
                firstName: 'Alex',
                lastName: 'G',
                role: 'Manager'
            };
            return this._currentUser;
        }

        if (pin === '1111') {
            this._currentUser = {
                id: 'u2',
                username: 'mike.s',
                firstName: 'Mike',
                lastName: 'S',
                role: 'Server'
            };
            return this._currentUser;
        }

        throw new Error("Invalid PIN");
    }

    async logout(): Promise<void> {
        this._currentUser = null;
    }

    async getCurrentSession(): Promise<AuthSession | null> {
        if (!this._currentUser) return null;
        return {
            token: 'mock-token',
            user: this._currentUser,
            terminalId: 'T-08',
            startedAt: new Date().toISOString()
        };
    }
}
