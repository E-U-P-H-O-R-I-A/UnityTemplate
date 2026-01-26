#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace XFurStudio2 {
    [CustomEditor(typeof(XFurStudioStrandsAsset))]
    public class XFurStudioStrandsAsset_Editor : Editor {

        public GUISkin pidiSkin2;
        public Texture2D xfurStudioLogo;

        XFurStudioStrandsAsset strandsAsset;

        bool[] folds = new bool[4];

        RenderTexture previewShape1, previewShape2, previewStrandsMap;

        public void OnEnable() {

            strandsAsset = (XFurStudioStrandsAsset)target;

        }



        public void OnDisable() {
        }


        public override void OnInspectorGUI() {

            Undo.RecordObject( strandsAsset, strandsAsset.name + "_" + strandsAsset.GetInstanceID() );

            if ( !pidiSkin2 ) {
                if ( AssetDatabase.FindAssets( "l: XFurStudio2UI" ).Length > 0 ) {
                    pidiSkin2 = (GUISkin)AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( AssetDatabase.FindAssets( "l: XFurStudio2UI" )[0] ), typeof( GUISkin ) );
                }
            }

            if ( !pidiSkin2 ) {
                GUILayout.Space( 12 );
                EditorGUILayout.HelpBox( "The needed GUISkin for this asset has not been found or is corrupted. Please re-download the asset to try to fix this issue or contact support if it persists", MessageType.Error );
                GUILayout.Space( 12 );
                return;
            }

            pidiSkin2.label = new GUIStyle( EditorStyles.label );

            var lStyle = new GUIStyle( EditorStyles.label );

            GUI.color = EditorGUIUtility.isProSkin ? new Color( 0.1f, 0.1f, 0.15f, 1 ) : new Color( 0.5f, 0.5f, 0.6f );
            GUILayout.BeginVertical( EditorStyles.helpBox );
            GUI.color = Color.white;

            AssetLogoAndVersion();

            GUILayout.Space( 16 );

            strandsAsset.xfurStrandsMethod = (XFurStudioStrandsAsset.XFurStrandsMethod)UpperCaseEnumField( new GUIContent( "Strands Texture Method", "The method that will be used to provide a fur strands texture to the XFur shader" ), strandsAsset.xfurStrandsMethod );

            GUILayout.Space( 16 );

            if ( strandsAsset.xfurStrandsMethod == XFurStudioStrandsAsset.XFurStrandsMethod.ProcedurallyGenerated ) {

                if (BeginCenteredGroup("Procedural Generation Parameters", ref folds[0] ) ) {

                    GUILayout.Space( 24 );

                    CenteredLabel( "First Pass" );

                    GUILayout.Space( 16 );

                    if ( !strandsAsset.strandShapeA.previewShape ) {
                        strandsAsset.strandShapeA.previewShape = new Texture2D( 32, 32 );
                        strandsAsset.strandShapeA.GenerateStrandShape( strandsAsset.strandShapeA.previewShape );
                    }
                    else {
                        strandsAsset.strandShapeA.GenerateStrandShape( strandsAsset.strandShapeA.previewShape );
                    }

                    GUILayout.BeginVertical();
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginHorizontal();
                    GUI.color = EditorGUIUtility.isProSkin ? new Color( 0.1f, 0.1f, 0.15f, 1 ) : new Color( 0.5f, 0.5f, 0.6f );
                    GUILayout.Box( "", EditorStyles.helpBox, GUILayout.Height( 80 ), GUILayout.Width( 80 ) );
                    GUI.color = Color.white;
                    var rect = GUILayoutUtility.GetLastRect();
                    
                    rect.x += 8;
                    rect.y += 8;
                    rect.width = 64;
                    rect.height = 64;

                    GUI.DrawTexture( rect, strandsAsset.strandShapeA.previewShape );
                    GUILayout.Space( 32 );
                    GUILayout.BeginVertical();
                    GUILayout.Space( 8 );
                    strandsAsset.strandShapeA.horizontalSize = EditorGUILayout.Slider( new GUIContent( "Horizontal Size" ), strandsAsset.strandShapeA.horizontalSize, 0, 1 );
                    strandsAsset.strandShapeA.verticalSize = EditorGUILayout.Slider( new GUIContent( "Vertical Size" ), strandsAsset.strandShapeA.verticalSize, 0, 1 );
                    strandsAsset.strandShapeA.gradient = EditorGUILayout.Slider( new GUIContent( "Gradient" ), strandsAsset.strandShapeA.gradient, 0 , 1 );
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.EndVertical();

                    GUILayout.Space( 16 );


                    strandsAsset.firstPassDensity = EditorGUILayout.Slider( new GUIContent( "Strands Density" ), strandsAsset.firstPassDensity, 0, 1 );
                    strandsAsset.firstPassSize = EditorGUILayout.Slider( new GUIContent( "Average Size" ), strandsAsset.firstPassSize, 0, 1 );
                    strandsAsset.firstPassVariation = 1.0f - EditorGUILayout.Slider( new GUIContent( "Size Variation" ), 1.0f - strandsAsset.firstPassVariation, 0, 1 );

                    GUILayout.Space( 24 );

                    CenteredLabel( "Second Pass" );

                    GUILayout.Space( 16 );

                    if ( !strandsAsset.strandShapeB.previewShape ) {
                        strandsAsset.strandShapeB.previewShape = new Texture2D( 32, 32 );
                        strandsAsset.strandShapeB.GenerateStrandShape( strandsAsset.strandShapeB.previewShape );
                    }
                    else {
                        strandsAsset.strandShapeB.GenerateStrandShape( strandsAsset.strandShapeB.previewShape );
                    }

                    GUILayout.BeginVertical();
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginHorizontal();
                    GUI.color = EditorGUIUtility.isProSkin ? new Color( 0.1f, 0.1f, 0.15f, 1 ) : new Color( 0.5f, 0.5f, 0.6f );
                    GUILayout.Box( "", EditorStyles.helpBox, GUILayout.Height( 80 ), GUILayout.Width( 80 ) );
                    GUI.color = Color.white;
                    rect = GUILayoutUtility.GetLastRect();

                    rect.x += 8;
                    rect.y += 8;
                    rect.width = 64;
                    rect.height = 64;

                    GUI.DrawTexture( rect, strandsAsset.strandShapeB.previewShape );
                    GUILayout.Space( 32 );
                    GUILayout.BeginVertical();
                    GUILayout.Space( 8 );
                    strandsAsset.strandShapeB.horizontalSize = EditorGUILayout.Slider( new GUIContent( "Horizontal Size" ), strandsAsset.strandShapeB.horizontalSize, 0, 1 );
                    strandsAsset.strandShapeB.verticalSize = EditorGUILayout.Slider( new GUIContent( "Vertical Size" ), strandsAsset.strandShapeB.verticalSize, 0 , 1 );
                    strandsAsset.strandShapeB.gradient = EditorGUILayout.Slider( new GUIContent( "Gradient" ), strandsAsset.strandShapeB.gradient, 0, 1 );
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.EndVertical();

                    GUILayout.Space( 16 );

                    strandsAsset.secondPassDensity = EditorGUILayout.Slider( new GUIContent( "Strands Density" ), strandsAsset.secondPassDensity, 0, 1 );
                    strandsAsset.secondPassSize = EditorGUILayout.Slider( new GUIContent( "Average Size" ), strandsAsset.secondPassSize, 0, 1 );
                    strandsAsset.secondPassVariation = 1.0f - EditorGUILayout.Slider( new GUIContent( "Size Variation" ), 1.0f - strandsAsset.secondPassVariation, 0, 1 );

                    GUILayout.Space( 16 );

                    strandsAsset.generateMips = EnableDisableToggle( new GUIContent( "Generate Mip Maps" ), strandsAsset.generateMips );

                    GUILayout.Space( 24 );

                    if ( CenteredButton( "Generate Strands Map", 256 ) ) {
                        strandsAsset.PoissonStrandsGenerator();
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }

                    GUILayout.Space(16);

                    if ( CenteredButton( "Export Strands Map", 256 ) ) {
                        if ( strandsAsset.FurStrands ) {
                            var pngData = strandsAsset.FurStrands.EncodeToPNG();
                            var path = EditorUtility.SaveFilePanel( "Export Texture", "Assets/", "_FurStrandsAssetOutput", "png" );

                            if ( pngData != null ) {
                                System.IO.File.WriteAllBytes( path, pngData );
                                AssetDatabase.Refresh();
                            }
                        }
                    }

                    if ( strandsAsset.FurStrands ) {
                        GUILayout.Space( 16 );
                        GUILayout.Box( "", pidiSkin2.customStyles[1], GUILayout.Height( 250 ) );
                        rect = GUILayoutUtility.GetLastRect();
                        rect.x += rect.width / 2 - 100;
                        rect.y += rect.height / 2 - 100;
                        rect.width = 200;
                        rect.height = 200;

                        GUI.DrawTexture( rect, strandsAsset.FurStrands );
                        GUILayout.Space( 16 );
                    }


                }
                EndCenteredGroup();

            }
            else {

                strandsAsset.CustomStrandsTexture = ObjectField<Texture2D>( new GUIContent( "Custom Strands Texture" ), strandsAsset.CustomStrandsTexture );

            }


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


        public void HelpBox( string message, MessageType messageType ) {
            GUILayout.Space( 8 );
            GUILayout.BeginHorizontal(); GUILayout.Space( 8 );
            GUILayout.BeginVertical( pidiSkin2.customStyles[5] );

            GUILayout.Space( 4 );
            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();

            var mType = "INFO";

            switch ( messageType ) {
                case MessageType.Error:
                    mType = "ERROR";
                    break;

                case MessageType.Warning:
                    mType = "WARNING";
                    break;
            }

            var tStyle = new GUIStyle();
            tStyle.fontSize = 11;
            tStyle.fontStyle = FontStyle.Bold;
            tStyle.normal.textColor = Color.black;

            GUILayout.Label( mType, tStyle );

            GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();
            GUILayout.Space( 4 );

            GUILayout.BeginHorizontal(); GUILayout.Space( 8 ); GUILayout.BeginVertical();
            tStyle.fontSize = 9;
            tStyle.fontStyle = FontStyle.Normal;
            tStyle.wordWrap = true;
            GUILayout.TextArea( message, tStyle );

            GUILayout.Space( 8 );
            GUILayout.EndVertical(); GUILayout.Space( 8 ); GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space( 8 ); GUILayout.EndHorizontal();
            GUILayout.Space( 8 );
        }


        public Color ColorField( GUIContent label, Color currentValue ) {

            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            currentValue = EditorGUILayout.ColorField( currentValue );
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );

            return currentValue;

        }



        /// <summary>
        /// Draws a standard object field in the PIDI 2020 style
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="label"></param>
        /// <param name="inputObject"></param>
        /// <param name="allowSceneObjects"></param>
        /// <returns></returns>
        public T ObjectField<T>( GUIContent label, T inputObject, bool allowSceneObjects = true ) where T : UnityEngine.Object {

            GUILayout.Space( 4 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            inputObject = (T)EditorGUILayout.ObjectField( inputObject, typeof( T ), allowSceneObjects );
            GUILayout.EndHorizontal();
            GUILayout.Space( 4 );
            return inputObject;
        }


        /// <summary>
        /// Draws a centered button in the standard PIDI 2020 editor style
        /// </summary>
        /// <param name="label"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public bool CenteredButton( string label, float width ) {
            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();
            var tempBool = GUILayout.Button( label, EditorGUIUtility.isProSkin?pidiSkin2.button:EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).button, GUILayout.MaxWidth( width ) );
            GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();
            GUILayout.Space( 2 );
            return tempBool;
        }

        /// <summary>
        /// Draws a button in the standard PIDI 2020 editor style
        /// </summary>
        /// <param name="label"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public bool StandardButton( string label, float width ) {
            var tempBool = GUILayout.Button( label, EditorGUIUtility.isProSkin?pidiSkin2.button:EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).button, GUILayout.MaxWidth( width ) );
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
            GUILayout.Label( strandsAsset.Version, pidiSkin2.customStyles[2] );
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

            if ( GUILayout.Button( label, EditorGUIUtility.isProSkin?pidiSkin2.button:EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).button ) ) {
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



        /// <summary>
        /// Custom integer field following the PIDI 2020 editor skin
        /// </summary>
        /// <param name="label"></param>
        /// <param name="currentValue"></param>
        /// <returns></returns>
        public int IntField( GUIContent label, int currentValue ) {

            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            currentValue = EditorGUILayout.IntField( currentValue, pidiSkin2.customStyles[4] );
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );

            return currentValue;
        }

        /// <summary>
        /// Custom float field following the PIDI 2020 editor skin
        /// </summary>
        /// <param name="label"></param>
        /// <param name="currentValue"></param>
        /// <returns></returns>
        public float FloatField( GUIContent label, float currentValue ) {

            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            currentValue = EditorGUILayout.FloatField( currentValue, pidiSkin2.customStyles[4] );
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );

            return currentValue;
        }


        /// <summary>
        /// Custom text field following the PIDI 2020 editor skin
        /// </summary>
        /// <param name="label"></param>
        /// <param name="currentValue"></param>
        /// <returns></returns>
        public string TextField( GUIContent label, string currentValue ) {

            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            currentValue = EditorGUILayout.TextField( currentValue, pidiSkin2.customStyles[4] );
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );

            return currentValue;
        }


        public Vector2 Vector2Field( GUIContent label, Vector2 currentValue ) {

            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            currentValue.x = EditorGUILayout.FloatField( currentValue.x, pidiSkin2.customStyles[4] );
            GUILayout.Space( 8 );
            currentValue.y = EditorGUILayout.FloatField( currentValue.y, pidiSkin2.customStyles[4] );
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );

            return currentValue;

        }


        /// <summary>
        /// Custom slider using the PIDI 2020 editor skin and adding a custom suffix to the float display
        /// </summary>
        /// <param name="label"></param>
        /// <param name="currentValue"></param>
        /// <param name="minSlider"></param>
        /// <param name="maxSlider"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public float SliderField( GUIContent label, float currentValue, float minSlider = 0.0f, float maxSlider = 1.0f, bool displayValue = true ) {

            GUILayout.Space( 4 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            GUI.color = Color.gray;
            currentValue = GUILayout.HorizontalSlider( currentValue, minSlider, maxSlider, GUI.skin.horizontalSlider, GUI.skin.horizontalSliderThumb );
            GUI.color = Color.white;
            if ( displayValue ) {
                GUILayout.Space( 12 );
                currentValue = Mathf.Clamp( EditorGUILayout.FloatField( float.Parse( currentValue.ToString( "n2" ) ), pidiSkin2.customStyles[4], GUILayout.MaxWidth( 40 ) ), minSlider, maxSlider );
            }
            GUILayout.EndHorizontal();
            GUILayout.Space( 4 );

            return currentValue;
        }


        /// <summary>
        /// Custom slider using the PIDI 2020 editor skin and adding a custom suffix to the float display
        /// </summary>
        /// <param name="label"></param>
        /// <param name="currentValue"></param>
        /// <param name="minSlider"></param>
        /// <param name="maxSlider"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public int IntSliderField( GUIContent label, int currentValue, int minSlider = 0, int maxSlider = 1 ) {

            GUILayout.Space( 4 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            GUI.color = Color.gray;
            currentValue = (int)GUILayout.HorizontalSlider( currentValue, minSlider, maxSlider, GUI.skin.horizontalSlider, GUI.skin.horizontalSliderThumb );
            GUI.color = Color.white;
            GUILayout.Space( 12 );
            currentValue = (int)Mathf.Clamp( EditorGUILayout.IntField( int.Parse( currentValue.ToString() ), pidiSkin2.customStyles[4], GUILayout.MaxWidth( 40 ) ), minSlider, maxSlider );
            GUILayout.EndHorizontal();
            GUILayout.Space( 4 );

            return currentValue;
        }


        /// <summary>
        /// Draw a custom popup field in the PIDI 2020 style
        /// </summary>
        /// <param name="label"></param>
        /// <param name="toggleValue"></param>
        /// <returns></returns>
        public int PopupField( GUIContent label, int selected, string[] options ) {


            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            selected = EditorGUILayout.Popup( selected, options, EditorGUIUtility.isProSkin?pidiSkin2.button:EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).button );
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );
            return selected;
        }



        /// <summary>
        /// Draw a custom toggle that instead of using a check box uses an Enable/Disable drop down menu
        /// </summary>
        /// <param name="label"></param>
        /// <param name="toggleValue"></param>
        /// <returns></returns>
        public bool EnableDisableToggle( GUIContent label, bool toggleValue, bool trueFalseToggle = false, params GUILayoutOption[] options ) {

            int option = toggleValue ? 1 : 0;

            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            if ( label != null ) {
                GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );

                if ( trueFalseToggle ) {
                    option = EditorGUILayout.Popup( option, new string[] { "FALSE", "TRUE" }, EditorGUIUtility.isProSkin?pidiSkin2.button:EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).button );
                }
                else {
                    option = EditorGUILayout.Popup( option, new string[] { "DISABLED", "ENABLED" }, EditorGUIUtility.isProSkin?pidiSkin2.button:EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).button );
                }
            }
            else {
                if ( trueFalseToggle ) {
                    option = EditorGUILayout.Popup( option, new string[] { "FALSE", "TRUE" }, EditorGUIUtility.isProSkin?pidiSkin2.button:EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).button, options );
                }
                else {
                    option = EditorGUILayout.Popup( option, new string[] { "DISABLED", "ENABLED" }, EditorGUIUtility.isProSkin?pidiSkin2.button:EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).button, options );
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );
            return option == 1;
        }


        /// <summary>
        /// Draw an enum field but changing the labels and names of the enum to Upper Case fields
        /// </summary>
        /// <param name="label"></param>
        /// <param name="userEnum"></param>
        /// <returns></returns>
        public int UpperCaseEnumField( GUIContent label, System.Enum userEnum ) {

            var names = System.Enum.GetNames( userEnum.GetType() );

            for ( int i = 0; i < names.Length; i++ ) {
                names[i] = System.Text.RegularExpressions.Regex.Replace( names[i], "(\\B[A-Z])", " $1" ).ToUpper();
            }

            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            var result = EditorGUILayout.Popup( System.Convert.ToInt32( userEnum ), names, EditorGUIUtility.isProSkin?pidiSkin2.button:EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).button );
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );
            return result;
        }


        /// <summary>
        /// Draw an enum field but without changing the labels and names of the enum to Upper Case fields
        /// </summary>
        /// <param name="label"></param>
        /// <param name="userEnum"></param>
        /// <returns></returns>
        public int StandardEnumField( GUIContent label, System.Enum userEnum ) {

            var names = System.Enum.GetNames( userEnum.GetType() );

            for ( int i = 0; i < names.Length; i++ ) {
                names[i] = names[i].ToUpper();
            }

            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            var result = EditorGUILayout.Popup( System.Convert.ToInt32( userEnum ), names, EditorGUIUtility.isProSkin?pidiSkin2.button:EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).button );
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );
            return result;
        }


        /// <summary>
        /// Draw a layer mask field in the PIDI 2020 style
        /// </summary>
        /// <param name="label"></param>
        /// <param name="selected"></param>
        public LayerMask LayerMaskField( GUIContent label, LayerMask selected ) {

            List<string> layers = null;
            string[] layerNames = null;

            if ( layers == null ) {
                layers = new List<string>();
                layerNames = new string[4];
            }
            else {
                layers.Clear();
            }

            int emptyLayers = 0;
            for ( int i = 0; i < 32; i++ ) {
                string layerName = LayerMask.LayerToName( i );

                if ( layerName != "" ) {

                    for ( ; emptyLayers > 0; emptyLayers-- ) layers.Add( "Layer " + (i - emptyLayers) );
                    layers.Add( layerName );
                }
                else {
                    emptyLayers++;
                }
            }

            if ( layerNames.Length != layers.Count ) {
                layerNames = new string[layers.Count];
            }
            for ( int i = 0; i < layerNames.Length; i++ ) layerNames[i] = layers[i];


            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );

            selected.value = EditorGUILayout.MaskField( selected.value, layerNames, EditorGUIUtility.isProSkin?pidiSkin2.button:EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).button );

            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );
            return selected;
        }



        #endregion





    }




}

#endif

