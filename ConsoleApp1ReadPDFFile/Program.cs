using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp1ReadPDFFile
{
    internal class Program
    {

        static void Main(string[] args)
        {
            string path = @"C:\Users\shahid\Downloads\Shaked Invoice PDF";
            string textFileToWrite = "myFile.txt";
            string csvFileToWrite = "myFile.csv";
            string startHere = "Start Here...";
            char newLine = '\n';
            string saperatorLine = "=======================================================";
            StringBuilder stringBuilder = new StringBuilder();
            //string csvHeaderRow = "AccountID, AccountName, InvoiceDate, DocumentNumber, TransectionAmount, TransectionPaymentMethod, TransectionDescription";
            string csvHeaderRow = "Client/Account Name,Client/Account ID,Opportunity Product,Schedule Date,Price/Payment per row,Document Number,Received Payment per row,Status (Invoice Open / Paid / Not generated),Description,Payment Method,Filename";

            var fileNames = Directory.GetFiles(path, "*.pdf", SearchOption.AllDirectories);

            File.WriteAllText(textFileToWrite, startHere, Encoding.UTF8);
            File.WriteAllText(csvFileToWrite, csvHeaderRow + Environment.NewLine, Encoding.UTF8);

            int loopCounter = 1;
            foreach (var file in fileNames)
            {
                //string normal = String.Empty;
                //stringBuilder.AppendLine(loopCounter + ": " + file + newLine);
                stringBuilder.AppendLine(file + newLine);
                loopCounter++;

                //normal = loopCounter + ": " + file + newLine + saperatorLine + newLine;
                using (PdfReader reader = new PdfReader(file))
                {
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        stringBuilder.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                        //normal += PdfTextExtractor.GetTextFromPage(reader, i);
                    }
                    //File.AppendAllText(fileToWrite1, normal + Environment.NewLine, Encoding.UTF8);
                }
                var processedString = StringToProcessList(stringBuilder.ToString());
                stringBuilder.Clear();

                var CSVString = ExtractInformationFromString(processedString);

                var populatedcsv = CSVStringPopulator(CSVString);

                File.AppendAllLines(textFileToWrite, processedString, Encoding.UTF8);
                File.AppendAllLines(csvFileToWrite, populatedcsv, Encoding.UTF8);

                //ExtractInformationFromString(processedString);
            }
        }

        private static List<string> CSVStringPopulator(CSVStructureClass cSVString)
        {
            List<string> kdkfkdkf = new List<string>();

            foreach (var trans in cSVString.Transections)
            {
                string delemeter = ",";
                string row =
                    cSVString.AccountName + delemeter +
                    cSVString.AccountID + delemeter + delemeter +
                    cSVString.InvoiceDate.ToString("d") + delemeter +
                    trans.TransectionAmount + delemeter +
                    cSVString.DocumentNumber + delemeter + delemeter + delemeter +
                    trans.TransectionDescription + delemeter +
                    trans.TransectionPaymentMethod + delemeter +
                    cSVString.FileName + delemeter;

                kdkfkdkf.Add(row);
            }

            return kdkfkdkf;
            //throw new NotImplementedException();
        }

        private static List<string> StringToProcessList(string v)
        {
            List<string> processList = new List<string>();

            var stringArray = v.Split('\n').ToList();
            foreach (var item in stringArray)
            {
                if (String.IsNullOrWhiteSpace(item))
                {
                    continue;
                }
                processList.Add(Regex.Replace(item, @"\s+", " ").Trim());
            }

            return processList;
        }

        private static CSVStructureClass ExtractInformationFromString(List<string> normal)
        {
            CSVStructureClass cSVStructureClass = new CSVStructureClass();
            cSVStructureClass.Transections = new List<TransStructure>();

            for (int i = 0; i < normal.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        cSVStructureClass.FileName = normal[i];
                        break;
                    case 4:
                        cSVStructureClass.DocumentNumber = ExtractNumberFromStringUsingColon(normal[i]);
                        break;
                    case 5:
                        cSVStructureClass.AccountName = ReversString(ExtractAccountName(normal[i]));
                        break;
                    case 6:
                        cSVStructureClass.AccountID = ExtractNumberFromStringUsingColon(normal[i]);
                        break;
                    case 7:
                        cSVStructureClass.InvoiceDate = ExtractDateFromStringUsingColon(normal[i]);
                        break;
                    case 10:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                        if (CheckTransectionsFromString(normal[i]))
                        {
                            float amountt = 0;
                            string payment = "";
                            string descipt = "";
                            TransStructure trans = new TransStructure();
                            var statue = ExtractTransectionDetailsFromStringUsingSpace(normal[i], out amountt, out payment, out descipt);

                            trans.TransectionAmount = amountt;
                            trans.TransectionPaymentMethod = payment;
                            trans.TransectionDescription = descipt;

                            cSVStructureClass.Transections.Add(trans);

                            //cSVStructureClass.Transections.TransectionAmount.Add(amountt);
                            //cSVStructureClass.TransectionPaymentMethod.Add(payment);
                            //cSVStructureClass.TransectionDescription.Add(descipt);
                            // cSVStructureClass.TransectionAmount.Add(ExtractTransectionDetailsFromStringUsingSpace(normal[i]));
                        }
                        break;
                    default:
                        break;
                }
            }
            return cSVStructureClass;

            //List<CSVStructureClass> listCSV = new List<CSVStructureClass>();

            //var stringArray = normal.Split('\n').ToList();
            //List<string> mainList = new List<string>();

            //foreach (var item in stringArray)
            //{
            //    if (String.IsNullOrWhiteSpace(item))
            //    {
            //        continue;
            //    }
            //    var processedString = Regex.Replace(item, @"\s+", " ").Trim();
            //    mainList.Add(processedString);
            //}

            //if (mainList.Count() < 15)
            //{
            //    Console.WriteLine("sddfdfd");
            //}
            ////throw new NotImplementedException();
            //if (mainList.Count() > 15)
            //{


            //}
        }
        private static string ReversString(string str)
        {
            string str2 = "";
            for (int i = str.Length - 1; i >= 0; --i)
                str2 += str[i];

            return str2;
        }
        private int ExtractNumberFromString(string value)
        {
            int number = 0;
            var resultString = Regex.Match(value, @"\d+").Value;

            if (int.TryParse(resultString, out number))
            {
                return number;
            }
            return number;
        }
        private static string ExtractAccountName(string value)
        {
            int number = 0;
            string result = "";
            var stringResulttt = value.Split(':').ToList();
            for (int i = 0; i < stringResulttt.Count; i++)
            {
                switch (i)
                {
                    case 1:
                        result = stringResulttt[i].Replace("השרומ קסוע רפסמ", "").Trim();
                        break;

                    default:
                        break;
                }
            }
            return result;
        }
        private static int ExtractNumberFromStringUsingColon(string value)
        {
            int number = 0;
            var stringResulttt = value.Split(':').ToList();
            if (int.TryParse(stringResulttt[0], out number))
            {
                return number;
            }
            return number;
        }
        private static bool ExtractTransectionDetailsFromStringUsingSpace(string value, out float amount, out string paymentMethod, out string description)
        {
            amount = 0;
            paymentMethod = null;
            description = null;
            int number = 0;
            var stringResulttt = value.Split(' ').ToList();
            if (stringResulttt.Count == 6)
            {
                for (int i = 0; i < stringResulttt.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            amount = float.Parse(stringResulttt[i].Trim());
                            //var resultString = Regex.Match(stringResulttt[i], @"\d+").Value;
                            //if (int.TryParse(resultString, out number))
                            //{
                            //    return number;
                            //}
                            break;
                        case 4:
                            paymentMethod = ReversString(stringResulttt[i]).Trim();
                            break;
                        case 5:
                            description = ReversString(stringResulttt[i]).Trim();
                            break;
                        default:
                            break;
                    }
                }
                return true;
            }
            //if (int.TryParse(stringResulttt[0], out number))
            //{
            //    return number;
            //}
            return false;
        }

        private static DateTime ExtractDateFromStringUsingColon(string value)
        {
            DateTime number = DateTime.MinValue;
            var stringResulttt = value.Split(':').ToList();
            if (DateTime.TryParse(stringResulttt[0], out number))
            {
                return number;
            }
            return number;
        }
        private static bool CheckTransectionsFromString(string value)
        {
            //var resultString = Regex.Match(value, @"[0-9.]+ [0-9/]+ [0-9]+").Value;
            var resultString = Regex.Match(value, @"(\d)+(.)(\d)+(\s)(\d)+(/)(\d)+(/)(\d)+(\s)(\d)+").Value;

            if (resultString.Length > 5)
            {
                return true;
            }
            return false;
        }
        private static List<string> ExtractTransectionsFromString(string value)
        {
            List<string> transectionList = new List<string>();
            int number = 0;

            var resultString = Regex.Match(value, @"[0-9.]+ [0-9/]+ [0-9]+").Value;


            var stringResulttt = value.Split(':').ToList();
            if (int.TryParse(stringResulttt[0], out number))
            {
                return transectionList;
            }
            return transectionList;
        }
    }
    class CSVStructureClass
    {
        public string FileName { get; set; }
        public string AccountName { get; set; }
        public int AccountID { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int DocumentNumber { get; set; }
        public List<TransStructure> Transections { get; set; } = new List<TransStructure>();
    }
    class TransStructure
    {
        public float TransectionAmount { get; set; }
        public string TransectionDescription { get; set; }
        public string TransectionPaymentMethod { get; set; }
    }
}
