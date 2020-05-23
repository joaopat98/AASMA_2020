using UnityEngine;
using System.IO;

public static class CSVManager
{
    public static string reportFolderName = "Statistics";
    private static string reportFileName = "statistics";
    private static string reportSeparator = ",";
    private static string TimeNow;
    private static string[] stepHeaders = new string[10] {
        "Step",
        "Healthy",
        "Infected",
        "Cured",
        "Dead",
        "Advice: Use Mask",
        "Advice: Social Distancing",
        "Using Mask",
        "Average Social Distancing",
        "Average Fear"
    };
    private static string[] parameterHeadings = new string[5] {
        "Average Trust",
        "Government Boldness",
        "Civilians",
        "Police",
        "Medical Staff"
    };
    private static string[] virusHeadings = new string[3] {
        "Lethality",
        "Transmission Rate",
        "Incubation Time"
    };

    public static void AppendToReport(string[] strings)
    {
        VerifyDirectory();
        VerifyFile();
        using (StreamWriter sw = File.AppendText(GetFilePath()))
        {
            string finalString = "";
            for (int i = 0; i < strings.Length; i++)
            {
                if (finalString != "")
                {
                    finalString += reportSeparator;
                }
                finalString += strings[i];
            }
            finalString += reportSeparator;
            sw.WriteLine(finalString);
        }
    }

    public static void CreateReport(string folderExtension, string parameters, string virus)
    {
        reportFolderName = reportFolderName + "/" + folderExtension;
        TimeNow = System.DateTime.Now.ToString("dd-MM-yyyy--HH_mm_ss");
        VerifyDirectory();
        using (StreamWriter sw = File.CreateText(GetFilePath()))
        {
            string finalString = "";
            for (int i = 0; i < parameterHeadings.Length; i++)
            {
                if (finalString != "")
                {
                    finalString += reportSeparator;
                }
                finalString += parameterHeadings[i];
            }
            finalString += reportSeparator;

            sw.WriteLine(finalString);
            sw.WriteLine(parameters);

            sw.WriteLine("");

            finalString = "";
            for (int i = 0; i < virusHeadings.Length; i++)
            {
                if (finalString != "")
                {
                    finalString += reportSeparator;
                }
                finalString += virusHeadings[i];
            }
            finalString += reportSeparator;
            sw.WriteLine(finalString);
            sw.WriteLine(virus);

            sw.WriteLine("");

            finalString = "";
            for (int i = 0; i < stepHeaders.Length; i++)
            {
                if (finalString != "")
                {
                    finalString += reportSeparator;
                }
                finalString += stepHeaders[i];
            }
            finalString += reportSeparator;
            sw.WriteLine(finalString);
        }
    }


    #region Operations

    static void VerifyDirectory()
    {
        string dir = GetDirectoryPath();
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    static void VerifyFile()
    {
        string file = GetFilePath();
        if (!File.Exists(file))
        {
            Debug.Log("Error: Tried to write to file '" + file + "'. File does not exist.");
            //CreateReport();
        }
    }

    #endregion

    #region Queries
    static string GetDirectoryPath()
    {
        return Application.dataPath + "/" + reportFolderName;
    }

    static string GetFilePath()
    {
        return GetDirectoryPath() +  "/" + reportFileName + "_" + TimeNow + ".csv";
    }

    public static string GetSettingsFilePath()
    {
        return GetDirectoryPath() + "/settings.json";
    }

    #endregion
}
