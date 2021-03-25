$(async function () {
  Vue.use(FormsEditor);

  var bookingsApp = new Vue({
    el: '#bookingsApp',
    data: {
      orderBy: 'ArrivalDate',
      sortDirection: 'Asc',
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
          this.bookings.forEach(b => {
            b.arrivalDate = new Date(b.arrivalDate).toLocaleDateString();
            b.departureDate = new Date(b.departureDate).toLocaleDateString();
            b.createdDate = new Date(b.createdDate).toLocaleDateString();
          });
        }
      },

      datePart: function (s) {
        return s.substr(0, 10);
      },

      alertClass: function (a) {
        if (a == 'New')
          return 'fas fa-star color-new';
        else if (a == 'Key')
          return 'fas fa-key color-warning';
        else if (a == 'Finalize')
          return 'fas fa-money-bill-alt color-warning';
        else
          return null;
      }
    }
  });
});