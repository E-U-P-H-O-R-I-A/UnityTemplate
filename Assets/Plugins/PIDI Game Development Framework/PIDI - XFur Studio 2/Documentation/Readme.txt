Thank you very much for purchasing one of our assets.

IMPORTANT : When updating to a new version of XFur Studio 2, if you are using an SRP (Universal or High Definition) remember to unpack the corresponding unitypackage files again as well.
This will ensure that your SRP specific assets and shaders are up to date and prevent any errors.

Whenever you update to a newer release of XFur Studio 2, select your database and open their inspectors. This will force the database to create and load any new resources added after an update.

Before you start developing with XFur Studio™ 2, we recommend you to familiarize yourself with the tool by reading its documentation thoroughly.

You can consult the online version of the manual here : https://pidiwiki.irreverent-software.com/wiki/doku.php?id=xfur_studio_2

The online documentation is updated frequently and contains additional materials such as videos and gifs that may make your learning experience easier.
If you need assistance with this product please contact us at support@irreverent-software.com with the following information : 

- Invoice or proof of purchase that includes your invoice number
- PIDI product and version used
- Unity version used
- Rendering Pipeline
- Graphics system (DirectX, Metal, OpenGL, OpenGLES, Vulkan, etc)
- Relevant screenshots / videos / steps to reproduce the issue

Thank you again for purchasing XFur Studio™ 2, we hope it will be a great addition for your projects!

The Irreverent Software™ team.



CHANGELOG : 


2.2.1 : 

Improvements : 

• Translucency / Transmission strength can now be controlled in all rendering pipelines
• Grooming algorithm can be changed in HDRP to a variant that better matches grooming in Standard and URP (changing Grooming Algorithm in the material within the database to 0 or 1 switches between algorithms)
• Translucency and Anisotropic Highlights can be turned off in URP directly in the material within the database by switching the Rendering Mode value to 0.



2.2.0 : 

Improvements & Fixes :

• Curly Fur is now stable and can be enabled with the Advanced Properties setting
• URP and HDRP 10.x packages now included for Unity 2020.2
• Bug with the URP variants that prevented rim lighting features from working as expected

New Features : 

• Fur Rendering 2.0 in HDRP with a new Hair shading model. Support for translucency and anisotropic highlights
• Fur Rendering 2.0 in URP with partial support for translucency and full support for anisotropic highlights
• Fur Rendering 2.0 in Built-in renderer with partial support for translucency and partial support for anisotropic highlights (main directional light only)
• Emissive fur in all pipelines both by emission map and emission colors
• Emissive decals in the decals module
• Experimental overdraw reduction feature in the LOD module
• Improved editor inspectors with support for both light and dark Unity themes and with a better integration with the editor itself



2.1.8 :

• Fixed an error with the new visibility mode
• Changed all uses properties in Material Property Blocks to use PropertyIDs instead of strings for performance and ease of use in the future. Properties are static for easy access


2.1.7 :

Improvements & Fixes :

• New custom visibility test in order to ensure that the fur's culling is not delayed. Before, in very fast moving objects, there could be a noticeable delay between the object becoming visible and the fur appearing, due to the use of the isVisible flag.
• Models can now be freely scaled when using XFur Studio Designer and their colliders will be adjusted accordingly for an easier painting.
• XFurStudioAPI for painting no longer locked to 1,1,1 scales. However, it has become slightly more demanding due to a few additional matrix transformations being implemented.
• XFur Studio Designer now has an improved UI with bigger font sizes, better scaling and easier to use sliders. These changes were planned for version 2.2.0 but were pushed forward as a very necessary QoL improvement


2.1.5 : 

Improvements & Fixes :

• General fixes to fur grooming in the HDRP variant of XFur Studio 2. Results are more consistent, they work better from within the HDRP workflow and the results respect global rotation and position of the mesh / armatures.

2.1.3 :

Improvements & Fixes :

• Dynamic LOD module no longer needs the Auto-update materials flag to be enabled in order to work.
• Internal code for updating materials and rendering fur has been improved
• New function to set fur profile assets through code has been added : SetFurProfileAsset and SetFurProfileAssetExt. Their functionality is the same as the SetFurDataFull and SetFurData functions that already exist, but they take a FurProfile asset as argument instead of the actual fur data contained within the asset, which should make its use much easier.
• Improved compatibility with Unity 2020.x

2.1.2 :

• New per-FX parameters in the VFX module (smoothness, penetration / interaction with fur strands) in HDRP and Universal RP
• Updated documentation with more beginner friendly language and more detailed descriptions / steps in some points


2.1.1 : 

New Features :

• New per-FX parameters in the VFX module (smoothness, penetration / interaction with fur strands)

Fixes and Improvements :

• Improvements to snow and rain effects in the VFX module
• Fixed a bug when changing the assigned renderer and replacing it with one with less material slots
• Fur strands R and G modifier parameters leave Beta and move into a new stable "Advanced settings" mode.
• Fur Rim boost leaves Beta.
• Fixed a bug that prevented physics and VFX from working on some multi-material models


2.1.0 : 

New features :

• Curly Fur is now supported ! (BETA)
• Fur strands now have more variation to produce more natural looking fur
• Fur coloration modifier of each fur strand pass can now be manually adjusted
• Fur rim color can now be boosted manually for better results
• Fur strands can be manually curled in both X and Y axes to produce curly / messy fur
• Custom, hand made fur strands textures for curly fur types


Fixes and improvements ;

• Improved Rim color mixing to allow for a wider variety of effects
• Improved fur strand seamless repetition
• Improved texture projection to reduce visible tiling has been ported to HDRP and URP
• Improved wind simulation has been ported to HDRP and URP
• Improved fur shadowing and shadows in HDRP and URP




2.0.7 :

Fixes and improvements : 

• Highly improved wind simulation
• Improved auto-scale behavior
• General QoL fixes for XFur Studio Designer
• Improved tiling for procedurally generated fur strands

New features : 

• New samples / tutorials
• New simple 3D model to showcase fur, use as tutorial and, in future updates, newer features
• New fur strands sampling method to reduce visible tiling
• New fur strand parameters to control per-pass size variation
• Procedurally generated fur strand maps can now be exported as .png textures for easy editing in external software such as Photoshop / GIMP.

2.0.6 :

Fixes and improvements : 

• Scaling works without issues when using Skinned Meshes
• Fur length can be automatically adjusted for scale with the new "Auto Adjust for Scale" toggle (should be turned off for non skinned meshes)


2.0.5 : 

Fixes and improvements :

• Universal RP demo now opens without issues
• Clarified language in the documentation to avoid confusion when adding XFur Studio Instance to a model
• Improved XFur Studio Designer UI
• Added option to zoom with "+" and "-" keys within XFur Studio Designer
• Other minor quality of life updates through the whole asset.

2.0.2 :

Fixes :

• Changing the scale of a mesh now updates the fur rendering immediately

2.0.1

Fixes and improvements :

• FX rendering for Basic Shells now matches 1 to 1 the FX rendering of XFShells
• Automatic fix for script execution order issues. May still need manual setups, but it should prevent most incompatibility issues with third party assets using custom script execution order setups.
• Adjusted rain direction vector
• Fixed incorrect setting in URP demo

New Features :

• Per material double sided fur settings (XFShells only). Select your database asset in order for it to auto-generate the double sided variants of XFur materials, then press Ctrl + S to force Unity to show the new assets.
• Per material light probe settings (XFShells only)
• Wind direction and strength now have adjustable influence over the rain and snow directions
• XFur Wind Zone component has a new, easier to use UI
• Exposed Fur Smoothness parameter on all fur materials

2.0.0

Bug Fix for 2018.4.26 :

• XFur Designer does not open. A corrupted asset due to a bug with Unity and metadata in the Custom Render Texture produces a crash in the Editor.