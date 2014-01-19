
function ChildInfoViewModel() {

    // Private:


    return {
        // Public
        name: ko.observable(""),
        age: ko.observable(1),
        gender: ko.observable("Male")
    };
};