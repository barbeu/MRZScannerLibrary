using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MRZScannerLibrary
{
    class ApiHttpClient
    {
        /// Http client
        public static HttpClient _httpClient = new HttpClient();

        /// Url main server ABBYY Cloud OCR SDK 
        public static string MainUrlServer = "https://cloud.ocrsdk.com/";

        /// Endpoint processMRZ Method
        public static string ProcessEndpoint = "processMRZ";

        /// Endpoint getTaskStatus Method
        public static string TaskStatusEndpoint = "getTaskStatus?taskId=";

        /// Full processMRZ url
        private static string getProcessMRZ()
        {
            return ApiHttpClient.MainUrlServer + ApiHttpClient.ProcessEndpoint;
        }

        /// Full task status url
        private static string getTaskStatus()
        {
            return ApiHttpClient.MainUrlServer + ApiHttpClient.TaskStatusEndpoint;
        }

        /// <summary>
        /// Шаг 1 - Отправить скан паспорта на сервер для начала обработки
        /// </summary>
        public static async Task<HttpResponseMessage> SendScannPassportToServer(byte[] image, string authData)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authData)));

            var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
            content.Add(new StreamContent(new MemoryStream(image)), "passport", "passport.jpg");

            return await _httpClient.PostAsync(getProcessMRZ(), content);
        }

        /// <summary>
        /// Шаг 2 - Если обработка паспорта завершена успешно, взять ссылку на сканированные данные 
        /// </summary>
        public static async Task<HttpResponseMessage> GetLinkTask(string id, string authData)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authData)));

            return  await _httpClient.GetAsync(ApiHttpClient.getTaskStatus() + id);
        }

        /// <summary>
        /// Шаг 3 - Взять сканированные данные с сервера
        /// </summary>
        public static async Task<HttpResponseMessage> GetDocumentData(string url)
        {
            _httpClient = new HttpClient();
            return await _httpClient.GetAsync(url);
        }
    }
}