export default {
    async getProcessed(page, size, from, to, type, headers) {
        const processed = await fetch(
            `${import.meta.env.VITE_API}/api/dispatches/processed?page=${page}&size=${size}&from=${from}&to=${to}&type=${type}`,
            headers
        ).then((data) => data.json());
        return processed
    },
    async getTemp(page, size, from, to, type, headers) {
        const temp = await fetch(
            `${import.meta.env.VITE_API}/api/dispatches/temp?page=${page}&size=${size}&from=${from}&to=${to}&type=${type}`,
            headers
        ).then((data) => data.json());
        return temp
    },
    async getInTransit(page, size, from, to, type, headers) {
        const dispatches = await fetch(
            `${import.meta.env.VITE_API}/api/dispatches?page=${page}&size=${size}&from=${from}&to=${to}&type=${type}`,
            await headers
        )
            .then((data) => data.json())
            .catch((error) => console.error("Unable to get dispatches.", error));
        return dispatches
    },
    async getPending(page, size, from, to, headers) {
        const dispatches = await fetch(
            `${import.meta.env.VITE_API}/api/dispatches/pending?page=${page}&size=${size}&from=${from}&to=${to}&type=Wagon`,
            await headers
        )
            .then((data) => data.json())
            .catch((error) => console.error("Unable to get dispatches.", error));
        return dispatches
    },
    async getWagons(page, size, from, to, headers) {
        const dispatches = await fetch(
            `${import.meta.env.VITE_API}/api/dispatches/speed?page=${page}&size=${size}&from=${from}&to=${to}&type=Wagon`,
            await headers
        )
            .then((data) => data.json())
            .catch((error) => console.error("Unable to get dispatches.", error));
        return dispatches
    },
    async getOsr(page, size, from, to, headers) {
        const dispatches = await fetch(
            `${import.meta.env.VITE_API}/api/bookings/osr?page=${page}&size=${size}&from=${from}&to=${to}`,
            await headers
        )
            .then((data) => data.json())
            .catch((error) => console.error("Unable to get dispatches.", error));
        return dispatches
    },
    async print(id, headers) {
        await fetch(`${import.meta.env.VITE_API}/api/dispatches/${id}/print`, {
            method: "POST",
            ...headers
        }).then((res) => console.log({ ...headers }))
            .catch((error) => console.error("Unable to get dispatches.", error));
    }
}