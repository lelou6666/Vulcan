// Checkbox CSS
// http://www.aspdotnet-suresh.com/2011/12/jquery-custom-styles-for-radio-button.html
$(function() {
    $('#checkboxes').jqTransform({ imgPath: '/js/jqtransformplugin/img/' });
});

function loadModels() {
    var series = getParameterByName("series");

    if (series != "") {
        $('.prod_item').each(function(i) {
            $(this).show();
        });
    }
    else {
        $('.prod_item').each(function(i) {
            $(this).delay(200 * i).fadeIn(500);
        });
    }
}

function showFilter1() {
    $("#checkbox1").slideToggle("fast");

    if ($("#expand1").attr("src") == "/images/plus.jpg") {
        $("#expand1").attr("src", "/images/minus.jpg");
    }
    else {
        $("#expand1").attr("src", "/images/plus.jpg");
    }
}

function showFilter2() {
    $("#checkbox2").slideToggle("fast");

    if ($("#expand2").attr("src") == "/images/plus.jpg") {
        $("#expand2").attr("src", "/images/minus.jpg");
    }
    else {
        $("#expand2").attr("src", "/images/plus.jpg");
    }
}


function showFilter3() {
    $("#checkbox3").slideToggle("fast");

    if ($("#expand3").attr("src") == "/images/plus.jpg") {
        $("#expand3").attr("src", "/images/minus.jpg");
    }
    else {
        $("#expand3").attr("src", "/images/plus.jpg");
    }
}

$(document).ready(function() {
    if ($(window).width() < 990) {
        $('#filter_title1').click(function() {
            showFilter1();
        });
        $('#filter_title2').click(function() {
            showFilter2();
        });
        $('#filter_title3').click(function() {
            showFilter3();
        });
    }
});

function beforeAsyncPostBack() {
    $("#content").children(':not(.checkbox_content)').fadeTo("fast", 0.4, function() {
        // Animation complete.
    });
}

function afterAsyncPostBack() {
    $("#content").children(':not(.checkbox_content)').fadeTo("fast", 1, function() {
        // Animation complete.
    });
}

Sys.Application.add_init(appl_init);

function appl_init() {
    var pgRegMgr = Sys.WebForms.PageRequestManager.getInstance();
    pgRegMgr.add_beginRequest(BeginHandler);
    pgRegMgr.add_endRequest(EndHandler);
}

function BeginHandler() {
    beforeAsyncPostBack();
}

function EndHandler() {
    afterAsyncPostBack();
}

$(document).ready(function() {
    ScrollToSeries();
});

function ScrollToSeries() {
    var series = getParameterByName("series");

    if (series != "") {
        var seriesName = "#" + series;
        var offset;
        if ($(window).width() > 900) {
            offset = 190;
        }
        else {
            offset = 70;
        }

        $("html, body").animate({ scrollTop: $(seriesName).offset().top - offset }, 300);
    }
}