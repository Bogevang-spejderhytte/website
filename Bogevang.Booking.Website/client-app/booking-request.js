$(async function () {
  Vue.use(FormsEditor);

  var bookingApp = new Vue({
    el: '#requestApp',
    data: {
      arrivalDate: null,
      departureDate: null,
      onlySelectedWeekdays: false,
      selectedWeekdays: [],
      tenantCategoryId: '',
      tenantName: null,
      purpose: null,
      contactName: null,
      contactPhone: null,
      contactAddress: null,
      contactCity: null,
      contactEMail: null,
      comments: null,
      approveTerms: false
    },

    mounted() {
      this.openEditableInputs();
    },

    methods: {
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
        if (!this.approveTerms) {
          window.alert("Du skal acceptere handelsbetingelserne først.");
          return;
        }

        var sendArgs = {
          arrivalDate: this.arrivalDate,
          departureDate: this.departureDate,
          onlySelectedWeekdays: this.onlySelectedWeekdays,
          selectedWeekdays: this.selectedWeekdays,
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