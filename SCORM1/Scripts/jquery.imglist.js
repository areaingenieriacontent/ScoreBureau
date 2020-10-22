(function ($) {

    $.fn.imageList = function (options) {

        var opts = $.extend({}, $.fn.imageList.defaults, options);


        $.getJSON(opts.connector, null, function (result) {
            
            var imgconn = $("<ul>").attr({
                "class": "thumbnails"
            }).appendTo("#" + opts.imgcontainer);

            $.each(result, function (i, field) {

                var imglist = $('<li />').appendTo(imgconn);
                var imglink = $('<a href="#" title="Select" onclick="apply("'+field.name+'", 1)" />').appendTo(imglist);

                $("<img />").attr({
                    id: "image-" + field.name,
                    src: field.url,
                    title: field.name,
                    "class": "thumbnail"
                }).appendTo(imglink);
            });
        });

    };

    $.fn.imageList.defaults = {
        connector: null,
        imgcontainer: ""
    };

}(jQuery));