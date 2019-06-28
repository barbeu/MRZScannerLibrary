using System;
using System.Collections.Generic;
using System.Text;

namespace MRZScannerLibrary.Models
{
    public class DocumentData
    {
        public string MrzType { get; set; }

        public string Line1 { get; set; }

        public string DocumentType { get; set; }

        public string DocumentSubtype { get; set; }

        public string IssuingCountry { get; set; }

        public string LastName { get; set; }

        public string GivenName { get; set; }

        public string Line2 { get; set; }

        public string DocumentNumber { get; set; }

        public string DocumentNumberVerified { get; set; }

        public string DocumentNumberCheck { get; set; }

        public string Nationality { get; set; }

        public string BirthDate { get; set; }

        public string BirthDateVerified { get; set; }

        public string BirthDateCheck { get; set; }

        public string Sex { get; set; }

        public string ExpiryDate { get; set; }

        public string ExpiryDateVerified { get; set; }

        public string ExpiryDateCheck { get; set; }

        public string PersonalNumber { get; set; }

        public string PersonalNumberVerified { get; set; }

        public string PersonalNumberCheck { get; set; }

        public string ChecksumVerified { get; set; }

        public string Checksum { get; set; }

        public DateOfBirthSctruct DateOfBirth;

        public string SeriesPassportNumber { get; set; }

        public string PassportNumber { get; set; }
    }
}
