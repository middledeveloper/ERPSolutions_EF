$(function () {
    $(".comments-content").hide();
    $(".instructions-content").hide();
    $(".fulldesc-content").hide();

    comments();
    instructions();
    fulldesc();

    if ($('#move-here').length > 0) {
        if ($('.field-validation-error').length > 0) {
            $([document.documentElement, document.body]).animate({
                scrollTop: $("#move-here").offset().top
            }, 2000);
        };
    };

});

function comments() {
    $('.comments-icon').on('click', function () {
        var ticket_id = $(this).attr('comment-id');
        var comments_block = '#comments_' + ticket_id;

        if ($(comments_block).is(":visible")) {
            $(comments_block).hide();
        } else {
            $(comments_block).show();
        }
    });
};

function instructions() {
    $('.instructions-icon').on('click', function () {
        var ticket_id = $(this).attr('instruction-id');
        var instructions_block = '#instructions_' + ticket_id;

        if ($(instructions_block).is(":visible")) {
            $(instructions_block).hide();
        } else {
            $(instructions_block).show();
        }
    });
};

function fulldesc() {
    $('.fulldesc-icon').on('click', function () {
        var ticket_id = $(this).attr('fulldesc-id');
        var fulldesc_block = '#fulldesc_' + ticket_id;

        if ($(fulldesc_block).is(":visible")) {
            $(fulldesc_block).hide();
        } else {
            $(fulldesc_block).show();
        }
    });
};