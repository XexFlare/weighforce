import actions from './actions'
import mutations from './mutations'
import getters from './getters'

const date = new Date();
date.setDate(date.getDate() - 7);
const state = () => ({
  currentQuery: "dispatches",
  inTransit: [],
  osr: [],
  processed: [],
  temp: [],
  status: null,
  page: 1,
  maxPage: 10,
  size: 50,
  from: date,
  to: new Date(),
  type: ""
})

export default {
  namespaced: true,
  state,
  getters,
  actions,
  mutations,
}