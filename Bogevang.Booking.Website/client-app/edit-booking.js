$(async function () {
  Vue.use(FormsEditor);

  var bookingApp = new Vue({
    el: '#bookingApp',
    components: {
      vuejsDatepicker
    },
    data: {
      da: vdp_translation_da.js,
      bookingId: location.hash.substr(1),
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
      isConfirmed: false
    },

    async mounted() {
      this.loadData();
    },

    methods: {
      edit: function(e) {
        this.startEditing();
      },


      save: async function (e) {
        var result = await this.saveData();
        if (result) {
          this.stopEditing();
          this.loadData();
        }
      },


      confirm: async function () {
        if (!confirm('Bekræft denne reservation?'))
          return;

        var result = await this.confirmBooking();
        if (result) {
          window.location = '/reservationer/send-mail?id=' + this.bookingId + '&template=reservationskvittering';
        }
      },


      sendMail() {
        window.location = '/reservationer/send-mail?id=' + this.bookingId + '&template=standard';
      },


      deleteBooking: async function (url) {
        if (!confirm('Slet denne reservation (uden at informere lejer)?'))
          return;

        result = await this.deleteData();
        if (result) {
          window.location = url;
        }
      },


      cancel: function (e) {
        this.stopEditing();
        this.loadData();
      },


      close: function (url) {
        window.location = url;
      },


      loadData: async function () {
        result = await this.getWithErrorHandling("/api/booking?id=" + this.bookingId);
        if (result) {
          const data = result.data;
          this.arrivalDate = new Date(data.arrivalDate);
          this.departureDate = new Date(data.departureDate);
          this.tenantCategoryId = data.tenantCategoryId;
          this.tenantName = data.tenantName;
          this.purpose = data.purpose;
          this.contactName = data.contactName;
          this.contactPhone = data.contactPhone;
          this.contactAddress = data.contactAddress;
          this.contactCity = data.contactCity;
          this.contactEMail = data.contactEMail;
          this.comments = data.comments;
          this.rentalPrice = data.rentalPrice;
          this.bookingState = data.bookingState;
          this.isConfirmed = data.isConfirmed;
          this.warnings = data.warnings;
        }
      },


      saveData: async function () {
        var saveArgs = {
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
          comments: this.comments,
          rentalPrice: this.rentalPrice,
          bookingState: this.bookingState
        };
        return await this.postWithErrorHandling(
          "/api/booking?id=" + this.bookingId,
          saveArgs
        );
      },


      confirmBooking: async function () {
        var confirmArgs = {}

        return await this.postWithErrorHandling(
          "/api/confirm-booking?id=" + this.bookingId,
          confirmArgs
        );
      },


      deleteData: async function () {
        return await this.deletetWithErrorHandling(
          "/api/booking?id=" + this.bookingId);
      },


      startEditing: function (e) {
        this.openEditableInputs();

        $('#editButton').prop('disabled', true);
        $('#saveButton').prop('disabled', false);
        $('#cancelButton').prop('disabled', false);
        $('#deleteButton').prop('disabled', true);
        $('#confirmButton').prop('disabled', true);
        $('#closeButton').prop('disabled', true);
      },


      stopEditing: function (e) {
        this.closeEditableInputs();

        $('#editButton').prop('disabled', false);
        $('#saveButton').prop('disabled', true);
        $('#cancelButton').prop('disabled', true);
        $('#deleteButton').prop('disabled', false);
        $('#confirmButton').prop('disabled', false);
        $('#closeButton').prop('disabled', false);
      }
    }
  });
});