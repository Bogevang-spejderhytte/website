$(async function () {
  var bookingApp = new Vue({
    el: '#bookingApp',
    data: {
      loading: true,
      errors: [],
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
      depositReceived: null,
      paymentReceived: null,
      depositReturned: null
    },
    async mounted() {
      this.loadData();
    },
    methods: {
      edit: function(e) {
        this.startEditing();
      },

      save: function (e) {
        this.saveData();
        this.stopEditing();
      },

      cancel: function (e) {
        this.stopEditing();
      },

      close: function (e) {
        
      },


      loadData: function () {
        this.loading = true;
        fetch("/api/booking?id=" + this.bookingId, {
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

        this.loading = false;
      },


      saveData: function () {
        var saveArgs = {
          arrivalDate: this.arrivalDate
        };

        const requestVerificationToken = this.$el.getAttribute('requestVerificationToken');

        debugger
        fetch("/api/booking?id=" + this.bookingId, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': requestVerificationToken
          },
          body: saveArgs
        })
          .then(res => res.json())
          .then(data => {
          })
          .catch(err => {
            window.alert("FEJL!");
          });
      },

      startEditing: function (e) {
        $('.editable').prop('readonly', false);
        $('select.editable').prop('disabled', false);

        $('#editButton').prop('disabled', true);
        $('#saveButton').prop('disabled', false);
        $('#cancelButton').prop('disabled', false);
        $('#closeButton').prop('disabled', true);
      },

      stopEditing: function (e) {
        $('.editable').prop('readonly', true);
        $('select.editable').prop('disabled', true);

        $('#editButton').prop('disabled', false);
        $('#saveButton').prop('disabled', true);
        $('#cancelButton').prop('disabled', true);
        $('#closeButton').prop('disabled', false);
      }
    }
  });
});