$(async function () {
  var bookingsApp = new Vue({
    el: '#bookingsApp',
    data: {
      loading: true,
      bookings: []
    },
    async mounted() {

      fetch("/api/bookings", {
        "method": "GET"
      })
        .then(response => {
          return response.json();
        })
        .then(j => {
          this.bookings = j.data.items
        });

      // Show entries now that everything is loaded
      this.loading = false;
    }
  });
});