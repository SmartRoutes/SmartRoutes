
SmartRoutes.MainPageViewController = function(pageID) {
    // Private:

    var mainPageViewID = pageID;

    return {
        // Public:

        RunPage: function() {
            // Do nothing.
        },

        StopPage: function() {
            // Do nothing.
        },

        GetPageViewID: function() {
            return mainPageViewID;
        }
    };
};