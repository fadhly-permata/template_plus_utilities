using System.Runtime.CompilerServices;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace IDC.Utilities.Data.MongoDB;

public static class CursorAndBsonExtensions
{
    public static BsonDocument ToBsonDocument(this JObject? json) =>
        json == null ? [] : BsonDocument.Parse(json.ToString());

    public static JObject ToJObject(this BsonDocument? bson) =>
        bson == null ? [] : JObject.Parse(bson.ToJson());

    public static async Task<List<JObject>?> ToJObject(
        this IAsyncCursor<BsonDocument>? cursor,
        CancellationToken cancellationToken = default
    )
    {
        if (cursor == null)
            return null;

        var results = new List<JObject>();
        while (await cursor.MoveNextAsync(cancellationToken: cancellationToken))
            results.AddRange(
                collection: cursor.Current.Select(selector: static doc =>
                    JObject.Parse(json: doc.ToJson())
                )
            );

        return results;
    }

    public static async IAsyncEnumerable<JObject> ToJObjectAsync(
        this IAsyncCursor<BsonDocument>? cursor,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        if (cursor == null)
            yield break;

        while (await cursor.MoveNextAsync(cancellationToken))
            foreach (var doc in cursor.Current)
                yield return JObject.Parse(doc.ToJson());
    }
}
