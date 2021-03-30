﻿$(async function () {
  Vue.use(FormsEditor);

  var bookingApp = new Vue({
    el: '#checkoutApp',
    data: {
      bookingId: null,
      bookingToken: null,
      startReading: null,
      endReading: null,
      deposit: null,
      comments: null
    },

    computed: {
      kwhUsed() {
        const kwh = this.endReading - this.startReading;
        return kwh < 0 ? null : kwh;
      },

      electricityPriceTotal() {
        return this.kwhUsed * electricityPriceUnit;
      },

      totalPrice() {
        return this.electricityPriceTotal - this.deposit;
      }
    },

    async mounted() {
      const urlParams = new URLSearchParams(window.location.search);
      this.bookingId = urlParams.get('id');
      this.bookingToken = urlParams.get('token');
      this.deposit = bookingDeposit; // Global variable in html code
      this.startEditing();
    },

    methods: {
      send: async function (e) {
        var result = await this.sendData();
        if (result) {
          window.location = "/checkout-success";
        }
      },


      cancel: function (e) {
        window.location = "/";
      },


      sendData: async function () {
        var sendArgs = {
          token: this.bookingToken,
          startReading: this.startReading,
          endReading: this.endReading,
          comments: this.comments
        };

        return this.postWithErrorHandling(
          "/api/checkout-booking?id=" + this.bookingId,
          sendArgs
        );
      },


      startEditing: function (e) {
        this.openEditableInputs();
      }
    }
  });
});