## [2.6.0] -

### New analyzers:

- Variable name too long ([#80](https://github.com/blowin/BlowinCleanCode/issues/80))

- Cognitive complexity ([#88](https://github.com/blowin/BlowinCleanCode/issues/88))

### Bug fixes:

- Fixed disable DeeplyNestedCodeFeatureAnalyze for root block ([commit](https://github.com/blowin/BlowinCleanCode/commit/abbb40aea9d00f1f6abac00fe3ee55a34198aade))

### Improvements:

- Changed highlight area for analyzers ([commit](https://github.com/blowin/BlowinCleanCode/commit/61d645eb876db1ac1b96ee3945f9513d916619d7)):
  
  - Switch statements should not be nested
  
  - Switch should not have a lot of cases
  
  - Deeply nested code

- Changed behavior of LongMethodFeatureAnalyze and LambdaHaveTooManyLinesFeatureAnalyze. They now count the actual number of lines of code. ([commit](https://github.com/blowin/BlowinCleanCode/commit/4a4b6e073a7ce0b2475a4a8d4901953d1ce392d8))

## [2.5.9] - 2022-07-29

- Fixed support visual studio 2022 ([commit](https://github.com/blowin/BlowinCleanCode/commit/52cfadc8f6e34d693040c4c0fcedd1c620ce648d))

## [2.5.8] - 2022-02-17

### Changed

- "ThreadStatic" fields should not be initialized. ([#65](https://github.com/blowin/BlowinCleanCode/issues/65))
- Middle man. ([#33](https://github.com/blowin/BlowinCleanCode/issues/33))
- VSIX package: support visual studio 2022 ([commit](https://github.com/blowin/BlowinCleanCode/commit/271127652f3ed28934a486e9c4450b9062451ced))

## [2.5.7] - 2022-01-21

### Bug fixes

- All analyzers from assembly BlowinCleanCode failed to load: Unable to load one or more of the requested types. ([#82](https://github.com/blowin/BlowinCleanCode/issues/82))
