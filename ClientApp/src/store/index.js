import { createStore, createLogger } from 'vuex'
import dispatches from './modules/dispatches'
import auth from './modules/auth'

const debug = process.env.NODE_ENV !== 'production'

export default createStore({
    modules: {
        dispatches,
        auth
    },
    strict: debug,
    plugins: debug ? [createLogger()] : []
})