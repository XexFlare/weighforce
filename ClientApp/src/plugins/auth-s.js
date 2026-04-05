import { SESSION_KEY } from "common/values";
import { BaseService } from "./base-service";

export class AuthService extends BaseService {

    constructor() {
        super()
    }

    async login(body) {
        let response = await this.http.post("login", body)
        if (response.success) {
            localStorage.setItem(SESSION_KEY, JSON.stringify(response.data))
        }
        return response
    }

    async register(body) {
        let response = await this.http.post("register", body)
        if (response.success) {
            localStorage.setItem(SESSION_KEY, JSON.stringify(response.data))
        }
        return response
    }
}