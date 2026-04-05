import { BaseService } from "./base-service"

export class Booking extends BaseService {
    constructor(headers) {
        super('bookings', headers)
    }
}
export class LocationsApi extends BaseService {
    constructor(headers) {
        super('offices', headers)
    }
}
export class CountriesApi extends BaseService {
    constructor(headers) {
        super('countries', headers)
    }
}
export class UnitsApi extends BaseService {
    constructor(headers) {
        super('units', headers)
    }
}
export class ProductsApi extends BaseService {
    constructor(headers) {
        super('products', headers)
    }
}
export class ContractsApi extends BaseService {
    constructor(headers) {
        super('contracts', headers)
    }
}
export class TpisApi extends BaseService {
    constructor(headers) {
        super('tis', headers)
    }
}
export class Transporter extends BaseService {
    constructor(headers) {
        super('transporters', headers)
    }
}
export class UserApi extends BaseService {
    constructor(headers) {
        super('users', headers)
    }
}