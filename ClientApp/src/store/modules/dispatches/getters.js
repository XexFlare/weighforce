export default {
  fromDate: (state) => {
    return state.from.toISOString().substr(0, 10)
  },
  toDate: (state) => {
    return state.to.toISOString().substr(0, 10)
  },
  inTransit: (state) => {
    return state.inTransit
  },
  canGoPrevPage: (state) => state.page > 1,
  canGoNextPage: (state) => state.page < state.maxPage,
}