﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
using System;

public class PopulateLayout : MonoBehaviour
{
    public GameObject prefab, bounds;  
	// prefab: Populate our Scroll bar with button prefab
	// bounds: Cloth model from blender 
	public int numberToCreate;
	public Sprite newSprite;
	public Button newButton;
	string path;
	public UnityEngine.Object[] sprites;  // To load patterns in resource folder
	public List<string> fileList;  // Saves filenames in Pattern folder
	public Dictionary<string, bool> fileDict;
	public Renderer render;

    void Start() {
		int count = -6;  // Number of predefined patterns = 5
		path = Path.Combine(Application.persistentDataPath, "Pattern");
		
		fileDict = new Dictionary<string, bool>();
		fileList = new List<string>();

		bounds = GameObject.Find("bounds");
		render = bounds.GetComponentInChildren<Renderer>();
    	render.enabled = true;
		
		// Loading predefined patterns
		sprites = Resources.LoadAll("Sprites", typeof(Sprite));
		foreach(var s in sprites) {
			count++;
			GameObject newObj = (GameObject)Instantiate(prefab, transform);
			newButton = newObj.GetComponent<Button>();
			newButton.image.sprite = (Sprite)s;
			newButton.GetComponentInChildren<Text>().text = "" + count;
			newButton.onClick.AddListener(OnClickImageSprite);
		}
		GenerateList(path);
    }

	void OnClickImageSprite() {
		int fileNumber = Convert.ToInt32(EventSystem.current.currentSelectedGameObject.GetComponent<Button>().GetComponentInChildren<Text>().text);
		fileNumber = fileNumber + 5;
		Debug.Log("PopulateLayout.OnClickImageSprite() ===> File number: " + fileNumber);
		Debug.Log("PopulateLayout.OnClickImageSprite() ===> Sprite: " + sprites[fileNumber]);
		
		Texture2D LoadedImage = ((Sprite)sprites[fileNumber]).texture;
        render.material.mainTexture = LoadedImage;
	}

	public void GenerateList(String path) {
		// Loading images in Pattern folder as Sprites 
		Debug.Log("PopulateLayout.GenerateList() ===> Pattern folder path: " + path);
		if(Directory.Exists(path)) {
			foreach (string file in System.IO.Directory.GetFiles(path)) {
				/*** TODO: Add more extensions ***/
				Debug.Log("Inside foreach at GenerateList()");
				if (file.EndsWith(".png") || file.EndsWith(".jpg")) {
					/*** TODO: Check this condition ***/
					if(!fileDict.ContainsKey(file)) {
						fileDict.Add(file, false);
						Debug.Log("Added to File Dict: " + fileDict);
					}
				}
			}
			Populate();
			Debug.Log("Out Populate()");
		}
		else {
			Debug.Log("PopulateLayout.GenerateList() ===> Directory not found: " + path);
		}
	}

	void Populate() {
		// Creating new pattern button
		Debug.Log("In Populate()" + fileDict);
		foreach(KeyValuePair<string, bool> entry in fileDict) {

			byte[] fileData;

			if(!entry.Value) {
				GameObject newObj = (GameObject)Instantiate(prefab, transform);
				Texture2D texture = null;
				fileList.Add(entry.Key);
				Debug.Log("FileDict Key: " + entry.Key);
				fileData = File.ReadAllBytes(entry.Key);
				texture = new Texture2D(2, 2);
				texture.LoadImage(fileData); 
				newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0,0), 100.0f);

				newButton = newObj.GetComponent<Button>();
				newButton.image.sprite = newSprite;
				newButton.GetComponentInChildren<Text>().text = "" + (fileList.Count - 1);
				newButton.onClick.AddListener(OnClickImage);

				// fileDict[entry.Key] = true;
			}
		}
		foreach(string file in fileList) {
			if(fileDict.ContainsKey(file)){
				fileDict[file] = true;
			}
		}
	}

	void OnClickImage() {
		int fileNumber = Convert.ToInt32(EventSystem.current.currentSelectedGameObject.GetComponent<Button>().GetComponentInChildren<Text>().text);
		string filepath = fileList[fileNumber];
		Debug.Log("PopulateLayout.OnClickImage() ===> Filepath of clicked image: " + filepath);
		
		// Updating cloth model's pattern
		byte[] byteArray = File.ReadAllBytes(@filepath);
        Texture2D LoadedImage = new Texture2D(2, 2);
        LoadedImage.LoadImage(byteArray);
        render.material.mainTexture = LoadedImage;
	}
}
