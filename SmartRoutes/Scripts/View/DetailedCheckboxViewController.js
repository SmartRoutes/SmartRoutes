
SmartRoutes.DetailedCheckboxViewController = function(detailedCheckboxView, descriptionVisibilityChangedCallback) {
    // Private:

    var detailedTextClass = "sr-detailed-checkbox-description-container";
    var descriptionVisible = $(detailedCheckboxView).children("." + detailedTextClass).is(":visible");

    function DetailedCheckboxExpanderClicked() {
        // TODO: look into how to make this a nice animation.
        if (descriptionVisible) {
            $(detailedCheckboxView).children("." + detailedTextClass).hide();
        }
        else {
            $(detailedCheckboxView).children("." + detailedTextClass).show();
        }

        descriptionVisible = !descriptionVisible;

        if (descriptionVisibilityChangedCallback) {
            descriptionVisibilityChangedCallback(detailedCheckboxView, descriptionVisible);
        }
    };

    (function Init() {
        $(detailedCheckboxView).children(".sr-detailed-checkbox-expander").click(DetailedCheckboxExpanderClicked);
    })();
    
    return {
        // Public:

        // Adds extra content to the description area that is shown/hidden
        // with the description.
        SetExtraContent: function(contentHtmlString) {
            $(".sr-detailed-checkbox-extra-content-container", detailedCheckboxView).empty();
            $(".sr-detailed-checkbox-extra-content-container", detailedCheckboxView).append(contentHtmlString);
        }
    }
};