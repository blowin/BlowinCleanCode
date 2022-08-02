## [2.6.0] -

- Changed highlight area for analyzers ([commit](https://github.com/blowin/BlowinCleanCode/commit/61d645eb876db1ac1b96ee3945f9513d916619d7)):
  - Switch statements should not be nested
  - Switch should not have a lot of cases
  - Deeply nested code
- Fixed disable DeeplyNestedCodeFeatureAnalyze for root block ([commit](https://github.com/blowin/BlowinCleanCode/commit/abbb40aea9d00f1f6abac00fe3ee55a34198aade))
- Name too long ([#80](https://github.com/blowin/BlowinCleanCode/issues/80), [Improved](https://github.com/blowin/BlowinCleanCode/commit/3cd153c6513ed17fa32318d266f8812a4b7b48f0))
- Use only ASCII characters for names ([commit](https://github.com/blowin/BlowinCleanCode/commit/abbb40aea9d00f1f6abac00fe3ee55a34198aade))
- Name too long ([#87](https://github.com/blowin/BlowinCleanCode/issues/87))

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
