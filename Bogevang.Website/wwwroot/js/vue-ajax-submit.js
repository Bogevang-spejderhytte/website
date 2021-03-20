// https://github.com/saqueib/vue-ajax-submit

Vue.directive('ajax-submit', {
  bind: function (el, binding, vnode) {
    // form element
    var $el = $(el),
      // Submit input button
      submitBtn = $el.closest('form').find(':submit'),
      // Submit input value
      submitBtnText = submitBtn.val(),
      // Loading text, use data-loading-text if found
      loadingText = submitBtn.data('loading-text') || 'Submitting...',
      // Form Method
      method = $el.find('input[name=_method]').val() || $el.prop('method'),
      // Action url for form
      url = $el.prop('action');

    // On form submit handler
    $el.on('submit', function (e) {
      // Prevent default action
      e.preventDefault();

      // Serialize the form data
      var formData = $el.serialize();

      // Disable the button and change the loading text
      submitBtn.val(loadingText);
      submitBtn.prop('disabled', true);

      // make http call using vue-resource
      vnode.context.$http({ url: url, method: method, body: formData })
        .then(function (res) {
          // Re-enable button with old value 
          submitBtn.val(submitBtnText);
          submitBtn.prop('disabled', false);
          removeErrorHighlight();

          // Reset the form
          $el[0].reset();

          // check success handler is present
          if (vnode.data.on && vnode.data.on.success) {
            vnode.data.on.success.fn.call(this, res);
          } else {
            // run default handler 
            responseSuccessHandler(res);
          }
        }, function (err) {
          // Re-enable button with old value 
          submitBtn.val(submitBtnText);
          submitBtn.prop('disabled', false);

          // check error handler is present
          if (vnode.data.on && vnode.data.on.error) {
            vnode.data.on.error.fn.call(this, err);
          } else {
            // run default handler
            responseErrorHandler(err);
          }
        });
    });
  }
});