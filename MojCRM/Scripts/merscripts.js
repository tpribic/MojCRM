$(function () {
    $(".date-picker").datepicker({
        language: 'hr',
        weekStart: 1,
        format: "dd.mm.yyyy.",
        autoclose: true,
        orientation: "auto top",
    });
    $("#btn-reset").click(function (e) {
        $(document).find('input[type=text]').attr('value', '');
        $(document).find("select option").attr('selected', false);
    });
})