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
      onlySelectedWeekdays: null,
      selectedWeekdays: [],
      tenantCategoryId: null,
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
      showRates() {
        window.open('/reservationer/priser', 'bogevang-secondary');
      },


      showCalendar() {
        window.open('/reservationer/kalender', 'bogevang-secondary');
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
        if (!this.approveTerms) {
          window.alert("Du skal acceptere handelsbetingelserne først.");
          return;
        }

        var sendArgs = {
          arrivalDate: this.truncateHoursFromDate(this.arrivalDate),
          departureDate: this.truncateHoursFromDate(this.departureDate),
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
        debugger
        return this.postWithErrorHandling(
          "/api/booking-request",
          sendArgs
        );
      }
    }
  });
});