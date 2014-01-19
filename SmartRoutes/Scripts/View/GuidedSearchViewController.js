/* requires(SmartRoutes.js) */

SmartRoutes.guidedSearchViewController = (function() {

    // Private:
    var childCount = 1;

    var that = this;


    (function Init() {

        // Setup the expansion button click handler.
        $(".sr-expansion-button").click(function() {
            // So, this callback is hit for every expansion button.
            // What must happen is
            // 1) Show the next section (if it exists).
            // 2) Change the expansion button to a collapse button
            // the current child info section.

            var nextChildInfoElement = $(this).closest(".sr-child-info-view").next();
            if (nextChildInfoElement.length > 0) {
                $(".sr-expansion-button").hide();
                $(".sr-collapse-button").hide();

                $(this).next("button", ".sr-collapse-button").show();
                $(".sr-expansion-button", nextChildInfoElement).show();

                nextChildInfoElement.show();
                ++that.childCount;
            }



        });

        // Setup the collapse button click handler.
        $(".sr-collapse-button").click(function() {
            // This is essentially the reverse of the
            // expansion button handler.

            var nextChildInfoElement = $(this).closest(".sr-child-info-view").next();
            nextChildInfoElement.hide();
            --that.childCount;

            // Hide the buttons.
            $(".sr-expansion-button").hide();
            $(".sr-collapse-button").hide();

            // Show the expansion button on the current child info view.
            $(this).prev("button", ".sr-expand-button").show();

            // Look at the previous child info view and show its
            // collapse button if the view exists.
            var previousChildInfoElement = $(this).closest(".sr-child-info-view").prev();
            if (previousChildInfoElement.length > 0) {
                $(".sr-collapse-button", previousChildInfoElement).show();
            }
        });
    })();


    return {
        // Public:

    };
})();