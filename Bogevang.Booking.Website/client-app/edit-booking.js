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
      bookingState: null
    },
    async mounted() {
      this.loadData();
    },
    methods: {
      edit: function(e) {
        this.errors = [];
        this.startEditing();
      },

      save: async function (e) {
        this.errors = [];
        var result = await this.saveData();
        debugger
        if (result) {
          if (!result.errors) {
            this.loadData();
            this.stopEditing();
          }
          else {
            console.log(result.title);
            for (var key in result.errors)
              if (result.errors.hasOwnProperty(key)) {
                this.errors.push(result.errors[key][0]);
                $('#' + key).addClass('is-invalid');
                $('#' + key + '_feedback').text(result.errors[key][0]);
              }
          }
        }
      },

      deleteBooking: function (url) {
        if (!confirm('Slet denne reservation (uden at informere lejer)?'))
          return;

        this.errors = [];

        const requestVerificationToken = this.$el.getAttribute('requestVerificationToken');

        return fetch("/api/booking?id=" + this.bookingId, {
          method: 'DELETE',
          headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': requestVerificationToken
          }
        })
          .then(res => {
            const contentType = res.headers.get('Content-Type').toLowerCase();
            console.log(contentType);
            console.log(res.status);
            if (contentType.includes('application/json') || contentType.includes('application/problem+json'))
              return res.json();
            else
              throw res;
          })
          .then(data => {
            console.log(data);
            window.location = url;
          })
          .catch(err => {
            window.alert(err);
            return null;
          });
      },

      cancel: function (e) {
        this.errors = [];
        this.stopEditing();
        this.loadData();
      },

      close: function (url) {
        window.location = url;
      },


      clearValidation: function (e) {
        $(e.srcElement).removeClass('is-invalid');
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

            this.loading = false;
          });
      },


      saveData: async function () {
        var saveArgs = JSON.stringify({
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
        });

        const requestVerificationToken = this.$el.getAttribute('requestVerificationToken');

        return fetch("/api/booking?id=" + this.bookingId, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': requestVerificationToken
          },
          body: saveArgs
        })
          .then(res => {
            const contentType = res.headers.get('Content-Type').toLowerCase();
            console.log(contentType);
            console.log(res.status);
            if (contentType.includes('application/json') || contentType.includes('application/problem+json'))
              return res.json();
            else
              throw res;
          })
          .then(data => {
            console.log(data);
            return data;
          })
          .catch(err => {
            window.alert(err);
            return null;
          });
      },

      startEditing: function (e) {
        $('.editable').prop('readonly', false);
        $('select.editable').prop('disabled', false);
        $('.editable').removeClass('is-invalid');

        $('#editButton').prop('disabled', true);
        $('#saveButton').prop('disabled', false);
        $('#cancelButton').prop('disabled', false);
        $('#deleteButton').prop('disabled', true);
        $('#closeButton').prop('disabled', true);
      },

      stopEditing: function (e) {
        $('.editable').prop('readonly', true);
        $('select.editable').prop('disabled', true);
        $('.editable').removeClass('is-invalid');

        $('#editButton').prop('disabled', false);
        $('#saveButton').prop('disabled', true);
        $('#cancelButton').prop('disabled', true);
        $('#deleteButton').prop('disabled', false);
        $('#closeButton').prop('disabled', false);
      }
    }
  });
});