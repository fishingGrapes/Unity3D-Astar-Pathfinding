using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class ToDo : EditorWindow {

	//Position of the vertical scoll slider
	private Vector2 scrollPos = Vector2.zero;

	//Initialize window
	[MenuItem("Window/To Do")]
	static void Init(){
		ToDo window = (ToDo)EditorWindow.GetWindow (typeof(ToDo));
		window.Show ();
	}


	void OnGUI()
	{
		//Get all directories in assets folder
		string[] assetPaths = AssetDatabase.GetAllAssetPaths();

		//List of scripts
		List<MonoScript> scripts = new List<MonoScript> ();

		//Get all scripts in assets folder
		foreach(string assetPath in assetPaths)
		{
			if (!assetPath.Contains ("/Editor/")) {
				if (assetPath.EndsWith (".cs") || assetPath.EndsWith (".js")) {
					scripts.Add (AssetDatabase.LoadAssetAtPath<MonoScript> (assetPath));
				}
			}
		}

		//Create scroll slider
		scrollPos = GUILayout.BeginScrollView (scrollPos);



		//Finding ToDos
		foreach (MonoScript script in scripts) {

			string sub;
			sub = script.text;

			//For counting the lines
			int lineCount = 0;

			if (sub.Contains ("//TODO:")) {
				GUILayout.Label ("In script \"" + script.name + "\"", EditorStyles.boldLabel);
			}

			while (sub != "") {
				if (sub.Contains ("//TODO:")) {
					//Find index of next ToDo, do not desplay the //todo part(hence the 8)
					int start = sub.IndexOf ("//TODO:") + 7;

					bool important = (sub[start] == '!') || (sub[start + 1] == '!');

					//Remove space
					if (sub [start] == ' ') {
						sub = sub.Substring (1);
					}

					//Remove the '!' from substring
					if (sub[start] == '!') {
						sub = sub.Substring (1);
					}

					//Find /n ending the line of the current ToDo
					int end = sub.Substring (start).IndexOf ("\n");

					//Count line number
					for (int i = 0; i < start + end + 1; i++) {
						if (sub [i] == '\n') {
							lineCount++;
						}
					}

					//Get the string of the current ToDo
					string todo = sub.Substring (start, end);
					//Decrease the size of current script
					sub = sub.Substring (start + end + 1);

					//Draw ToDo line
					GUILayout.BeginHorizontal ();
					if (!important) {
						GUI.backgroundColor = Color.yellow;
					} else {
						GUI.backgroundColor = Color.red;
					}
					if (GUILayout.Button (todo + " at line " + lineCount.ToString (), EditorStyles.textArea)) {
						AssetDatabase.OpenAsset (script, lineCount);
					}
					GUILayout.EndHorizontal ();
				} else {
					//Reset the substring and line count to prepare the next file
					lineCount = 0;
					sub = "";
				}
			}
		}

		GUILayout.EndScrollView ();
	}

}
