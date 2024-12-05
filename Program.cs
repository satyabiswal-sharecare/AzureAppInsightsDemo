using System.Text;
using Newtonsoft.Json;

class Program
{
    static async Task Main(string[] args)
    {
        string instrumentationKey = ""; //Pleaase provide instrument key
        string apiUrl = "https://dc.services.visualstudio.com/v2/track";
        Console.WriteLine("Please enter 1 for Message and 2 for Exception");
        var inputData = Console.ReadLine();
        if (inputData == "1")
        {
            // Create telemetry item
            var telemetryItem = new
            {
                name = $"Microsoft.ApplicationInsights.{instrumentationKey}.Message",
                time = DateTime.UtcNow.ToString("o"),
                iKey = instrumentationKey,

                tags = new
                {
                    ai_cloud_role = "MyApplication",
                    ai_operation_id = Guid.NewGuid().ToString(),

                },
                data = new
                {
                    baseType = "MessageData",
                    baseData = new
                    {
                        message = $"This is a test error from API from VSCode {DateTime.UtcNow.ToString("O")}",
                        severityLevel = 3, // 0: Verbose, 1: Information, 2: Warning, 3: Error, 4: Critical

                        properties = new
                        {
                            app = "testapp",
                            RequestResponseId = "12356"
                        }

                    }
                }

            };
            Console.WriteLine(telemetryItem.ToString());
            await SendTelemetry(apiUrl, telemetryItem);
        }
        else
        {
            var payload = new
            {
                name = "Microsoft.ApplicationInsights.Exception",
                time = DateTime.UtcNow.ToString("o"),
                iKey = instrumentationKey,
                data = new
                {
                    baseType = "ExceptionData",
                    baseData = new
                    {
                        ver = 2,
                        exceptions = new[]
                               {
                                    new
                                    {
                                        typeName = "System.Exception",
                                        message = "An error occurred",
                                        hasFullStack = true,
                                        stack = "Example stack trace here"
                                    }
                                },
                        severityLevel = 3,
                        properties = new
                        {
                            CustomProperty = "CustomValue",
                            RequestResponseId = "12356"
                        }
                    }
                }
            };
            Console.WriteLine(payload.ToString());
            await SendTelemetry(apiUrl, payload);
        }
    }

    static async Task SendTelemetry(string url, object telemetryItem)
    {
        using (var client = new HttpClient())
        {
            string json = JsonConvert.SerializeObject(telemetryItem);
            Console.WriteLine(json);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Telemetry logged successfully.");
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed to log telemetry: {response.StatusCode} - {error}");
            }
        }
    }
}
