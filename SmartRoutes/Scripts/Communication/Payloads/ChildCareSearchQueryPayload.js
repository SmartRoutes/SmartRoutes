﻿

// The payload object that represents all the data
// that must be passed to the server to perform
// a child care search.
SmartRoutes.Communication.ChildCareSearchQueryPayload = function(childInformationPayload, scheduleTypePayloadArray, accreditationPayloadArray, serviceTypePayload) {

    return {
        childInformation: childInformationPayload,
        scheduleType: scheduleTypePayload,
        accreditations: accreditationPayloadArray,
        serviceTypes: serviceTypePayload
    };
};
