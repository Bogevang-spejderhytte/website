$(async function () {
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
        debugger
        this.errors = [];
        var result = await this.sendData();
        debugger
        if (result) {
          // FIXME: we have both Cofoundry and ASP.NET validation responses
          if (!result.errors || result.isValid) {
            window.location = "/booking-success";
          }
          else {
            console.log(result.title);
            for (var key in result.errors) {
              if (result.errors.hasOwnProperty(key)) {
                for (var i in result.errors[key])
                  this.errors.push(result.errors[key][i]);
                if (key) {
                  $('#' + key).addClass('is-invalid');
                  $('#' + key + '_feedback').text(result.errors[key][0]);
                }
              }
            }
            window.scrollTo(0,0);
          }
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
        var sendArgs = JSON.stringify({
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
        });

        const requestVerificationToken = this.$el.getAttribute('requestVerificationToken');

        return fetch("/api/booking-request", {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': requestVerificationToken
          },
          body: sendArgs
        })
          .then(res => {
            debugger
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
    }
  });
});