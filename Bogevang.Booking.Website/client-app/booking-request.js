$(async function () {
  Vue.use(FormsEditor);

  var bookingApp = new Vue({
    el: '#requestApp',
    data: {
      loading: true,
      errors: [],
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
      this.startEditing();
    },

    methods: {
      send: async function (e) {
        this.errors = [];
        var result = await this.sendData();
        if (result) {
          window.location = "/booking-success";
        }
      },


      cancel: function (e) {
        window.location = "/";
      },


      clearValidation: function (e) {
        $(e.srcElement).removeClass('is-invalid');
      },


      startEditing: function (e) {
        $('.editable').prop('readonly', false);
        $('select.editable').prop('disabled', false);
        $('.editable').removeClass('is-invalid');
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

        const requestVerificationToken = this.$el.getAttribute('requestVerificationToken');

        return this.postWithErrorHandling(
          "/api/booking-request",
          sendArgs,
          requestVerificationToken
        );
      },
    }
  });
});