using System;
using System.Text.Json;

namespace CsvLinqPadDriver.UserExtensions.Dynamic
{
    public class CortexDocument
    {
        public long ElementId { get; private init; }
        public int DocTypeId { get; private init; }
        public int OwnerPartitionId { get; private init; }
        public DateTime? LastWriteTime { get; private init; }
        public DateTime? DeletedDate { get; private init; }
        public string? ModelType { get; private init; }
        public bool IsObject { get; private init; }
        public Relationship? Relation { get; private init; }
        public JsonElement? Properties { get; private init; }

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
                Relation = isObject ? null : new Relationship(data),
                Properties = isObject ? data.GetProperty(nameof(Properties)) : null
            };
        }

        private object ToDump() => new
        {
            ElementId,
            DocTypeId,
            OwnerPartitionId,
            LastWriteTime,
            DeletedDate,
            ModelType,
            IsObject,
            Relation = IsObject ? null : new Lazy<Relationship>(() => Relation!),
            Properties = IsObject ? new Lazy<JsonElement?>(() => Properties) : null
        };

        public class Relationship
        {
            public Relationship(JsonElement data)
            {
                SourceId = Int64.Parse(data.GetProperty(nameof(SourceId)).ToString()!);
                TargetId = Int64.Parse(data.GetProperty(nameof(TargetId)).ToString()!);
            }

            public long SourceId { get; }
            public long TargetId { get; }

            private object ToDump() => new { SourceId, TargetId };
        }
    }
}