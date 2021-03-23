$(async function () {
  Vue.use(FormsEditor);

  var bookingsApp = new Vue({
    el: '#bookingsApp',
    data: {
      loading: true,
      bookings: []
    },
    async mounted() {
      debugger
      result = await this.getWithErrorHandling("/api/bookings");
      if (result) {
        this.bookings = result.data;
      }
    }
  });
});