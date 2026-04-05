export default {
  setCurrentQuery(state, page) {
    state.currentQuery = page
  },
  setOsr(state, osr) {
    state.osr = osr.data
    state.maxPage = osr.totalPages
    state.totalRecords = osr.totalRecords
  },
  setDispatches(state, dispatches) {
    state.inTransit = dispatches.data
    state.maxPage = dispatches.totalPages
    state.totalRecords = dispatches.totalRecords
  },
  setProcessed(state, processed) {
    state.processed = processed.data
    state.maxPage = processed.totalPages
    state.totalRecords = processed.totalRecords
  },
  setTemp(state, temp) {
    state.temp = temp.data
    state.maxPage = temp.totalPages
    state.totalRecords = temp.totalRecords
  },
  setPrinted(state, id) {
    state.inTransit = state.inTransit.map((dispatch) => {
      if (dispatch.id == id) {
        if (dispatch.status == "Transit")
          dispatch.initialWeight.printed = true
        else dispatch.receivalWeight.printed = true
        return dispatch
      }
      return dispatch;
    });
    state.processed = state.processed.map((dispatch) => {
      if (dispatch.id == id) {
        if (dispatch.status == "Transit")
          dispatch.initialWeight.printed = true
        else dispatch.receivalWeight.printed = true
        return dispatch
      }
      return dispatch;
    });
    state.temp = state.temp.map((dispatch) => {
      if (dispatch.id == id) {
        if (dispatch.status == "Transit")
          dispatch.initialWeight.printed = true
        else dispatch.receivalWeight.printed = true
        return dispatch
      }
      return dispatch;
    });
  },
  updateDispatch(state, updatedDispatch) {
    state.inTransit = state.inTransit.map((dispatch) => {
      if (dispatch.id == updatedDispatch.id) {
        return updatedDispatch;
      }
      return dispatch;
    });
  },
  updateBooking(state, { updatedDispatch, booking }) {
    state.inTransit = state.inTransit.map((dispatch) => {
      if (dispatch.id == updatedDispatch.id) {
        updatedDispatch.booking = booking
        updatedDispatch.product = booking.transportInstruction.product
        updatedDispatch.contract = booking.transportInstruction.contract
        return updatedDispatch
      }
      return dispatch;
    });
  },
  updateTempDispatch(state, updatedDispatch) {
    state.temp = state.temp.map((dispatch) => {
      if (dispatch.id == updatedDispatch.id) {
        return updatedDispatch;
      }
      return dispatch;
    });
  },
  removeDispatch(state, del) {
    state.inTransit = state.inTransit.filter(
      (dispatch) => dispatch.id != del.id
    );
  },
  resetFilters(state) {
    const date = new Date();
    date.setDate(date.getDate() - 7);
    state.from = date
    state.to = new Date()
    state.page = 1
    state.size = 50
    state.type = ""
  },
  setPage(state, page) {
    state.page = page
  },
  setPageSize(state, size) {
    state.size = size
  },
  setFromDate(state, date) {
    state.from = date
  },
  setToDate(state, date) {
    state.to = date
  },
  setType(state, type) {
    state.type = type
  },
  setMaxPage(state, maxPage) {
    state.maxPage = maxPage
  },
  setTotalRecords(state, totalRecords) {
    state.totalRecords = totalRecords
  }
}