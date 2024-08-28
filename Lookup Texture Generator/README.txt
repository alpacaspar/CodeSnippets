The Lookup Texture Generator can be used in the Unity Editor to generate simple textures that can be used in shaders and VFX.
See the included screenshots for a look at the tool.

The LookupTextureGenerator class (LookupTextureGenerator.cs) is the main class powering the tool, and is responsible for drawing the window and UI, and the exporting of the texture.

The generator features multiple different editors (screenshot02.png), these editors are all controlled by a separate class which implements the ILUTEditor (ILUTEditor.cs) interface.

The most complex of these editors is the Worley Noise Editor (LUTEditors/WorleyNoiseEditor.cs). This editor uses a compute shader (ComputeShaders/WorleyCompute.compute) to speed up the generation process during configuration.