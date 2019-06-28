﻿using System;
using System.IO;
using System.Threading;

namespace testMRZLib
{
    class Program
    {
        static void Main(string[] args)
        {
            MRZScannerLibrary.MRZScannerLibrary test =
                 new MRZScannerLibrary.MRZScannerLibrary("My_app_testProcessMRZ", "Y5J5xYPvDyVRBGR47dfh76kc");

            byte[] image = File.ReadAllBytes("C:\\Users\\yubarbeu\\Desktop\\240px-Lauda,_Niki_1973-07-06.webp");

            test.StartScan(image);

            while (true)
            {
                Console.WriteLine(test.isScanCompleted + " - \n");

                if (test.isScanCompleted)
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
    }
}
