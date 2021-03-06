﻿
// pageID - The id of the form page view.
SmartRoutes.ChildInformationFormPageController = (function(pageID) {

    // Private:
    var childInformationFormPageID = pageID;
    var maxChildren = 3;
    var childCount = 1;
    var childInfoViewModels = new Array();
    var validationCallback = null;
    var pageVisitedKey = "childInfoPageVisited";

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

        // Signals that this is now the active form page.
        RunPage: function(pageValidationCallback) {
            validationCallback = pageValidationCallback;

            // Just call the validation callback here since
            // this page can't be invalid.
            validationCallback(true);

            window.sessionStorage.setItem(pageVisitedKey, true);
        },

        // Signals that this is not the active form page.
        StopPage: function() {
            // Do nothing for now.
        },

        // Indicates if the filled in fields are valid or not.
        IsPageValid: function() {
            // This is a tautology.  The name isn't required
            // and can be arbitrary.
            return true;
        },

        GetPageVisited: function() {
            var visitedString = window.sessionStorage.getItem(pageVisitedKey);
            var visited = false;
            if (visitedString === "true") {
                visited = true;
            }

            return visited;
        },

        // Gets the ID for the form page element.
        GetFormPageID: function() {
            return childInformationFormPageID;
        },

        // Returns an array of objects containing the name, age, and gender
        // entered for the children.
        GetChildInformationPayloads: function() {
            var childInformation = new Array();

            for (var childIndex = 0; childIndex < childCount; ++childIndex) {
                var childViewModel = childInfoViewModels[childIndex];
                childInformation.push(new SmartRoutes.Communication.ChildInformationPayload(childViewModel.ageGroup()));
            }

            return childInformation;
        },

        GetChildNames: function() {
            var childNames = new Array();

            $.each(childInfoViewModels, function(index, model) {
                if (model.name() !== "") {
                    childNames.push(model.name());
                }
                else {
                    childNames.push("Child " + (index + 1).toString());
                }
            });

            return childNames;
        },
    };
});