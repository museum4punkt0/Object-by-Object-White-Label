using UnityEditor;

public static class EditorUtils
{
	public static System.Type GetType(SerializedProperty property)
	{
		System.Type parentType = property.serializedObject.targetObject.GetType();
		System.Reflection.FieldInfo fi = parentType.GetFieldViaPath(property.propertyPath);
		return fi.FieldType;
	}

	public static System.Reflection.FieldInfo GetFieldViaPath(this System.Type type, string path)
	{
		System.Type parentType = type;
		System.Reflection.FieldInfo fi = type.GetField(path, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
		string[] perDot = path.Split('.');
		foreach (string fieldName in perDot)
		{
			fi = parentType.GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
			if (fi != null)
				parentType = fi.FieldType;
			else
				return null;
		}

		return fi;
	}
}
