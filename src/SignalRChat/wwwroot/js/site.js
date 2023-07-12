// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": false,
    "progressBar": true,
    "positionClass": "toast-bottom-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};

window.addEventListener('offline', () => {
    toastr.info('No Internet Connection', 'info');
});

window.addEventListener('online', () => {
    toastr.info('Internet Connected', 'info');
});


function checkDupplicateValue(el) {
    debugger;
    let newSelectedValue = $(el).val();
    console.log(newSelectedValue);
    let newSelectedText = $(el).find('option:selected').text();
    let newSelectedIdAttr = $(el).attr('id');

    const dropdownArray = document.getElementsByClassName('itemClass');

    let length = dropdownArray.length;
    for (let i = 0; i < length; i++) {
        let oldSelectedValue = dropdownArray[i].value;
        let oldSelectedIdAttr = dropdownArray[i].getAttribute('id');
        if (oldSelectedIdAttr != newSelectedIdAttr && oldSelectedValue == newSelectedValue) {
            //el.selectedIndex = 0;
            $(el).val(1).trigger('change.select2');
            toastr.info(`${newSelectedText} already selected`, 'info');
            return;
        }
    }
}