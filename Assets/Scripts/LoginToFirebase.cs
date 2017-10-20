using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;


public class LoginToFirebase : MonoBehaviour {

	Firebase.Auth.FirebaseAuth auth;
	Firebase.Auth.FirebaseUser user;

	void Start() {
		// Set this before calling into the realtime database.
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://unity-firebase-hello-world.firebaseio.com/");
		FirebaseApp.DefaultInstance.SetEditorP12FileName(Application.dataPath + "/FirebaseAssetPackage/Editor Default Resources/Unity Firebase Hello World-1e8c83ab0a0c.p12");
		FirebaseApp.DefaultInstance.SetEditorServiceAccountEmail("unity-editor@unity-firebase-hello-world.iam.gserviceaccount.com");
		FirebaseApp.DefaultInstance.SetEditorP12Password("notasecret");
		Debug.Log ("Using P12 for Firebase Access in Editor");

		InitializeFirebase ();
		ReadDatabase ();

	}


	void InitializeFirebase() {
		Debug.Log("Setting up Firebase Auth");
		auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
		auth.StateChanged += AuthStateChanged;
		AuthStateChanged(this, null);
	}

	void ReadDatabase() {

		DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

		FirebaseDatabase.DefaultInstance
			.GetReference("/")
			.GetValueAsync().ContinueWith(task => {
				if (task.IsFaulted) {
					// Handle the error...
				}
				else if (task.IsCompleted) {
					DataSnapshot snapshot = task.Result;
					// Do something with snapshot...

					var databaseItems = snapshot.Value as Dictionary<string, object>;

					foreach (var item in databaseItems)
					{
						Debug.Log(item.Key); 
						var values = item.Value as Dictionary<string, object>;
						foreach (var v in values)
						{
							Debug.Log(v.Key + ": " + v.Value); 
						}
					}
				}
			});
	}

	void AuthStateChanged(object sender, System.EventArgs eventArgs) {
		if (auth.CurrentUser != user) {
			
			bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

			if (!signedIn && user != null) {
				Debug.Log("Signed out " + user.UserId);
			}
			user = auth.CurrentUser;

			if (signedIn) {
				Debug.Log("Signed in " + user.UserId);
			}
		}
	}

}
