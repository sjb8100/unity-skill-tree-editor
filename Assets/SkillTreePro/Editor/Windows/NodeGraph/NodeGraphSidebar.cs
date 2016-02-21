﻿using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Adnc.SkillTreePro {
	public class NodeGraphSidebar {
		Rect pos;
		Color backgroundColor = Color.white;
		const int padding = 10;

		public void Update (Rect pos) {
			this.pos = pos;

			WrapperBegin();
			Content();
			WrapperEnd();
		}

		void Content () {
			EditorGUI.BeginChangeCheck();
			Categories();
			if (EditorGUI.EndChangeCheck()) {
				EditorUtility.SetDirty(Wm.Db);
			}
		}

		void Inspector () {
			if (Wm.DbCol == null) {
				EditorGUILayout.LabelField("Please select a skill collection to edit.");
				return;
			}
		}

		void Categories () {
			int deleteIndex = -1;
			int upIndex = -1;
			int downIndex = -1;

			for (int i = 0, l = Wm.Db.categories.Count; i < l; i++) {
				SkillCategoryDefinition cat = Wm.Db.categories[i];
				EditorGUILayout.BeginVertical(EditorStyles.helpBox);

				if (Wm.DbCat == cat) {
					cat._displayName = EditorGUILayout.TextField(cat._displayName);
				} else {
					EditorGUILayout.LabelField(cat.DisplayName);
				}

				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Edit", GUILayout.ExpandWidth(false))) {
					Wm.DbCat = cat;
				}

				if (Wm.DbCat != cat && GUILayout.Button("Destroy", GUILayout.ExpandWidth(false))) {
					deleteIndex = i;
				}

				if (GUILayout.Button("Up", GUILayout.ExpandWidth(false))) {
					upIndex = i;
				}

				if (GUILayout.Button("Down", GUILayout.ExpandWidth(false))) {
					downIndex = i;
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
			}

			if (deleteIndex > -1) {
				Wm.DestroyCategory(deleteIndex);
			} else if (upIndex > -1) {
				MoveCategory(upIndex, true);
			} else if (downIndex > -1) {
				MoveCategory(downIndex, false);
			}

			if (GUILayout.Button("Add Category")) {
				SkillCategoryDefinition scd = ScriptableObject.CreateInstance("SkillCategoryDefinition") as SkillCategoryDefinition;
				scd.Setup();
				AssetDatabase.AddObjectToAsset(scd, Wm.Db);

				SkillCollectionStartDefinition scsd = ScriptableObject.CreateInstance("SkillCollectionStartDefinition") as SkillCollectionStartDefinition;
				scsd._displayName = "Start";
				scsd.Setup(Wm.DbCat);
				AssetDatabase.AddObjectToAsset(scsd, Wm.Db);

				scd.start = scsd;
				Wm.Db.categories.Add(scd);

				EditorUtility.SetDirty(Wm.Db);
				AssetDatabase.SaveAssets();
			}
		}

		void MoveCategory (int index, bool up) {
			SkillCategoryDefinition cat = Wm.Db.categories[index];
			int max = Wm.Db.categories.Count - 1;

			Wm.Db.categories.RemoveAt(index);

			if (up) {
				Wm.Db.categories.Insert(Mathf.Max(index - 1, 0), cat);
			} else {
				Wm.Db.categories.Insert(Mathf.Min(index + 1, max), cat);
			}

			EditorUtility.SetDirty(Wm.Db);
		}

		void WrapperBegin () {
			float innerWidth = pos.width - (padding * 2f);
			float innerHeight = pos.height - (padding * 2f);

			GUILayout.BeginArea(pos); // Container
			DrawBox(new Rect(0, 0, pos.width, pos.height), backgroundColor);
			GUILayout.BeginArea(new Rect(padding, padding, innerWidth, innerHeight)); // Padding
		}

		void WrapperEnd () {
			GUILayout.EndArea();
			GUILayout.EndArea();
		}

		static void DrawBox (Rect position, Color color) {
			Color oldColor = GUI.color;

			GUI.color = color;
			GUI.Box(position, "");

			GUI.color = oldColor;
		}
	}
}
