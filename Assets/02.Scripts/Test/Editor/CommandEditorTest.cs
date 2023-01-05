using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using System;
using Gather.Interact;

[CustomPropertyDrawer(typeof(InteractCommand<DefaultGroup>))]
public class CommandEditorTest : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);

        SerializedProperty authority = property.FindPropertyRelative("authority");

        EditorGUI.PropertyField(position, property, label, true);
        if (property.isExpanded)
        {
            /*if (GUI.Button(new Rect(position.xMin + 30f, position.yMax - 20f, position.width - 30f, 20f), "button"))
            {
                // do things
            }*/
        }
        
        //test2(property.serializedObject.targetObject, property);

        SerializedProperty testField2 = property.FindPropertyRelative("testField2");
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded)
            return EditorGUI.GetPropertyHeight(property) + 20f;
        return EditorGUI.GetPropertyHeight(property);
        /*
                SerializedObject childObj = new UnityEditor.SerializedObject(property.objectReferenceValue as Command);
                SerializedProperty ite = childObj.GetIterator();

                float totalHeight = EditorGUI.GetPropertyHeight(property, label, true) + EditorGUIUtility.standardVerticalSpacing;

                while (ite.NextVisible(true))
                {
                    totalHeight += EditorGUI.GetPropertyHeight(ite, label, true) + EditorGUIUtility.standardVerticalSpacing;
                }

                return totalHeight;*/
    }
    /*
    public void test<T>(object item)
    {
        Type[] types = Assembly.GetAssembly(typeof(T)).GetTypes();
        foreach (Type type in types)
        {
            //Debug.Log(type.Name);
            if (type.IsSubclassOf(typeof(T)))
            {
                // 이걸로 캐스팅 해보고, 안되면 다음으로 넘어가기

                Debug.Log(item.GetType().Name);

                bool check = true;
                var fields = type.GetFields();
                foreach (var field in fields)
                {
                    try
                    {
                        field.SetValue(item, Convert.ChangeType(field.GetValue(item), field.FieldType));
                        //Debug.Log($"{field.Name}({field.FieldType}) : {Convert.ChangeType(field.GetValue(item), field.FieldType)}");
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                        check = false;
                    }
                }
                if (check)
                {
                    Debug.Log("222222222222" + type.Name);
                    foreach (var field in fields)
                    {
                        Debug.LogAssertion($"{field.Name}({field.FieldType}) : {Convert.ChangeType(field.GetValue(item), field.FieldType)}");
                        //EditorGUILayout.LabelField(field.Name, field.GetValue(item).ToString());
                    }
                }

                //EditorGUILayout.LabelField(type.Name, type.Name);

                // type이 T의 자식 클래스이다.
            }
        }
    }*/

    public void test2(object item, SerializedProperty property)
    {
        Debug.Log("qqqqqqqqqqq");
        if (item is InteractCommand<DefaultGroup>)
        {
            Debug.Log("InteractionCommand<ObjectGroup>");
            if (item is DeleteObjectCommand)
            {
                //(DeleteObjectCommand)item;
                Test3<DeleteObjectCommand>(item as DeleteObjectCommand, property);
                Debug.Log("DeleteObjectCommand");
            }
        }
        else if (item is DeleteObjectCommand)
        {
            Debug.Log("DeleteObjectCommand");
        }
    }

    public void Test3<T>(T item, SerializedProperty property)
    {
        Debug.Log("wwwwwwwwwww");
        var fields = item.GetType().GetFields();
        foreach (var field in fields)
        {
            Debug.LogAssertion($"{field.Name}({field.FieldType}) : {Convert.ChangeType(field.GetValue(item), field.FieldType)}");
            //EditorGUILayout.LabelField(field.Name, field.GetValue(item).ToString());
        }
    }
}

/*[CustomEditor(typeof(List<InteractionCommand<ObjectGroup>>))]
public class CommandEditorTest3 : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("test", "test1");

        Debug.Log(3333);
    }
}*/

[CustomEditor(typeof(DefaultGroup))]
public class CommandEditorTest2 : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        /*DefaultGroup t = (DefaultGroup)target;

        int cnt = 0;

        EditorGUILayout.LabelField("Command List");
        
        foreach (var item in t.commandList)
        {
            EditorGUILayout.LabelField(cnt++.ToString(), item.GetType().Name);
            if (item.GetType() == typeof(DeleteObjectCommand))
            {
                Test<DeleteObjectCommand>((DeleteObjectCommand)item.Value);
            }
            else
            {
                EditorGUILayout.LabelField("ObjectGroup", "etc");
            }
            EditorGUILayout.Space();
        }
        
        Type[] types = Assembly.GetAssembly(typeof(DefaultGroup)).GetTypes();
        foreach (Type type in types)
        {
            //Debug.Log($"{type.Name} {type.IsSubclassOf(typeof(InteractionCommand<ObjectGroup>))}");
            if (type.IsSubclassOf(typeof(InteractCommand<DefaultGroup>)))
            {
                if (GUILayout.Button(type.Name))
                {
                    InteractCommand<DefaultGroup> command = (InteractCommand<DefaultGroup>)Activator.CreateInstance(type);
                    command.id = UnityEngine.Random.Range(0, 100);
                    t.commandList.Add(command.id, command);
                }
            }
        }*/
    }

    /*public void Test<T>(T item)
    {
        var fields = item.GetType().GetFields();
        foreach (var field in fields)
        {
            if (field.FieldType == typeof(int))
            {
                field.SetValue(item, EditorGUILayout.IntField(field.Name, (int)field.GetValue(item)));
            }
            else if (field.FieldType == typeof(string))
            {
                field.SetValue(item, EditorGUILayout.TextField(field.Name, (string)field.GetValue(item)));
            }
            else if (field.FieldType == typeof(bool))
            {
                field.SetValue(item, EditorGUILayout.Toggle(field.Name, (bool)field.GetValue(item)));
            }
            else if (field.FieldType == typeof(byte))
            {
                field.SetValue(item, (byte)EditorGUILayout.IntField(field.Name, (byte)field.GetValue(item)));
            }
            else
            {
                EditorGUILayout.LabelField(field.FieldType.Name, field.Name);
            }
        }
    }*/

    // T: InteractionCommand<ObjectGroup>
    /*public void test<T>(T item)
    {
        Type[] types = Assembly.GetAssembly(typeof(T)).GetTypes();
        foreach (Type type in types)
        {
            //Debug.Log(type.Name);
            if (type.IsSubclassOf(typeof(T)))
            {
                // 이걸로 캐스팅 해보고, 안되면 다음으로 넘어가기

                Debug.Log(item.GetType().Name);
                if (item.GetType() == type)
                {
                    Debug.Log("222222222222" + type.Name);
                    var fields = type.GetFields();
                    foreach (var field in fields)
                    {
                        EditorGUILayout.LabelField(field.Name, field.GetValue(item).ToString());
                    }
                }

                bool check = true;
                var fields = type.GetFields();
                foreach (var field in fields)
                {
                    try
                    {
                        field.SetValue(item, Convert.ChangeType(field.GetValue(item), field.FieldType));
                        //Debug.Log($"{field.Name}({field.FieldType}) : {Convert.ChangeType(field.GetValue(item), field.FieldType)}");
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                        check = false;
                    }
                }
                if (check)
                {
                    Debug.Log("222222222222" + type.Name);
                    foreach (var field in fields)
                    {
                        DrawDefaultInspector();
                        Debug.LogAssertion($"{field.Name}({field.FieldType}) : {Convert.ChangeType(field.GetValue(item), field.FieldType)}");
                        EditorGUILayout.LabelField(field.Name, field.GetValue(item).ToString());
                    }
                }

                var value2 = Convert.ChangeType(item, type);
                Debug.Log(value2);

                EditorGUILayout.LabelField(type.Name, type.Name);
                Debug.Log("1111111111" + type.Name);

                // type이 T의 자식 클래스이다.

                // type의 필드를 가져온다.
                var fields = type.GetFields();
                foreach (var field in fields)
                {
                    Debug.Log($"{field.Name}");
                    //(T)Convert.ChangeType(field.GetValue(item), typeof(T))
                    //EditorGUILayout.LabelField(field.Name, (T)Convert.ChangeType(field.GetValue(item), typeof(T)));
                    EditorGUILayout.LabelField(field.Name, field.GetValue(item).ToString());
                }
            }
        }

        //T cmd = item as T;
        EditorGUILayout.LabelField("ObjectGroup", "DeleteObjectCommand");
        var fields = typeof(DeleteObjectCommand).GetFields();
        foreach (var field in fields)
        {
            EditorGUILayout.LabelField(field.Name, field.GetValue(item).ToString());
        }
    }*/

    /*public void test2()
    {
        var assemblyName = "Some.Assembly.Name";
        var nameSpace = "Some.Namespace.Name";
        var className = "ClassNameFilter";

        var asm = Assembly.Load(assemblyName);
        var classes = asm.GetTypes().Where(p =>
             p.Namespace == nameSpace &&
             p.Name.Contains(className)
        ).ToList();
    }

    static IEnumerable<string> GetClasses(string nameSpace)
    {
        Assembly asm = Assembly.GetExecutingAssembly();

        foreach (Type type in asm.GetTypes())
        {
            Debug.Log($"{type.Namespace} {type.Name} {type}");
        }


        return asm.GetTypes()
            .Where(type => type.Namespace == nameSpace)
            .Select(type => type.Name);
    }*/
}

