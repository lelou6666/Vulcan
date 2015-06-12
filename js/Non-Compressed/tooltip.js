/*ToolTip  */

(function($){
    $.fn.tooltip = function(options){
        var tipwrap = $("#tooltipWrap"),
            tipcontent = "",
            tipon = false,
            tipheight = 0,
            offset = { x:-13, y:-14 },      // offset the tip from mouse position
            target = { x:0, y:0 };
 
        return this.each(function(){
            var $this = $(this),
                title = $this[0].title,
                timer;
 
            // make sure the current element has a title, else move on to the next one
            if (title === "") return true;
 
            // clear the title attribute so the native tooltip doesn"t popup
            $this.attr("title","");
 
            $this.on("mousemove", function(e){
                // this function is called if mouse has stopped moving (see below)
                var onmousestop = function(){
                    // flag the tooltip as on
                    tipon = true;
 
                    // this is the tooltip container, we only need to create it the first time
                    // so we check if one already exists, if not we create it
                    if (!tipwrap.length) {
                        tipwrap = $("<div/>", {
                            id: "tooltipWrap",
                            class: "tooltip-wrap",
                            html: "<div class=\"tip-content\"></div><div class=\"tip-point tip-point-bottom\"></div>"
                        })
                        .hide().appendTo( $("body") );
 
                        // we also only need to get the handle for the content once
                        tipcontent = tipwrap.find(".tip-content");
                    }
 
                    // put the title into the tooltip content
                    // this is separate from the creation above because this needs to be done on every hover
                    tipcontent.text(title);
 
                    // we need the tooltip height (w/ content) to reposition it above the mouse pointer
                    tipheight = tipwrap.height();
 
                    // calculate the target coordinates (just makes the code neater)
                    target.x = e.pageX + offset.x;
                    target.y = e.pageY - tipheight + offset.y;
 
                    // position the element a bit off so we can slide it into view
                    tipwrap.css({ top:target.y - 15, left:target.x });
 
                    // slide and fade the tooltip into view
                    tipwrap.animate({ opacity:"show", top:target.y}, 250);
                };
 
                // run onmousestop after .4 seconds, UNLESS it is cleared by mouse move
                clearTimeout(timer);
                if (!tipon) timer = setTimeout(onmousestop, 400);
            })
 
            .on("mouseleave", function(e){
                // we only need to hide the tooltip if it was actually on
                if (tipon) tipwrap.animate({ opacity:"hide", top:target.y - 15}, 250);
                // the flag and the time should always be reset to be sure
                tipon = false;
                clearTimeout(timer);
            });
        });
    };  // end fn.tooltip
})(jQuery);