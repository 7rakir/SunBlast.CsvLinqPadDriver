using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace CsvLinqPadDriver.Extensions
{
    // ReSharper disable UnusedType.Global
    // ReSharper disable UnusedMember.Global
    // ReSharper disable UnusedMember.Local
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    // ReSharper disable MemberCanBePrivate.Global
    // ReSharper disable MemberCanBePrivate.Local

    public static class CortexExtensions
    {
        public static IEnumerable<CortexDocument> ParseCortex(this IEnumerable<dynamic> documents)
        {
            return documents.Select(CortexDocument.GetCortexDocument);
        }

        public class CortexDocument
        {
            public long ElementId { get; private init; }
            public int DocTypeId { get; private init; }
            public int OwnerPartitionId { get; private init; }
            public DateTime? LastWriteTime { get; private init; }
            public DateTime? DeletedDate { get; private init; }
            public string? ModelType { get; private init; }
            public bool IsObject { get; private init; }
            public dynamic? Data { get; private init; }

            public static CortexDocument GetCortexDocument(dynamic document)
            {
                JsonElement data = JsonSerializer.Deserialize<JsonElement>(document.Data);
                var isObject = data.TryGetProperty(nameof(ModelType), out var modelType);
                return new CortexDocument()
                {
                    ElementId = Int64.Parse(document.ElementId),
                    DocTypeId = Int32.Parse(document.DocTypeId),
                    OwnerPartitionId = Int32.Parse(document.OwnerPartitionId),
                    LastWriteTime = DateTime.TryParse(document.LastWriteTime, out DateTime r) ? r : null,
                    DeletedDate = DateTime.TryParse(document.DeletedDate, out DateTime d) ? d : null,

                    IsObject = isObject,

                    ModelType = isObject ? modelType.ToString()! : data.GetProperty("Type").ToString()!,
                    Data = isObject ? null : new Relation(data)
                };
            }

            private class Relation
            {
                public Relation(JsonElement data)
                {
                    SourceId = Int64.Parse(data.GetProperty("SourceId").ToString()!);
                    TargetId = Int64.Parse(data.GetProperty("TargetId").ToString()!);
                }
                
                public long SourceId { get; }
                public long TargetId { get; }
                
                private object ToDump() => new {SourceId, TargetId};
            }
        }
    }
}