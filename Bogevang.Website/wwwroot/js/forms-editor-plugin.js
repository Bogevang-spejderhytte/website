FormsEditorMixin =
{
  data: function () {
    debugger
    return {
      isLoading: true,
      errors: []
    }
  },

  methods: {
    getWithErrorHandling: async function (url, errorHandler) {
      const options = {
        method: 'GET'
      }

      return await this.executeFetchWithErrorHandling(url, options, errorHandler);
    },


    postWithErrorHandling: async function (url, body, requestVerificationToken, errorHandler) {
      const jsonBody = JSON.stringify(body);

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


    deletetWithErrorHandling: async function (url, requestVerificationToken, errorHandler) {
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
        this.isLoading = true;

        const response = await fetch(url, options);

        const contentType = response.headers.get('Content-Type').toLowerCase();
        console.log('Response (' + response.status + '): ' + contentType);

        if (contentType.includes('application/json') || contentType.includes('application/problem+json')) {
          const result = await response.json();

          if (response.ok)
            return result;
          else if (!response.ok && errorHandler)
            errorHandler(result);
          else if (!response.ok && !errorHandler)
            this.standardFormsErrorHandler(result);
        }
        else
          window.alert("Unexpected response type: " + contentType);
      }
      catch (ex) {
        window.alert(ex);
      }
      finally {
        this.isLoading = false;
      }

      return null;
    },


    standardFormsErrorHandler: function (result) {
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
    }
  }
};


FormsEditor = {
  install: function (Vue) {
    Vue.mixin(FormsEditorMixin);
  }
}
