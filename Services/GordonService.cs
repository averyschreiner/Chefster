using System.Text;
using System.Text.Json;
using Chefster.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using static System.Net.Mime.MediaTypeNames;

namespace Chefster.Services;

public class GordonService(IHttpClientFactory httpClientFactory)
{
    public readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    public readonly string API_KEY = Environment.GetEnvironmentVariable("GORDON")!;
    public readonly string ASSIST_ID = Environment.GetEnvironmentVariable("ASSIST_ID")!;

    private async Task<List<string?>> CreateThreadAndRun()
    {
        var httpClient = _httpClientFactory.CreateClient();

        var id = new { assistant_id = ASSIST_ID };

        // create the content for the request
        var assistantId = new StringContent(
            JsonSerializer.Serialize(id),
            Encoding.UTF8,
            Application.Json
        );

        // assign headers
        httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");
        httpClient.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Bearer {API_KEY}");

        var threadAndRun = await httpClient.PostAsync(
            $"https://api.openai.com/v1/threads/runs",
            assistantId
        );

        // grab content, parse, and return the run and thread ids
        var content = await threadAndRun.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content).RootElement;

        var threadId = json.GetProperty("thread_id").GetString();
        var runId = json.GetProperty("id").GetString();

        if (threadId != null && runId != null)
        {
            return [runId, threadId];
        }
        else
        {
            return [];
        }
    }

    // It works without creating a message???

    private async Task CreateMessage(string threadId, StringContent? bodyContent)
    {
        var httpClient = _httpClientFactory.CreateClient();

        httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");
        httpClient.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Bearer {API_KEY}");

        var message = await httpClient.PostAsync(
            $"https://api.openai.com/v1/threads/{threadId}/messages",
            bodyContent
        );

        // make sure we get a successful call before continuing
        //message.EnsureSuccessStatusCode();
    }

    //will return a gordonResponseModel eventually
    public async Task<GordonResponseModel> GetMessageResponse(string considerations)
    {
        var MAX_ATTEMPTS = 10;
        var httpClient = _httpClientFactory.CreateClient();

        httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");
        httpClient.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Bearer {API_KEY}");

        // create run and thread ids and save them
        var threadAndRunIds = await CreateThreadAndRun();
        if (threadAndRunIds.Count != 2)
        {
            return new GordonResponseModel { Response = [], Success = false };
        }

        var ids = new { runId = threadAndRunIds[0]!, threadId = threadAndRunIds[1]! };

        // create request body
        var jsonBody = new
        {
            assistant_id = Environment.GetEnvironmentVariable("GORDON_ID"),
            thread = new { messages = new[] { new { role = "user", content = considerations } } }
        };
        var strContent = new StringContent(
            JsonSerializer.Serialize(jsonBody),
            Encoding.UTF8,
            "application/json"
        );

        //create message
        await CreateMessage(ids.threadId, strContent);

        // loop until we get a response back of complete
        var attempts = 0;
        do
        {
            attempts += 1;

            var successCheck = await httpClient.GetAsync(
                $"https://api.openai.com/v1/threads/{ids.threadId}/runs/{ids.runId}"
            );

            var successCheckContent = await successCheck.Content.ReadAsStringAsync();
            var status = JsonDocument
                .Parse(successCheckContent)
                .RootElement.GetProperty("status")
                .GetString();

            if (status == "completed")
            {
                break;
            }
            Console.WriteLine("Trying again!");
            await Task.Delay(3000); // wait a few seconds before trying again
        } while (attempts != MAX_ATTEMPTS);

        // try to grab the response
        var gordonResponse = await httpClient.GetAsync(
            $"https://api.openai.com/v1/threads/{ids.threadId}/messages"
        );
        var content = await gordonResponse.Content.ReadAsStringAsync();

        // fail out if we couldnt get anything
        if (attempts == MAX_ATTEMPTS || content == null)
        {
            Console.WriteLine($"Failed to retrieve Gordon response");
            return new GordonResponseModel { Response = [], Success = false };
        }

        // get final gordon response
        var json = JsonDocument.Parse(content).RootElement;
        var result = json.GetProperty("data")[0].GetProperty("content")[0].ToString();

        var converted = ConvertGordonResponse(result);
        if (converted.IsNullOrEmpty())
        {
            return new GordonResponseModel { Response = [], Success = false };
        }
        return new GordonResponseModel { Response = converted, Success = false };
    }

    private List<GordonRecipeModel?> ConvertGordonResponse(string response)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        var recipes = new List<GordonRecipeModel?>();
        try
        {
            var json = JsonDocument
                .Parse(response)
                .RootElement.GetProperty("text")
                .GetProperty("value")
                .ToString();
            var recipes_json = new List<string> { json };

            foreach (var r in recipes_json)
            {
                //Console.WriteLine(r);
                var recipe = JsonSerializer.Deserialize<GordonRecipeModel>(r, options);
                recipes.Add(recipe);
            }

            return recipes;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failure: {e}");
            return [];
        }
    }
}
