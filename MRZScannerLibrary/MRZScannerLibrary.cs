using MRZScannerLibrary.Models;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MRZScannerLibrary
{
    public class MRZScannerLibrary
    {
        /// Application name
        private string ApplicationName;

        /// Application password
        private string ApplicationPassword;

        /// <summary>
        /// Указывает завершено ли сканирование документа
        /// </summary>        
        public bool isScanCompleted { get; private set; }

        /// <summary>
        /// Указывает успешно ли завершено сканирование документа
        /// </summary>
        public bool isScanSuccessCompleted { get; private set; }

        /// <summary>
        /// Данные сканированные с документа
        /// </summary>
        public DocumentData documentData { get; private set; }

        /// <summary>
        /// В случае неудачного сканирования документа сообщение об ошибке
        /// </summary>
        public string ExeptionMessage { get; private set; }

        public MRZScannerLibrary(string ApplicationName, string ApplicationPassword)
        {
            documentData = new DocumentData();
            isScanCompleted = false;
            isScanSuccessCompleted = false;
            this.ApplicationName = ApplicationName;
            this.ApplicationPassword = ApplicationPassword;
        }

        /// <summary>
        /// Запустить сканирование
        /// </summary>
        /// <param name="image">Скан паспорта</param>
        public async void StartScan(byte[] image, Action callback)
        {
            isScanCompleted = false;
            isScanSuccessCompleted = false;
            ExeptionMessage = "";

            await Task.Run(() => ProcessScan(image));

            if (callback != null)
            {
                callback.Invoke();
            }
        }

        private void ProcessScan(byte[] image)
        {
            var responceRegistrationTask = ApiHttpClient.SendScannPassportToServer(image, GetConcatAuthData());

            XmlDocument registrationTaskXml = new XmlDocument();
            try
            {
                switch(responceRegistrationTask.Result.StatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                        registrationTaskXml.LoadXml(responceRegistrationTask.Result.Content.ReadAsStringAsync().Result);
                        break;
                    default:
                        isScanCompleted = true;
                        isScanSuccessCompleted = false;
                        this.ExeptionMessage = responceRegistrationTask.Result.ReasonPhrase.ToString();
                        return;
                }
            }
            catch (Exception ex)
            {
                this.ExeptionMessage = responceRegistrationTask.Result.ReasonPhrase;
                isScanCompleted = true;
                isScanSuccessCompleted = false;
                return;
            }

            var idTask = registrationTaskXml.LastChild.FirstChild.Attributes.GetNamedItem("id").InnerText;

            XmlDocument statusTaskXml = new XmlDocument();
            string linkTask, xmlString;

            while (true)
            {
                var responceStatusTask = ApiHttpClient.GetLinkTask(idTask, GetConcatAuthData());
                xmlString = responceStatusTask.Result.Content.ReadAsStringAsync().Result;

                statusTaskXml.LoadXml(xmlString);

                var status = statusTaskXml.LastChild.FirstChild.Attributes.GetNamedItem("status").InnerText;

                if (status == "Completed")
                {
                    linkTask = statusTaskXml.LastChild.FirstChild.Attributes.GetNamedItem("resultUrl").InnerText;
                    break;
                }

                Thread.Sleep(500);
            }

            var responceDocumentData = ApiHttpClient.GetDocumentData(linkTask);
            XmlDocument documentDataXml = new XmlDocument();
            documentDataXml.LoadXml(responceDocumentData.Result.Content.ReadAsStringAsync().Result);

            MemoryStream xmlStream = new MemoryStream();
            documentDataXml.Save(xmlStream);

            xmlStream.Flush();
            xmlStream.Position = 0;

            XmlSerializer serializer = new XmlSerializer(typeof(document));
            document _documentData = (document)serializer.Deserialize(xmlStream);

            documentData = new DocumentData();

            if (_documentData.field != null)
            {
                foreach (documentField field in _documentData.field)
                {
                    PropertyInfo propertyInfo = documentData.GetType().GetProperty(field.type);
                    propertyInfo.SetValue(documentData, field.value, null);
                }

                setDateOfBirth(documentData.BirthDate);
                setSerialDocNumber(documentData.DocumentNumber);
                setDocNumber(documentData.DocumentNumber);

                isScanSuccessCompleted = true;
            }
            else
            {
                isScanSuccessCompleted = false;
            }

            isScanCompleted = true;
        }

        private void setDateOfBirth(string dateOfBirthStr)
        {
            if (dateOfBirthStr == "")
                return;

            this.documentData.DateOfBirth.Year = dateOfBirthStr.Substring(0, 2);
            this.documentData.DateOfBirth.Month = dateOfBirthStr.Substring(2, 2);
            this.documentData.DateOfBirth.Day = dateOfBirthStr.Substring(4, 2);
        }

        private void setSerialDocNumber(string docNumber)
        {
            if (docNumber == "")
                return;

            switch(documentData.Nationality)
            {
                case "RUS":
                    this.documentData.PassportNumber = docNumber.Substring(3);
                    break;
                default:
                    this.documentData.PassportNumber = docNumber;
                    break;
            }
        }

        private void setDocNumber(string docNumber)
        {
            if (docNumber == "")
                return;

            this.documentData.SeriesPassportNumber = docNumber.Substring(0, 3);

            if (this.documentData != null && this.documentData.PersonalNumber != null)
                this.documentData.SeriesPassportNumber += this.documentData.PersonalNumber.Substring(0, 1);
        }

        private string GetConcatAuthData()
        {
            return String.Format("{0}:{1}", ApplicationName, ApplicationPassword);
        }
    }
}
