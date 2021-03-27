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

      orderByArrival: async function () {
        if (this.orderBy == 'ArrivalDate') {
          this.sortDirection = this.invertSortDirection(this.sortDirection);
        }
        else {
          this.sortDirection = 'Asc';
          this.orderBy = 'ArrivalDate';
        }
        await this.search();
      },

      arrivalIconState: function () {
        return this.sortState('ArrivalDate');
      },

      createdIconState: function () {
        return this.sortState('CreatedDate');
      },

      sortState: function (name) {
        if (this.orderBy != name)
          return null;
        else if (this.sortDirection == 'Desc')
          return 1;
        else
          return 2;
      },

      orderByCreated: async function () {
        if (this.orderBy == 'CreatedDate') {
          this.sortDirection = this.invertSortDirection(this.sortDirection);
        }
        else {
          this.sortDirection = 'Asc';
          this.orderBy = 'CreatedDate';
        }
        await this.search();
      },

      invertSortDirection: function (dir) {
        return dir == 'Asc' ? 'Desc' : 'Asc';
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