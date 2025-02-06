using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Drawing;

public class GeminiImageProcessor
{
    private readonly string _apiKey;
    private readonly string _endpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-001:generateContent";

    public GeminiImageProcessor(string apiKey)
    {
        _apiKey = apiKey;
    }

    public async Task<string> ProcessImageAsync(Image image)
    {
        try
        {
            string base64Image = ConvertImageToBase64(image);

            var lang = "Russian";

            string translatedText = await TranslateTextAsync(lang, ConvertImageToBase64(image));

            return translatedText;
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    public async Task<string> ProcessTextBlocksAsync(Image image)
    {
        try
        {
            string base64Image = ConvertImageToBase64(image);

            string detectedBlocks = await DetectTextBlocksAsync(ConvertImageToBase64(image));

            return detectedBlocks;
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    private string ConvertImageToBase64(Image image)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] imageBytes = ms.ToArray();
            return Convert.ToBase64String(imageBytes);
        }
    }

    private async Task<string> TranslateTextAsync(string lang, string base64Image)
    {
        using (HttpClient client = new HttpClient())
        {
            string requestBody = JsonSerializer.Serialize(new
            {
                model = "gemini-2.0-flash-001", // "gemini-1.5-flash", "gemini-2.0-flash-001",
                contents = new[]
                {
                    new
                    {

                        parts = new object[]
                        {
                            new
                            {
                                text = $"Translate text on image to {lang}. Reply only with translated text."
                            },

                            new
                            {
                                inline_data = new
                                {
                                    mime_type = "image/jpeg",
                                    data = base64Image
                                }
                            }

                        }
                    }
                }
            });

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{_endpoint}?key={_apiKey}")
            {
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };

            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                using JsonDocument doc = JsonDocument.Parse(responseContent);
                var translatedText = doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
                return translatedText;
            }
            else
            {
                return $"API Gemini Error: {responseContent}";
            }
        }
    }

    private async Task<string> DetectTextBlocksAsync(string base64Image)
    {
        using (HttpClient client = new HttpClient())
        {
            string requestBody = JsonSerializer.Serialize(new
            {
                model = "gemini-1.5-flash", // "gemini-1.5-flash", "gemini-2.0-flash",
                contents = new[]
                {
                    new
                    {

                        parts = new object[]
                        {
                            new
                            {
                                text = @"I have upload a comic book page, Find the text blocks on it and return a list of them in the following format:
(x; y; width; height) the text in the block.
Take the upper left corner as the 0,0 coordinate point.
Specify x coordinates and width of the block relative to the image width
Specify y coordinates and block height relative to the image height.
Each block must contain text corresponding to one text bubble or phrase
The text of each block should be one line, without hyphenation.
If there's a hyphenation in the phrase, put the text on one line.
Reply strictly only with blocks info. No addtitonal comments"
                            },

                            new
                            {
                                inline_data = new
                                {
                                    mime_type = "image/jpeg",
                                    data = base64Image
                                }
                            }

                        }
                    }
                }
            });

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{_endpoint}?key={_apiKey}")
            {
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };

            HttpResponseMessage response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                using JsonDocument doc = JsonDocument.Parse(responseContent);
                var translatedText = doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
                return translatedText;
            }
            else
            {
                return $"API Gemini Error: {responseContent}";
            }
        }
    }
}
