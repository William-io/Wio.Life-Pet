import { User } from "./user-interface";

export interface AuthResponse {
    isSuccess: boolean;
    user: User;
    token: string;
}