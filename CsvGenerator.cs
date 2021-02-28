using System;
using System.Collections;  // support for IList
using System.Collections.Generic;  // support for List<>
using System.IO;  // support for File
using System.Reflection;  // support for PropertyInfo
using System.Text;  // support for Encoding


namespace dotnetCore_dropbox
{
    public class CsvGenerator<T>
    {
        protected LogService _logService;
        protected bool _debug;
        private readonly T modelObject;


        public CsvGenerator(LogService logService, bool debug, T value)
        {
            _logService = logService;
            _debug = debug;

            // header record (use to initialize T)
            modelObject = value;
        }


        // Output SQL query data to CSV file
        public void CreateCsvFile(string csvFile, List<T> sqlData, bool headerString)
        {
            _logService.LogActivity($"Preparing to ouput dataset to CSV file: {csvFile}");
            _logService.LogActivity($"Start time: {DateTime.Now.ToString()}");

            string csvString = "";

            if (headerString)
            {
                csvString = CreateCsvHeaderString() + CreateCsvString(sqlData);
            }
            else
            {
                csvString = CreateCsvString(sqlData);
            }

            try
            {
                File.WriteAllText(csvFile, csvString);

                // // test error logging
                // Exception ex = new Exception();
                // throw ex;

                _logService.LogActivity($"CSV file created: {DateTime.Now.ToString()}");
            }
            catch (Exception ex)
            {
                _logService.LogActivity($"Error during output to CSV file: {csvFile}");
                _logService.LogActivity("Error: " + ex);
            }
        }


        // Output string to CSV file
        public void CreateCsvFile(string csvFile, string csvString)
        {
            try
            {
                _logService.LogActivity($"Preparing to ouput string to CSV file: {csvFile}");

                // support for Spanish characters via Encoding.UTF8
                File.WriteAllText(csvFile, csvString, Encoding.UTF8);

                _logService.LogActivity("CSV file created!");
            }
            catch (Exception ex)
            {
                _logService.LogActivity($"Error during output to CSV file: {csvFile}");
                _logService.LogActivity("Error: " + ex);
            }
        }


        // Create CSV header string from model
        public string CreateCsvHeaderString()
        {
            string csvHeaderString = "";

            try
            {
                _logService.LogActivity($"Preparing to output properties from {modelObject.GetType()} to string...");

                // append (sanitized) property value + comma for every property in model (T)
                foreach (PropertyInfo propertyInfo in modelObject.GetType().GetProperties())
                {
                    csvHeaderString += $"{propertyInfo.Name},";                    
                }
                
                // remove trailing comma from final property
                csvHeaderString = csvHeaderString.TrimEnd(',');

                // carriage return for next record in List<T>
                csvHeaderString += Environment.NewLine;
            }
            catch (Exception ex)
            {
                _logService.LogActivity($"Error creating CSV header string from {modelObject.GetType()}...");
                _logService.LogActivity("Error: " + ex);
            }

            return csvHeaderString;
        }


        // Create CSV string from (model) records in SQL data
        public string CreateCsvString(List<T> sqlData)
        {
            string csvString = "";
            string propertyValue;
            
            try
            {
                _logService.LogActivity($"Preparing to output values from List<{modelObject.GetType()}> to string...");

                foreach (T record in sqlData)
                {
                    // append (sanitized) property value + comma for every property in model (T)
                    foreach (PropertyInfo propertyInfo in record.GetType().GetProperties())
                    {
                        // Console.WriteLine($"{propertyInfo.Name}: {propertyInfo.GetValue(record, null)}");

                        // get value of current property
                        propertyValue = CheckValueForComma(propertyInfo.GetValue(record, null).ToString());

                        // // check for List<T> property
                        // if (propertyValue.Contains("System.Collections.Generic.List`1") && propertyInfo.Name != "")
                        // {
                        //     csvString = GetNestedListProperties(record, propertyInfo, csvString);
                        // }
                        // else
                        // {
                            csvString += $"{propertyValue},";
                        // }
                    }
                    
                    // remove trailing comma from final property
                    csvString = csvString.TrimEnd(',');

                    // carriage return for next record in List<T>
                    csvString += Environment.NewLine;
                }
            }
            catch (Exception ex)
            {
                _logService.LogActivity($"Error creating string from List<{modelObject.GetType()}>...");
                _logService.LogActivity("Error: " + ex);
            }

            if (_debug)
            {
                Console.WriteLine($"csvString:{Environment.NewLine}{csvString}");
            }

            return csvString;
        }


        // Check record value for inner comma (otherwise results in additional delimited fields in CSV)
        public string CheckValueForComma(string value)
        {
            if (value.Contains(","))
            {
                value = '"' + value + '"';
            }

            return value;
        }


        public string GetNestedListProperties(T record, PropertyInfo propertyInfo, string csvString)
        {
            _logService.LogActivity("Nested property detected...");

            string nestedListPropertyName = propertyInfo.Name;

            if (nestedListPropertyName != "")
            {
                // dig into List<T> property's properties
                PropertyInfo nestedPropertyInfo = record.GetType().GetProperty(nestedListPropertyName);
                object nestedPropertyValue = nestedPropertyInfo.GetValue(record, null);
                IList nestedListProperty = (IList)nestedPropertyValue;

                // an extra blank line to create a little more separation between subproperties
                Console.WriteLine($"nestedListPropertyName: {nestedListPropertyName}");

                // add separator and header as appropriate
                csvString += Environment.NewLine;

                switch(nestedListPropertyName)
                {
                    case "SharedFolders":
                        csvString += "-----,-----,-----,-----,-----,-----,-----,-----,-----";
                        csvString += Environment.NewLine;
                        csvString += "SharedFolderId,SharedFolderName,SharePathLower,ParentFolderName,ShareIsTeamFolder,ShareIsInsideTeamFolder,SharedFolderUsers,SharedFolderGroups,SharedFolderInvitees";
                        break;

                    case "FolderMemberUsers":
                        csvString += "-----,-----,-----";
                        csvString += Environment.NewLine;
                        csvString += "FolderMemberUserAccountId,FolderMemberUserDisplayName,FolderMemberUserEmail";
                        break;

                    default:
                        Console.WriteLine($"!!!!! no match on nestedListPropertyName: {nestedListPropertyName}");
                        break;
                }

                csvString += Environment.NewLine;
             
                foreach (object nestedList in nestedListProperty)
                {
                    csvString += Environment.NewLine;

                    // iterate through each List<T> object and get property values
                    foreach (PropertyInfo nestedListPropertyInfo in nestedList.GetType().GetProperties())
                    {
                        // append nested property values
                        csvString += $"{nestedListPropertyInfo.GetValue(nestedList, null)},";

                        Console.WriteLine($"nested property value: {nestedListPropertyInfo.GetValue(nestedList, null)}");

                        // call again to cover deeply nested properties (e.g. shared folder members data)
                        if (nestedListPropertyInfo.Name != "")
                        {
                            Console.WriteLine($"***** inside nested nestedListPropertyInfo.Name != \"\"");
                            Console.WriteLine($"nestedListPropertyInfo.Name: {nestedListPropertyInfo.Name}");

                            GetNestedListProperties(record, nestedListPropertyInfo, csvString);
                        }
                    }
                }

                // reset for next iteration
                nestedListPropertyName = "";
            }

            return csvString;
        }

    }
}