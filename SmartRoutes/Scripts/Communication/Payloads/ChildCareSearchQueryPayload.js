

// The payload object that represents all the data
// that must be passed to the server to perform
// a child care search.
SmartRoutes.Communication.ChildCareSearchQueryPayload = function(childInformationPayload, scheduleTypePayloadArray, accreditationPayloadArray, serviceTypePayload) {

    return {
        ChildInformation: childInformationPayload,
        ScheduleType: scheduleTypePayloadArray,
        Accreditations: accreditationPayloadArray,
        ServiceTypes: serviceTypePayload
    };
};
