#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace XFurStudio2 {
    
    [CustomEditor(typeof(XFurStudioDatabase))]
    public class XFurStudioDatabase_Editor:Editor {

        public GUISkin pidiSkin2;
        public Texture2D xfurStudioLogo;

        XFurStudioDatabase database;

        bool[] folds;
        bool profilesFold;
        bool mainFold;

        public void OnEnable() {
            database = (XFurStudioDatabase)target;

            database.LoadResources();

        }

        public override void OnInspectorGUI() {

            if ( !pidiSkin2 ) {
                GUILayout.Space( 12 );
                EditorGUILayout.HelpBox( "The needed GUISkin for this asset has not been found or is corrupted. Please re-download the asset to try to fix this issue or contact support if it persists", MessageType.Error );
                GUILayout.Space( 12 );
                return;
            }

            pidiSkin2.label = new GUIStyle( EditorStyles.label );
            var lStyle = new GUIStyle(EditorStyles.label);

            GUI.color = EditorGUIUtility.isProSkin ? new Color( 0.1f, 0.1f, 0.15f, 1 ) : new Color( 0.5f, 0.5f, 0.6f );
            GUILayout.BeginVertical( EditorStyles.helpBox );
            GUI.color = Color.white;

            AssetLogoAndVersion();


            GUILayout.Space( 16 );
            CenteredLabel( "XFur Studio™ 2 Database, "+database.Version );
            GUILayout.Space( 16 );

           

            GUILayout.BeginHorizontal(); GUILayout.Space( 32 ); GUILayout.BeginVertical();

            database.RenderingMode = (XFurStudioDatabase.XFurRenderingMode)EditorGUILayout.EnumPopup( new GUIContent( "Rendering System" ), database.RenderingMode, EditorGUIUtility.isProSkin?pidiSkin2.button:EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).button );

            GUILayout.Space( 16 );


            GUILayout.Label( "XFur Shells Renderer System : " + (database.XFShellsReady ? "Ready" : "Not Found"), pidiSkin2.label );
            GUILayout.Label( "Basic Shells System : " + (database.BasicReady ? "Ready" : "Not Found"), pidiSkin2.label );
            GUILayout.Label( "URP System : " + (database.URPReady ? "Ready" : "Not Found"), pidiSkin2.label );
            GUILayout.Label( "HDRP System : " + (database.HDRPReady ? "Ready" : "Not Found"), pidiSkin2.label );

            GUILayout.Space( 16 );

            if ( !database.XFShellsReady && database.RenderingMode == XFurStudioDatabase.XFurRenderingMode.Standard ) {
                EditorGUILayout.HelpBox( "The XFShells shaders for the Built-in renderer could not be found. Importing the Standard Pipeline folder included with this asset is necessary to locate and load these shaders and use XFur Studio with the Built-in renderer.", MessageType.Warning );
            }

            if ( !database.BasicReady && database.RenderingMode == XFurStudioDatabase.XFurRenderingMode.Standard ) {
                EditorGUILayout.HelpBox( "The Basic Shells shaders for the Built-in renderer could not be found. Importing the Standard Pipeline folder included with this asset is necessary to locate and load these shaders and use XFur Studio with the Built-in renderer.", MessageType.Warning );
            }

            if ( !database.URPReady && database.RenderingMode == XFurStudioDatabase.XFurRenderingMode.Universal ) {
                EditorGUILayout.HelpBox( "The URP specific shaders could not be found. Unpacking the Universal RP unity package included with this asset is necessary to locate and load these shaders and use XFur Studio with Universal RP.", MessageType.Warning );
            }
            else if (database.RenderingMode == XFurStudioDatabase.XFurRenderingMode.Universal) {
                EditorGUILayout.HelpBox( "Please remember that, while XFur Studio™ 2 may work with newer Universal RP releases, official support for Universal RP is limited to Universal RP 7.x in Unity 2019.4", MessageType.Info );
            }
            
            if ( !database.HDRPReady && database.RenderingMode == XFurStudioDatabase.XFurRenderingMode.HighDefinition ) {
                EditorGUILayout.HelpBox( "The HDRP specific shaders could not be found. Unpacking the High Definition RP unity package included with this asset is necessary, in order to locate and load these shaders and use XFur Studio with the High Definition RP.", MessageType.Warning );
            }
            else if ( database.RenderingMode == XFurStudioDatabase.XFurRenderingMode.HighDefinition ) {
                EditorGUILayout.HelpBox( "Please remember that, while XFur Studio™ 2 may work with newer High Definition RP releases, official support for High Definition RP is limited to HDRP 7.x in Unity 2019.4", MessageType.Info );
            }

            GUILayout.EndVertical(); GUILayout.Space( 32 ); GUILayout.EndHorizontal();

            GUILayout.Space( 16 );

           
            EditorUtility.SetDirty( database );

            GUILayout.Space( 16 );

            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();

            lStyle = new GUIStyle(EditorStyles.label);
            lStyle.fontStyle = FontStyle.Italic;
            lStyle.fontSize = 8;

            GUILayout.Label( "Copyright© 2017-2020,   Jorge Pinal N.", lStyle );

            GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();

            GUILayout.Space( 24 );
            GUILayout.EndVertical();


        }




#region PIDI 2020 EDITOR


       


        /// <summary>
        /// Draws a centered button in the standard PIDI 2020 editor style
        /// </summary>
        /// <param name="label"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public bool CenteredButton( string label, float width ) {
            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();
            var tempBool = GUILayout.Button( label, pidiSkin2.button, GUILayout.MaxWidth( width ) );
            GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();
            GUILayout.Space( 2 );
            return tempBool;
        }

        
        /// <summary>
        /// Draws the asset's logo and its current version
        /// </summary>
        public void AssetLogoAndVersion() {

            GUILayout.BeginVertical( xfurStudioLogo, pidiSkin2 ? pidiSkin2.customStyles[1] : null );
            GUILayout.Space( 45 );
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label( database.Version, pidiSkin2.customStyles[2] );
            GUILayout.Space( 6 );
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Draws a label centered in the Editor window
        /// </summary>
        /// <param name="label"></param>
        public void CenteredLabel( string label ) {

            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();
            GUILayout.Label( label, pidiSkin2.label );
            GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();

        }

        /// <summary>
        /// Begins a custom centered group similar to a foldout that can be expanded with a button
        /// </summary>
        /// <param name="label"></param>
        /// <param name="groupFoldState"></param>
        /// <returns></returns>
        public bool BeginCenteredGroup( string label, ref bool groupFoldState ) {

            if ( GUILayout.Button( label, pidiSkin2.button ) ) {
                groupFoldState = !groupFoldState;
            }
            GUILayout.BeginHorizontal(); GUILayout.Space( 12 );
            GUILayout.BeginVertical();
            return groupFoldState;
        }


        /// <summary>
        /// Finishes a centered group
        /// </summary>
        public void EndCenteredGroup() {
            GUILayout.EndVertical();
            GUILayout.Space( 12 );
            GUILayout.EndHorizontal();
        }
      


#endregion




    }



    class XFurPostProcessor : AssetPostprocessor {
        static void OnPostprocessAllAssets( string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths ) {
            
            foreach ( string str in importedAssets ) {
                if ( AssetDatabase.GetMainAssetTypeAtPath( str ) == typeof( XFurStudioDatabase ) ) {
                    AssetDatabase.LoadAssetAtPath<XFurStudioDatabase>( str ).LoadResources();
                }
            }
        }
    }


}

#endif