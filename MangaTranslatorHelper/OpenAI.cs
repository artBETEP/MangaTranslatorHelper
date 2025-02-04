using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MangaTranslatorHelper
{
    public class OpenAI
    {
        private readonly string _apiKey;
        private readonly string _endpoint = "https://api.openai.com/v1/chat/completions";
        private const string gptModel = "gpt-4o-mini";



        public OpenAI(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<string> SendMessageAsync(string userMessage, string model = gptModel, int maxTokens = 150, double temperature = 0.7)
        {
            var requestBody = new
            {
                model = model,
                messages = new[]
                {
                new { role = "system", content = "You are a helpful assistant." },
                new { role = "user", content = userMessage }
            },
                max_tokens = maxTokens,
                temperature = temperature
            };

            string json = JsonSerializer.Serialize(requestBody);

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                    var response = await client.PostAsync(_endpoint, new StringContent(json, Encoding.UTF8, "application/json"));

                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var chatResponse = JsonSerializer.Deserialize<ChatResponse>(responseContent, options);

                        return chatResponse?.Choices[0].Message.Content ?? "No reply from model.";
                    }
                    else
                    {
                        return $"Error: {response.StatusCode}\n{responseContent}";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }

        private class ChatResponse
        {
            public Choice[] Choices { get; set; }
        }

        private class Choice
        {
            public Message Message { get; set; }
        }

        private class Message
        {
            public string Role { get; set; }
            public string Content { get; set; }
        }

        public async Task<string> SendRequestWithImageAsync(Bitmap picture, string prompt)
        {
            try
            {
                var converter = new ImageConverter();
                var imageBytes = (byte[])converter.ConvertTo(picture, typeof(byte[]));

                var base64Image = Convert.ToBase64String(imageBytes);

                var requestBody = new
                {
                    prompt = prompt,
                    image = base64Image, 
                    model = gptModel
                };

                string json = JsonSerializer.Serialize(requestBody);

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                    var response = await client.PostAsync(_endpoint, new StringContent(json, Encoding.UTF8, "application/json"));

                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return responseContent;
                    }
                    else
                    {
                        return $"Error: {response.StatusCode}\n{responseContent}";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }
    }
}
