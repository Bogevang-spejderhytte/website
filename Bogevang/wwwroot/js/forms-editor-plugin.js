FormsEditorMixin =
{
  data: function () {
    if (!FormsEditorMixin.editorModalOverlay) {
      FormsEditorMixin.editorModalOverlay = new bootstrap.Modal(document.getElementById('loaderOverlay'), {
        backdrop: "static", //remove ability to close modal with click
        keyboard: false, //remove option to close with keyboard
      });
    }

    return {
      datePickerConfig: { timeAdjust: "00:00:00" },
      isWorking: true,
      isEditing: false,
      errors: [],
      notifications: []
    }
  },

  methods: {
    clearErrors: function () {
      this.errors = [];
      this.warnings = [];
      $('.editable').removeClass('is-invalid');
    },


    openEditableInputs: function() {
      this.clearErrors();
      this.isEditing = true;
      $('.editable').prop('readonly', false);
      $('input.editable').prop('disabled', false);
      $('select.editable').prop('disabled', false);
    },


    closeEditableInputs: function () {
      this.clearErrors();
      this.isEditing = false;
      $('.editable').prop('readonly', true);
      $('input.editable').prop('disabled', true);
      $('select.editable').prop('disabled', true);
    },


    clearValidation: function (e) {
      $(e.srcElement).removeClass('is-invalid');
    },


    showModalSpinner: function () {
      FormsEditorMixin.editorModalOverlay.show();
    },


    hideModalSpinner: function () {
      FormsEditorMixin.editorModalOverlay.hide();
    },


    truncateHoursFromDate(d) {
      return d == null
        ? null
        : new Date(d.setHours(0, 0, 0))
    },


    datestr2localTimestamp(s) {
      var d = new Date(s);
      const offset = d.getTimezoneOffset();
      d = new Date(d.getTime() - (offset * 60 * 1000));
      parts = d.toISOString().split('T');
      return parts[0] + ' ' + parts[1].substring(0, 8);
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
});


Vue.component('html-editor', {
  props: ['id', 'height'],
  data() {
    return {
      text: ""
    };
  },
  mounted() {
    this.text = this.getSlotValue();
    tinymce.init({
      selector: '#' + this.id,
      height: this.height
    });
  },
  methods: {
    // <slot></slot> does not work with <textarea>, so use them a bit different.
    getSlotValue() {
      if (this.$slots.default && this.$slots.default.length) {
        return this.$slots.default[0].text;
      }
      return '';
    }
  },
  template: '<textarea :id="id" v-model="text"></textarea>'
});
