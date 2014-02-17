using System;

namespace SmartRoutes.Model
{
    public interface IDestination : ILocation
    {
        string Name { get; }

        bool SundayReported { get; }
        DateTime? SundayBegin { get; }
        DateTime? SundayEnd { get; }

        bool MondayReported { get; }
        DateTime? MondayEnd { get; }
        DateTime? MondayBegin { get; }

        bool TuesdayReported { get; }
        DateTime? TuesdayBegin { get; }
        DateTime? TuesdayEnd { get; }

        bool WednesdayReported { get; }
        DateTime? WednesdayEnd { get; }
        DateTime? WednesdayBegin { get; }

        bool ThursdayReported { get; }
        DateTime? ThursdayBegin { get; }
        DateTime? ThursdayEnd { get; }

        bool FridayReported { get; }
        DateTime? FridayBegin { get; }
        DateTime? FridayEnd { get; }

        bool SaturdayReported { get; }
        DateTime? SaturdayBegin { get; }
        DateTime? SaturdayEnd { get; }
    }
}