using System;
using System.Collections.Generic;
using System.Linq;

namespace CsvLinqPadDriver.UserExtensions.Dynamic
{
    public static class DynamicExtensions
    {
        public static IEnumerable<T> WhereDelayed<T>(this IEnumerable<dynamic> collection,
            DateTime timeOfGatheringDiagnostics)
        {
            return collection.Where(n =>
                    n.UnManaged == "False" && n.External == "False" &&
                    DateTime.Parse(n.NextPoll).AddSeconds(Int32.Parse(n.PollInterval) * 2) < timeOfGatheringDiagnostics)
                .Cast<T>();
        }

        public static IEnumerable<CortexDocument> ParseCortex(this IEnumerable<dynamic> documents)
        {
            return documents.Select(CortexDocument.GetCortexDocument);
        }
    }
}