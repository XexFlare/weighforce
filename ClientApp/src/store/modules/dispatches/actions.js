import api from '../../../api'

export default {
  async print({ commit, rootGetters }, dispatch) {
    if ((dispatch.status == "Transit" && dispatch.initialWeight.printed == false) || ((dispatch.status == "Processed" || dispatch.status == "Temp") && dispatch.receivalWeight.printed == false)) {
      try {
        await api.print(dispatch.id, rootGetters['auth/headers'])
        await new Promise((resolve) => setTimeout(resolve, 5000));
        commit('setPrinted', dispatch.id)
      } catch (error) {
        console.log(error)
      }
    }
  },
  async getInTransit({ commit, state, getters, rootGetters }) {
    const inTransit = await api.getInTransit(state.page, state.size, getters.fromDate, getters.toDate, state.type, rootGetters['auth/headers'])
    commit('setDispatches', inTransit)
  },
  async getPending({ commit, state, getters, rootGetters }) {
    const inTransit = await api.getPending(state.page, state.size, getters.fromDate, getters.toDate, rootGetters['auth/headers'])
    commit('setDispatches', inTransit)
  },
  async getWagons({ commit, state, getters, rootGetters }) {
    const wagons = await api.getWagons(state.page, state.size, getters.fromDate, getters.toDate, rootGetters['auth/headers'])
    commit('setDispatches', wagons)
  },
  async getOsr({ commit, state, getters, rootGetters }) {
    const osr = await api.getOsr(state.page, state.size, getters.fromDate, getters.toDate, rootGetters['auth/headers'])
    commit('setOsr', osr)
  },
  async getProcessed({ commit, state, getters, rootGetters }) {
    const processed = await api.getProcessed(state.page, state.size, getters.fromDate, getters.toDate, state.type, rootGetters['auth/headers'])
    commit('setProcessed', processed)
  },
  async getTemp({ commit, state, getters, rootGetters }) {
    const temp = await api.getTemp(state.page, state.size, getters.fromDate, getters.toDate, state.type, rootGetters['auth/headers'])
    commit('setTemp', temp)
  },
  setType({ dispatch, commit, state }, type) {
    commit('setType', type)
    dispatch(state.currentQuery)
  },
  setFromDate({ dispatch, commit, state }, date) {
    commit('setFromDate', date)
    dispatch(state.currentQuery)
  },
  setToDate({ dispatch, commit, state }, date) {
    commit('setToDate', date)
    dispatch(state.currentQuery)
  },
  nextPage({ dispatch, commit, state }) {
    if (state.page < state.maxPage) {
      commit('setPage', state.page + 1)
      dispatch(state.currentQuery)
    }
  },
  prevPage({ dispatch, commit, state }) {
    if (state.page > 1) {
      commit('setPage', state.page - 1)
      dispatch(state.currentQuery)
    }
  },
  resetPage({ dispatch, commit, state }) {
    commit('setPage', 1)
    dispatch(state.currentQuery)
  }
}