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
      isApproved: false,
      isRejected: false,
      welcomeLetterIsSent: false,
      tenantSelfServiceToken: null
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


      approve: async function () {
        if (!confirm('Godkend denne reservation?'))
          return;

        var result = await this.doApprove();
        if (result) {
          window.location = '/reservationer/send-mail?id=' + this.bookingId + '&template=godkendelsesbrev';
        }
      },


      reject: async function () {
        if (!confirm('Afvis denne reservation?'))
          return;

        var result = await this.doReject();
        if (result) {
          window.location = '/reservationer/send-mail?id=' + this.bookingId + '&template=afvisningsbrev';
        }
      },


      sendWelcomeLetter: async function () {
        if (!confirm('Send velkomstbrev?'))
          return;

        var result = await this.doSendWelcomeLetter();
        if (result) {
          window.location = '/reservationer/send-mail?id=' + this.bookingId + '&template=velkomstbrev';
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


      openSelfService() {
        const url = '/reservationer/slutafregning?id=' + this.bookingId + '&token=' + this.tenantSelfServiceToken;
        window.open(url, 'bogevang-secondary');
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
          this.isApproved = data.isApproved;
          this.isRejected = data.isRejected;
          this.welcomeLetterIsSent = data.welcomeLetterIsSent;
          this.warnings = data.warnings;
          this.tenantSelfServiceToken = data.tenantSelfServiceToken;
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


      doApprove: async function () {
        var approveArgs = {}

        return await this.postWithErrorHandling(
          "/api/approve-booking?id=" + this.bookingId,
          approveArgs
        );
      },


      doReject: async function () {
        var rejectArgs = {}

        return await this.postWithErrorHandling(
          "/api/reject-booking?id=" + this.bookingId,
          rejectArgs
        );
      },


      doSendWelcomeLetter: async function () {
        var sendArgs = {}

        return await this.postWithErrorHandling(
          "/api/send-welcome-letter?id=" + this.bookingId,
          sendArgs
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
        $('#approveButton').prop('disabled', true);
        $('#rejectButton').prop('disabled', true);
        $('#sendWelcomeLetterButton').prop('disabled', true);
        $('#closeButton').prop('disabled', true);
      },


      stopEditing: function (e) {
        this.closeEditableInputs();

        $('#editButton').prop('disabled', false);
        $('#saveButton').prop('disabled', true);
        $('#cancelButton').prop('disabled', true);
        $('#deleteButton').prop('disabled', false);
        $('#approveButton').prop('disabled', false);
        $('#rejectButton').prop('disabled', false);
        $('#sendWelcomeLetterButton').prop('disabled', false);
        $('#closeButton').prop('disabled', false);
      }
    }
  });
});