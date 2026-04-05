import api from '../../api'

const state = () => ({
  accessToken: '',
  officeId: null,
  status: null
})

// getters
const getters = {
  token: (state) => {
    return state.accessToken
  },
  headers: (state) => {
    return {
      headers: !state.accessToken
        ? {}
        : {
          Authorization: `Bearer ${state.accessToken}`,
          OfficeId: state.officeId,
        },
    };
  },
}

// actions
const actions = {

}

// mutations
const mutations = {
  setAccessToken(state, token) {
    state.accessToken = token
  },
  setOfficeId(state, id) {
    state.officeId = id
  },
}

export default {
  namespaced: true,
  state,
  getters,
  actions,
  mutations,
}