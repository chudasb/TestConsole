using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

namespace PSS.ExtesionsMethod
{
    public static class HttpClientExtensions
    {
        public static async Task<string> PostAndCheckStatusAsync(this HttpClient client, string postUrl, string statusUrl, object postBody, TimeSpan interval, TimeSpan duration)
        {
            var cancellationTokenSource = new CancellationTokenSource(duration);
            var cancellationToken = cancellationTokenSource.Token;

            try
            {
                // Perform the POST request
                var content = new StringContent(JsonConvert.SerializeObject(postBody), Encoding.UTF8, "application/json");
                HttpResponseMessage postResponse = await client.PostAsync(postUrl, content, cancellationToken);
                postResponse.EnsureSuccessStatusCode();

                // Extract session_id from the response
                var postResponseBody = await postResponse.Content.ReadAsStringAsync();
                var sessionId = JsonConvert.DeserializeObject<dynamic>(postResponseBody).session_id;

                // Check the session status periodically
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        HttpResponseMessage statusResponse = await client.GetAsync($"{statusUrl}?session_id={sessionId}", cancellationToken);
                        statusResponse.EnsureSuccessStatusCode();
                        var statusResponseBody = await statusResponse.Content.ReadAsStringAsync();
                        var status = JsonConvert.DeserializeObject<dynamic>(statusResponseBody).status;

                        if (status != "NOT_FOUND")
                        {
                            return statusResponseBody;
                        }
                    }
                    catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
                    {
                        // If the task was canceled due to the interval, continue to the next iteration
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while checking status: {ex.Message}");
                    }

                    await Task.Delay(interval, cancellationToken);
                }

                throw new TimeoutException("The operation has timed out.");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
                throw;
            }
            catch (TaskCanceledException ex) when (cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Operation was canceled.");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw;
            }
        }
    }
}
