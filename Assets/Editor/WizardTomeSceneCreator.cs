using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public static class WizardTomeSceneCreator
{
    [MenuItem("WizardTome/Create Prototype Scene")]
    public static void CreateScene()
    {
        System.IO.Directory.CreateDirectory("Assets/Scenes");
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Prototype.unity");
        AssetDatabase.Refresh();
        EditorSceneManager.OpenScene("Assets/Scenes/Prototype.unity");
        UnityEngine.Debug.Log("WizardTome scene created. Press Play to explore.");
    }
}
