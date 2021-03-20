$(async function () {
  var bookingApp = new Vue({
    el: '#bookingApp',
    data: {
      loading: true,
      arrivalDate: null,
      departureDate: null,
      tenantCategoryId: null,
      tenantName: null,
      purpose: null,
      contactName: null,
      contactPhone: null,
      contactAddress: null,
      contactCity: null,
      contactEMail: null,
      comments: null,
      rentalPrice: null,
      bookingState: null,
      depositReceived: null,
      paymentReceived: null,
      depositReturned: null
    },
    async mounted() {
      debugger
      const id = location.hash.substr(1);
      fetch("/api/booking?id=" + id, {
        "method": "GET"
      })
        .then(response => {
          return response.json();
        })
        .then(j => {
          this.arrivalDate = j.data.arrivalDate.substr(0, 10);
          this.departureDate = j.data.departureDate.substr(0, 10);
          this.tenantCategoryId = j.data.tenantCategoryId;
          this.tenantName = j.data.tenantName;
          this.purpose = j.data.purpose;
          this.contactName = j.data.contactName;
          this.contactPhone = j.data.contactPhone;
          this.contactAddress = j.data.contactAddress;
          this.contactCity = j.data.contactCity;
          this.contactEMail = j.data.contactEMail;
          this.comments = j.data.comments;
          this.rentalPrice = j.data.rentalPrice;
          this.bookingState = j.data.bookingState;
          this.depositReceived = j.data.depositReceived;
          this.paymentReceived = j.data.paymentReceived;
          this.depositReturned = j.data.depositReturned;
        });

      // Show entries now that everything is loaded
      this.loading = false;
    }
  });
});