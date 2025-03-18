using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace IDC.Utilities.Data;

public sealed partial class MongoHelper
{
    /// <summary>
    /// Converts a JObject to a BsonDocument.
    /// </summary>
    /// <param name="json">The JObject to convert.</param>
    /// <returns>A BsonDocument representation of the input JSON.</returns>
    /// <remarks>
    /// This method is useful when you need to convert JSON data to BSON format for MongoDB operations.
    /// </remarks>
    private static BsonDocument ToBsonDocument(JObject json) => BsonDocument.Parse(json.ToString());

    /// <summary>
    /// Converts a BsonDocument to a JObject.
    /// </summary>
    /// <param name="bson">The BsonDocument to convert.</param>
    /// <returns>A JObject representation of the input BSON.</returns>
    /// <remarks>
    /// This method is helpful when you need to convert BSON data from MongoDB to JSON format for further processing or output.
    /// </remarks>
    private static JObject ToJObject(BsonDocument bson) => JObject.Parse(bson.ToJson());

    /// <summary>
    /// Checks if a collection exists in the database.
    /// </summary>
    /// <param name="collectionName">The name of the collection to check.</param>
    /// <returns>True if the collection exists, false otherwise.</returns>
    /// <remarks>
    /// This method queries the database to check for the existence of a specific collection.
    /// It uses a filter to match the collection name and checks if any results are returned.
    ///
    /// Example:
    /// <code>
    /// if (mongoHelper.CollectionExists("users"))
    /// {
    ///     Console.WriteLine("Users collection exists.");
    /// }
    /// </code>
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown when the method is called on a disposed instance.</exception>
    public bool CollectionExists(string collectionName)
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            var filter = new BsonDocument("name", collectionName);
            var collections = _database.ListCollections(
                new ListCollectionsOptions { Filter = filter }
            );
            return collections.Any();
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Creates a new collection in the database.
    /// </summary>
    /// <param name="collectionName">The name of the collection to create.</param>
    /// <returns>The current MongoHelper instance for method chaining.</returns>
    /// <remarks>
    /// This method creates a new collection in the MongoDB database.
    /// If the collection already exists, MongoDB will ignore the creation request without raising an error.
    ///
    /// Example:
    /// <code>
    /// mongoHelper.CreateCollection("newCollection")
    ///            .InsertDocument("newCollection", new BsonDocument("key", "value"));
    /// </code>
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown when the method is called on a disposed instance.</exception>
    public MongoHelper CreateCollection(string collectionName)
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _database.CreateCollection(collectionName);
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }

    /// <summary>
    /// Drops (deletes) a collection from the database.
    /// </summary>
    /// <param name="collectionName">The name of the collection to drop.</param>
    /// <returns>The current MongoHelper instance for method chaining.</returns>
    /// <remarks>
    /// This method removes the specified collection and all its documents from the database.
    /// Use with caution as this operation is irreversible.
    ///
    /// Example:
    /// <code>
    /// mongoHelper.DropCollection("oldCollection")
    ///            .CreateCollection("newCollection");
    /// </code>
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown when the method is called on a disposed instance.</exception>
    public MongoHelper DropCollection(string collectionName)
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _database.DropCollection(collectionName);
            return this;
        }
        catch (Exception ex)
        {
            _logging?.LogError(exception: ex);
            throw;
        }
    }
}
