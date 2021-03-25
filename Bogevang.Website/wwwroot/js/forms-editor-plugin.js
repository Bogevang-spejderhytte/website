FormsEditorMixin =
{
  data: function () {
    var overlay = new bootstrap.Modal(document.getElementById('loaderOverlay'), {
      backdrop: "static", //remove ability to close modal with click
      keyboard: false, //remove option to close with keyboard
    });

    return {
      isWorking: true,
      errors: [],
      loaderModalOverlay: overlay
    }
  },

  methods: {
    clearErrors: function () {
      this.errors = [];
      $('.editable').removeClass('is-invalid');
    },


    openEditableInputs: function() {
      this.clearErrors();
      $('.editable').prop('readonly', false);
      $('select.editable').prop('disabled', false);
    },


    closeEditableInputs: function () {
      this.clearErrors();
      $('.editable').prop('readonly', true);
      $('select.editable').prop('disabled', true);
    },


    clearValidation: function (e) {
      $(e.srcElement).removeClass('is-invalid');
    },


    showModalSpinner: function () {
      this.loaderModalOverlay.show();
    },


    hideModalSpinner: function () {
      this.loaderModalOverlay.hide();
    },


    getWithErrorHandling: async function (url, errorHandler) {
      const options = {
        method: 'GET'
      }

      return await this.executeFetchWithErrorHandling(url, options, errorHandler);
    },


    postWithErrorHandling: async function (url, body, errorHandler) {
      const jsonBody = JSON.stringify(body);

      const requestVerificationToken = this.$el.elements['__RequestVerificationToken'].value;

      const options = {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'RequestVerificationToken': requestVerificationToken
        },
        body: jsonBody
      }

      return await this.executeFetchWithErrorHandling(url, options, errorHandler);
    },


    deletetWithErrorHandling: async function (url, errorHandler) {
      const requestVerificationToken = this.$el.elements['__RequestVerificationToken'].value;

      const options = {
        method: 'DELETE',
        headers: {
          'RequestVerificationToken': requestVerificationToken
        }
      }

      return await this.executeFetchWithErrorHandling(url, options, errorHandler);
    },


    executeFetchWithErrorHandling: async function (url, options, errorHandler) {
      try {
        this.showModalSpinner();
        this.clearErrors();
        this.isWorking = true;

        const response = await fetch(url, options);

        const contentType = response.headers.get('Content-Type').toLowerCase();
        console.log('Response (' + response.status + '): ' + contentType);

        if (contentType.includes('application/json') || contentType.includes('application/problem+json')) {
          const result = await response.json();

          if (response.ok) {
            return result;
          }
          else if (!response.ok && errorHandler)
            errorHandler(result, contentType);
          else if (!response.ok && !errorHandler) {
            if (contentType.includes('application/problem+json'))
              this.problemJsonErrorHandler(result);
            else
              this.cofoundryErrorHandler(result);
          }
        }
        else
          window.alert("Unexpected response type: " + contentType);
      }
      catch (ex) {
        window.alert(ex);
      }
      finally {
        this.hideModalSpinner();
        this.isWorking = false;
      }

      return null;
    },


    // Returned by ASP.NET API validation
    problemJsonErrorHandler: function (result) {
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
      window.scrollTo(0, 0);
    },


    cofoundryErrorHandler: function (result) {
      debugger
      result.errors.forEach(error => {
        this.errors.push(error.message);
        error.properties.forEach(property => {
          if (property) {
            $('#' + property).addClass('is-invalid');
            $('#' + property + '_feedback').text(error.message);
          }
        });
      });

      window.scrollTo(0, 0);
    }
  }
};


FormsEditor = {
  install: function (Vue) {
    Vue.mixin(FormsEditorMixin);
  }
}


Vue.component('two-state-icon', {
  props: ['one', 'two', 'state'],
  //data: function () {
  //  return {
  //    state: 1
  //  }
  //},
  methods:
  {
    iconStateClass: function () {
      if (this.state == 1)
        return this.one;
      else if (this.state == 2)
        return this.two;
    }
  },
  template: '<i v-if="state != null" :class="iconStateClass()"></i>'
  //<button v-on:click="count++">You clicked me {{ count }} times.</button>'
})
