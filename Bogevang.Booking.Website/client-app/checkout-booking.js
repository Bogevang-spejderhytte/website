$(async function () {
  Vue.use(FormsEditor);

  var bookingApp = new Vue({
    el: '#checkoutApp',
    data: {
      bookingId: null,
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

      electricityPrice() {
        return this.kwhUsed * electricityPrice;
      },

      totalPrice() {
        return this.electricityPrice - this.deposit;
      }
    },

    async mounted() {
      const urlParams = new URLSearchParams(window.location.search);
      this.bookingId = urlParams.get('id');
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
        };

        return this.postWithErrorHandling(
          "/api/checkout-booking?" + this.bookingId,
          sendArgs
        );
      },


      startEditing: function (e) {
        this.openEditableInputs();
      }
    }
  });
});