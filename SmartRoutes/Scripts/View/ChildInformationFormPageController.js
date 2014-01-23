
// pageID - The id of the form page view.
SmartRoutes.ChildInformationFormPageController = (function(pageID) {

    // Private:
    var childInformationPageID = pageID;
    var maxChildren = 3;
    var childCount = 1;
    var childInfoViewModels = new Array();

    (function Init() {
        // Setup the knockout viewmodel bindings.
        var childInfoViews = $(".sr-child-info-view");
        for (var childInfoIndex = 0; childInfoIndex < childInfoViews.length; ++childInfoIndex) {
            childInfoViewModels[childInfoIndex] = new ChildInfoViewModel();
            ko.applyBindings(childInfoViewModels[childInfoIndex], childInfoViews[childInfoIndex]);
        }
    })();

    // Event handlers.

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
            ++childCount;
        }
    });

    // Setup the collapse button click handler.
    $(".sr-collapse-button").click(function() {
        // This is essentially the reverse of the
        // expansion button handler.

        var nextChildInfoElement = $(this).closest(".sr-child-info-view").next();
        nextChildInfoElement.hide();
        --childCount;

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


    return {
        // Public:

        RunPage: function() {
            $("#" + childInformationPageID).fadeIn(SmartRoutes.Constants.formPageFadeInTime);
        },

        StopPage: function() {
            // Do nothing for now.
        },

        // Indicates if the filled in fields are valid or not.
        IsPageDataValid: function() {
            // This is a tautology.  The name isn't required
            // and can be arbitrary.
            return true;
        },

        // Returns an array of objects containing the name, age, and gender
        // entered for the children.
        GetChildInformation: function() {
            var childInformation = new Array();

            for (var childIndex = 0; childIndex < childCount; ++childIndex) {
                childInformation.push({ name: value.name(), age: value.age(), gender: value.gender() });
            }

            return childInformation;
        },
    };
});