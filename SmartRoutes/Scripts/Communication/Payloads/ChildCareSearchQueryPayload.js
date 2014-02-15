

// The payload object that represents all the data
// that must be passed to the server to perform
// a child care search.
SmartRoutes.Communication.ChildCareSearchQueryPayload = function(childInformationPayload, scheduleTypePayloadArray, locationsAndTimes, accreditationPayloadArray, serviceTypePayload) {

    return {
        ChildInformation: childInformationPayload,
        ScheduleType: scheduleTypePayloadArray,
        LocationsAndTimes: locationsAndTimes,
        Accreditations: accreditationPayloadArray,
        ServiceTypes: serviceTypePayload
    };
};
