using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace TestingMRZScannerLibrary
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            //MRZScannerLibrary.MRZScannerLibrary test = new MRZScannerLibrary.MRZScannerLibrary();

            //Step1 add task passport
            var resp = step1();
            var xmlString = resp.Result.Content.ReadAsStringAsync().Result;
            Console.WriteLine(xmlString + "\n");

            //Spet2 get task link if ready
            XmlDocument xDoc1 = new XmlDocument();
            xDoc1.LoadXml(xmlString);

            //TODO FIX THIS
            var id = xDoc1.LastChild.FirstChild.Attributes.GetNamedItem("id").InnerText;

            var resp2 = step2("https://cloud.ocrsdk.com/getTaskStatus?taskId=", id);
            var xmlString2 = resp2.Result.Content.ReadAsStringAsync().Result;
            Console.WriteLine(xmlString2 + "\n");

            //Step3 get info on task
            XmlDocument xDoc2 = new XmlDocument();
            xDoc2.LoadXml(xmlString2);

            //TODO FIX THIS
            var link = xDoc2.LastChild.FirstChild.Attributes.GetNamedItem("resultUrl").InnerText;
            
            var resp3 = step3(link);
            Console.WriteLine(resp3.Result.Content.ReadAsStringAsync().Result + "\n");

            //Step4 init library
            */

            MRZScannerLibrary.MRZScannerLibrary test = 
                new MRZScannerLibrary.MRZScannerLibrary("My_app_testProcessMRZ", "Y5J5xYPvDyVRBGR47dfh76kc");

            byte[] image = File.ReadAllBytes("C:\\Users\\yubarbeu\\Desktop\\passports\\2.jpg");

            test.StartScan(image);

            while(true)
            {
                Console.WriteLine(test.isScanCompleted + " - \n");

                if(test.isScanCompleted)
                {
                    Console.WriteLine("ExpMsg = " + test.ExeptionMessage);
                    Console.WriteLine("isCompl = " + test.isScanCompleted);
                    Console.WriteLine("isSuccCompl = " + test.isScanSuccessCompleted);
                    Console.WriteLine("==========");
                    Console.WriteLine("DocNumber = " + test.documentData.DocumentNumber);
                    Console.WriteLine("BirthDate = " + test.documentData.BirthDate);
                    break;
                }
                Thread.Sleep(1000);
            }
        }

        public void CommitPostProcessing()
        {
            //Some code
        }

        public static async Task<HttpResponseMessage> step1()
        {
            byte[] image = File.ReadAllBytes("C:\\Users\\yubarbeu\\Desktop\\passport_rf.jpg");

            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"My_app_testProcessMRZ:Y5J5xYPvDyVRBGR47dfh76kc")));

            var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
            content.Add(new StreamContent(new MemoryStream(image)), "passport", "passport.bmp");

            return await client.PostAsync("https://cloud.ocrsdk.com/processMRZ", content);
            
        }

        /// <summary>
        /// if 'status' ready go to step3
        /// </summary>
        /// <param name="url"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> step2(string url, string id)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"My_app_testProcessMRZ:Y5J5xYPvDyVRBGR47dfh76kc")));

            return await client.GetAsync(url + id);
        }

        public static async Task<HttpResponseMessage> step3(string url)
        {
            var client = new HttpClient();
            return await client.GetAsync(url);
        }
    }
}
