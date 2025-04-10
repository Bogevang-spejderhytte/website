﻿$(async function () {
  Vue.use(FormsEditor);

  var bookingsApp = new Vue({
    el: '#bookingsApp',
    data: {
      orderBy: 'ArrivalDate',
      sortDirection: 'Asc',
      stateFilter: ["Requested", "Approved"],
      selectableYears: [],
      selectedYear: "all",
      bookingNumber: null,
      bookings: []
    },

    async mounted() {
      this.selectableYears = [{ title: "Alle år", value: "all"}];
      for (var year = new Date().getFullYear(); year > 2000; --year)
        this.selectableYears.push({ title: year, value: year });
      await this.search();
      this.openEditableInputs();
    },

    methods:
    {
      search: async function () {
        searchArgs = {
          orderBy: this.orderBy,
          sortDirection: this.sortDirection,
          bookingState: this.stateFilter,
          year: this.selectedYear
        };

        result = await this.postWithErrorHandling("/api/bookings", searchArgs);
        if (result) {
          this.bookings = result.data;
          this.bookings.forEach(b => {
            b.arrivalDate = new Date(b.arrivalDate).toLocaleDateString();
            b.departureDate = new Date(b.departureDate).toLocaleDateString();
            b.createdDate = new Date(b.createdDate).toLocaleDateString();
          });
        }
      },

      async gotoBooking() {
        if (!this.bookingNumber)
          return;

        searchArgs = {
          bookingNumber: this.bookingNumber
        };

        // 
        //this.bookingNumber = null;

        result = await this.postWithErrorHandling("/api/bookings", searchArgs);
        if (result && result.data && result.data.length > 0) {
          const url = '/reservationer/edit#' + result.data[0].id;
          window.location = url;
        }
        else {
          window.alert('Ukendt aftalenummer');
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

      privateClass: function (p) {
        if (p)
          return 'fas fa-eye-slash color-neutral';
        else
          return null;
      },

      alertClass: function (a) {
        if (a == 'New')
          return 'fas fa-star color-new';
        else if (a == 'Key')
          return 'fas fa-envelope color-warning';
        else if (a == 'AwaitingCheckout')
          return 'fas fa-balance-scale color-warning';
        else if (a == 'Finalize')
          return 'fas fa-money-bill-alt color-warning';
        else
          return null;
      },

      rowClass: function (isCancelled) {
        if (isCancelled)
          return 'grid-row row-cancelled';
        else
          return 'grid-row';
      }
    }
  });
});