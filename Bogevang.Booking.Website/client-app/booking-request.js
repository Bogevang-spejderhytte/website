$(async function () {
  Vue.use(FormsEditor);

  var bookingApp = new Vue({
    el: '#requestApp',
    components: {
      vuejsDatepicker
    },
    data: {
      da: vdp_translation_da.js,
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
      comments: null
    },

    mounted() {
      this.openEditableInputs();
    },

    methods: {
      showCalendar() {
        window.open('/reservationer/kalender', 'bogevang-kalender');
      },


      send: async function (e) {
        var result = await this.sendData();
        if (result) {
          window.location = "/booking-success";
        }
      },


      cancel: function (e) {
        window.location = "/";
      },


      sendData: async function () {
        var sendArgs = {
          arrivalDate: this.arrivalDate,
          departureDate: this.departureDate,
          tenantCategoryId: this.tenantCategoryId,
          tenantName: this.tenantName,
          purpose: this.purpose,
          contactName: this.contactName,
          contactPhone: this.contactPhone,
          contactAddress: this.contactAddress,
          contactCity: this.contactCity,
          contactEMail: this.contactEMail,
          comments: this.comments
        };

        return this.postWithErrorHandling(
          "/api/booking-request",
          sendArgs
        );
      }
    }
  });
});