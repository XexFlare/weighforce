import { HttpService } from "./http-service";

export class BaseService {

    http

    constructor(url_prefix = "", headers) {
        this.http = (new HttpService(url_prefix, headers))
    }

    async getAll() {
        return await this.http.get(``)
    }

    async get(id) {
        return await this.http.get(`/${id}`)
    }

    async create(body) {
        return await this.http.post(``, body)
    }

    async update(id, body) {
        return await this.http.put(`/${id}`, body)
    }

    async delete(id) {
        return await this.http.remove(`/${id}`)
    }
}