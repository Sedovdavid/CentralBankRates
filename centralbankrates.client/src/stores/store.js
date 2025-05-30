import {defineStore} from 'pinia'

export const DatesStore = defineStore('counter', {
  state: () => ({
    dateFrom: null,
    dateTo: null
  }),
  actions: {
    setDates(dateFrom, dateTo) {
      this.dateFrom = dateFrom
      this.dateTo = dateTo
    },
  },
})
