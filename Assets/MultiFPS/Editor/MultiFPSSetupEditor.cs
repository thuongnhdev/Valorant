using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MultiFPS
{
    [CustomEditor(typeof(MultiFPSSetup))]
    public class MultiFPSSetupEditor : Editor
    { 
        [Tooltip("(This will setup script execution order for necessary scripts, and set tags and collision layers that MultiFPS uses)")]
        public override async void OnInspectorGUI()
        {

            if (GUILayout.Button("Setup MultiFPS"))
            {
                await LayerSetupEditor.SetupLayers();
                TagSetupEditor.SetupTags();
            }
        }
    }
}