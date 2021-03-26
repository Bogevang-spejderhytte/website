$(async function () {
  Vue.use(FormsEditor);

  var bookingApp = new Vue({
    el: '#bookingApp',
    data: {
      bookingId: null,
      template: null
    },

    async mounted() {
      const urlParams = new URLSearchParams(window.location.search);
      this.bookingId = urlParams.get('id');
      this.template = urlParams.get('template');
    },

    methods: {
      send: async function () {
        debugger
        var subject = $('#subject')[0].value;
        var message = tinymce.get('message').getContent();
        var sendCommand = {
          bookingId: this.bookingId,
          subject: subject,
          message : message
        }

        const result = await this.postWithErrorHandling(
          "/api/send-booking-mail",
          sendCommand
        );

        if (result) {
          window.location = '/reservationer/edit#' + this.bookingId;
        }
      },

      cancel: function () {

      }
    }
  });
});