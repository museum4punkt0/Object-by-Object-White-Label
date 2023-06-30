using System;
using System.Globalization;
using UnityEngine;

public class StringUtils
{
	public const string htmlTagEm = "em";
	public const string htmlTagI = "i";

	public static string ReplaceTag(string input, string htmlTagToReplace, string htmlTagNew)
	{
		string result = input;

		result = result.Replace(string.Format("<{0}>", htmlTagToReplace), string.Format("<{0}>", htmlTagNew));
		result = result.Replace(string.Format("</{0}>", htmlTagToReplace), string.Format("</{0}>", htmlTagNew));

		return result;
	}

	public static string RemoveWezitLinefeed(string input)
	{
		string result = input;
		result = result.Replace("<br />", "");
		result = result.Replace("<BR />", "");
		return result;
	}

    public static string RemoveUnwantedTags(string input)
	{
		string result = input;
		result = result.Replace("<p>", "");
		result = result.Replace("</p>", "");
        
		return result;
	}

    public static string RemoveUnderlineTag(string input)
    {
        string result = input;
        result = result.Replace("<u>", "");
		result = result.Replace("</u>", "");

        return result;
    }

	public static string ReplaceBoldTags(string input)
	{
		return input.Replace("strong>", "b>");
	}

	public static string CleanFromWezit(string input)
	{
		if (string.IsNullOrEmpty(input)) return input;
		string result = input;

		result = ReplaceTag(result, htmlTagEm, htmlTagI);
		result = RemoveWezitLinefeed(result);
		result = ReplaceBoldTags(result);
        result = RemoveUnwantedTags(result);

		return result;
	}

	//Convert a string into a Vector3. String has to be in the (x, y, z) format
	public static Vector3 StringToVector3(string stringVector)
     {
        // Remove the parentheses
        if (stringVector.StartsWith ("(") && stringVector.EndsWith (")")) {
            stringVector = stringVector.Substring(1, stringVector.Length-2);
        }
 
        // split the items
        string[] stringArray = stringVector.Split(',');
 
        // store as a Vector3
        Vector3 realVector3 = new Vector3(
            float.Parse(stringArray[0], CultureInfo.InvariantCulture),
            float.Parse(stringArray[1], CultureInfo.InvariantCulture),
            float.Parse(stringArray[2], CultureInfo.InvariantCulture));
 
        return realVector3;
     }

	public static string ScrambleString(string input, bool keepSpaces = true)
	{
        string tempString = input;
        string scrumbledString = "";
        while(tempString != "")
        {
            int charIndex = UnityEngine.Random.Range(0, tempString.Length);
			if(tempString[charIndex] == ' ')
			{
				if(keepSpaces)
				{
					scrumbledString += tempString[charIndex].ToString();
				}
			}
			else
			{
				scrumbledString += tempString[charIndex].ToString();
			}
            tempString = tempString.Substring(0, charIndex) + tempString.Substring(charIndex + 1, tempString.Length - (charIndex + 1));
        }
		return scrumbledString;
	}

	public static string AddCustomTagsFromWezit(string input)
	{
		string result = input;
		result = result.Replace('[', '<').Replace(']', '>');

		return result;
	}
}
