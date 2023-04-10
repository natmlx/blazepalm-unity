## 1.0.2
+ Added `BlazePalmPredictor.Tag` constant string for embedding the edge model.
+ Upgraded to NatML 1.1.4.

## 1.0.1
+ Added `BlazePalmPipeline.Create` static method for creating the pipeline.
+ Added `BlazePalmPredictor.Create` static method for creating the predictor.
+ Improved memory behaviour of `BlazePalmPipeline` by removing new ROI image allocations.
+ Removed `BlazePalmPipeline` public constructor. Use the `BlazePalmPipeline.Create` method instead.
+ Removed `BlazePalmPredictor` public constructor. Use the `BlazePalmPredictor.Create` method instead.
+ Upgraded to NatML 1.1.3.

## 1.0.0
+ First release.