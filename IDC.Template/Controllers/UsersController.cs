using IDC.Template.Utilities;
using IDC.Utilities;
using IDC.Utilities.Data;
using IDC.Utilities.Extensions;
using IDC.Utilities.Models.API;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace IDC.Template.Controllers;

[Route("api/[controller]")]
[ApiController]
[Tags(tags: "Demo Mongo")]
public class UsersController : ControllerBase
{
    private readonly MongoRepository<JObject> _repository;
    private readonly Language _language;
    private readonly SystemLogging _systemLogging;
    private const int TimeoutMs = 5000;

    public UsersController(Language language, SystemLogging systemLogging)
    {
        _language = language;
        _systemLogging = systemLogging;

        try
        {
            var mongoHelper = new MongoHelper("mongodb://localhost:27017", "Learning");
            _repository = new MongoRepository<JObject>(mongoHelper, "Users");
        }
        catch (Exception ex)
        {
            _systemLogging.LogError($"Failed to connect to MongoDB: {ex.Message}");
            throw;
        }
    }

    [HttpPost]
    public async Task<APIResponseData<JObject>> Create([FromBody] JObject data)
    {
        try
        {
            var cts = new CancellationTokenSource(TimeoutMs);
            var id = await _repository.InsertOneAsync(
                document: data,
                customId: data.PropGet<string?>(path: "_id", defaultValue: null),
                cancellationToken: cts.Token,
                callback: insertedId =>
                    _systemLogging.LogInformation($"Document inserted with ID: {insertedId}")
            );
            data["_id"] = id;
            return new APIResponseData<JObject>().ChangeData(data: data);
        }
        catch (Exception ex)
        {
            return new APIResponseData<JObject>()
                .ChangeStatus(language: _language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: _systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    [HttpGet]
    public async Task<APIResponseData<List<JObject>>> GetAll()
    {
        try
        {
            var cts = new CancellationTokenSource(TimeoutMs);
            var result = await _repository.FindAsync(
                filter: new JObject(),
                cancellationToken: cts.Token,
                callback: results =>
                    _systemLogging.LogInformation($"Found {results.Count} documents")
            );
            return new APIResponseData<List<JObject>>().ChangeData(data: result);
        }
        catch (Exception ex)
        {
            return new APIResponseData<List<JObject>>()
                .ChangeStatus(language: _language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: _systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    [HttpGet("{id}")]
    public async Task<APIResponseData<JObject?>> GetById(string id)
    {
        try
        {
            var cts = new CancellationTokenSource(TimeoutMs);
            var filter = new JObject { ["_id"] = id };
            var result = await _repository.FindOneAsync(
                filter: filter,
                cancellationToken: cts.Token,
                callback: doc =>
                    _systemLogging.LogInformation(
                        $"Document {(doc != null ? "found" : "not found")}"
                    )
            );
            return new APIResponseData<JObject?>().ChangeData(data: result);
        }
        catch (Exception ex)
        {
            return new APIResponseData<JObject?>()
                .ChangeStatus(language: _language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: _systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    [HttpPut("{id}")]
    public async Task<APIResponseData<JObject?>> Update(string id, [FromBody] JObject data)
    {
        try
        {
            var cts = new CancellationTokenSource(TimeoutMs);
            var filter = new JObject { ["_id"] = id };
            var update = new JObject { ["$set"] = data };

            await _repository.UpdateOneAsync(
                filter: filter,
                update: update,
                cancellationToken: cts.Token,
                callback: count => _systemLogging.LogInformation($"Updated {count} document(s)")
            );

            var updatedDoc = await _repository.FindOneAsync(
                filter: filter,
                cancellationToken: cts.Token
            );
            return new APIResponseData<JObject?>().ChangeData(data: updatedDoc);
        }
        catch (Exception ex)
        {
            return new APIResponseData<JObject?>()
                .ChangeStatus(language: _language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: _systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }

    [HttpDelete("{id}")]
    public async Task<APIResponseData<bool>> Delete(string id)
    {
        try
        {
            var cts = new CancellationTokenSource(TimeoutMs);
            var filter = new JObject { ["_id"] = id };
            var count = await _repository.DeleteOneAsync(
                filter: filter,
                cancellationToken: cts.Token,
                callback: deletedCount =>
                    _systemLogging.LogInformation($"Deleted {deletedCount} document(s)")
            );
            return new APIResponseData<bool>().ChangeData(data: count > 0);
        }
        catch (Exception ex)
        {
            return new APIResponseData<bool>()
                .ChangeStatus(language: _language, key: "api.status.failed")
                .ChangeMessage(
                    exception: ex,
                    logging: _systemLogging,
                    includeStackTrace: Commons.IsDebugEnvironment()
                );
        }
    }
}
