$(async function () {
  Vue.use(FormsEditor);

  var bookingsApp = new Vue({
    el: '#bookingsApp',
    data: {
      orderBy: 'ArrivalDate',
      sortDirection: 'Desc',
      bookingState: null,
      bookings: []
    },
    async mounted() {
      await this.search();
      this.openEditableInputs();
    },

    methods:
    {
      search: async function () {
        query = new URLSearchParams({
          orderBy: this.orderBy,
          sortDirection: this.sortDirection
        });

        if (this.bookingState)
          query.append('bookingState', this.bookingState);

        result = await this.getWithErrorHandling("/api/bookings?" + query.toString());
        if (result) {
          this.bookings = result.data;
        }
      },

      datePart: function (s) {
        return s.substr(0, 10);
      }
    }
  });
});