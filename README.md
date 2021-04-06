# nimbus-unity

## Required Build Tools

- Android NDK: 21.3.6528147

## Tested Build Tools

- Android Gradle Plugin: 4.1.3
- Gradle: 6.8.3

## Tested Unity Versions

- 2021.1
- 2020.3

## Android Notes

For some reason there is an issue intantiating the OpenRTB package objects
that is fixed by including the kotlin-reflect module as a dependency.
