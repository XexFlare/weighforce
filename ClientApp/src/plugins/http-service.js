const ROOT_URL = import.meta.env.VITE_API + '/api/';

export class HttpService {

    headers = {}

    constructor(url_prefix = "", { headers }) {
        this.url_prefix = url_prefix
        this.headers = {
            ...headers,
            Accept: "application/json",
            "Content-Type": "application/json",
        }
        // this.getHeaders()
    }

    async get(url, queryParams) {
        try {
            let response = await fetch(ROOT_URL + this.getUrl(url) + this.mapQueryParams(queryParams), {
                headers: this.headers
            })
            let jsonResponse = await response.json()
            return jsonResponse
        } catch (error) {
            console.log(error)
            return null
        }
    }

    async post(url, body, queryParams = null) {
        try {
            let response = await fetch(ROOT_URL + this.getUrl(url) + this.mapQueryParams(queryParams), {
                method: "POST",
                headers: this.headers,
                body: JSON.stringify(body)
            })

            let jsonResponse = await response.json()
            return jsonResponse
        } catch (error) {
            console.log(error);
            return null
        }

    }

    async put(url, body, queryParams = null) {
        try {
            let response = await fetch(ROOT_URL + this.getUrl(url) + this.mapQueryParams(queryParams), {
                method: "PUT",
                headers: this.headers,
                body: JSON.stringify(body)
            })
            let jsonResponse = await response.json()
            return jsonResponse
        } catch (error) {
            console.log(error);
            return null
        }
    }

    async remove(url, queryParams = null) {
        try {
            let response = await fetch(ROOT_URL + this.getUrl(url) + this.mapQueryParams(queryParams), {
                method: "DELETE",
                headers: this.headers
            })
            let jsonResponse = await response.json()
            return jsonResponse
        } catch (error) {
            console.log(error)
            return null
        }
    }

    getUrl(url) {
        return this.url_prefix + url
    }

    getHeaders() {
        this.headers = {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        }
        if (this.checkSession()) {
            let apiToken = this.getSession().api_token
            this.headers = {
                ...this.headers,
                "Authorisation": `Bearer ${apiToken}`
            }
        }
    }

    getSession() {
        let session = localStorage.getItem('SESSION_KEY')
        if (session) {
            return JSON.parse(session)
        }
        return session
    }

    checkSession() {
        return localStorage.getItem('SESSION_KEY') !== null
    }

    mapQueryParams(queryParams) {
        return queryParams
            ? Object.keys(queryParams).map(function (key) {
                return key + '=' + queryParams[key]
            }).join('&')
            : ""
    }
}