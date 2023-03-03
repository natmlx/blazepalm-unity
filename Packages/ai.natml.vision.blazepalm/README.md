# BlazePalm
Palm landmark prediction with MediaPipe Hands in Unity Engine.

## Installing BlazePalm
Add the following items to your Unity project's `Packages/manifest.json`:
```json
{
  "scopedRegistries": [
    {
      "name": "NatML",
      "url": "https://registry.npmjs.com",
      "scopes": ["ai.natml"]
    }
  ],
  "dependencies": {
    "ai.natml.vision.blazepalm": "1.0.1"
  }
}
```

## Predicting Hands in an Image
First, create the BlazePalm pipeline:
```csharp
// Create the BlazePalm pipeline
var pipeline = await BlazePalmPipeline.Create();
```

Then detect hands in the image:
```csharp
// Detect hands in the image
Texture2D image = ...;
BlazePalmPredictor.Hand[] hands = pipeline.Predict(image);
```

___

## Requirements
- Unity 2021.2+

## Quick Tips
- Join the [NatML community on Discord](https://natml.ai/community).
- Discover more ML models on [NatML Hub](https://hub.natml.ai).
- See the [NatML documentation](https://docs.natml.ai/unity).
- Contact us at [hi@natml.ai](mailto:hi@natml.ai).

Thank you very much!