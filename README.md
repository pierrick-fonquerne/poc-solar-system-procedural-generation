# Procedural Solar System Generation

Procedural Solar System Generation is a Unity (HDRP) project that generates a solar system with stars, planets, and moons using procedural generation techniques.

## Table of Contents

- [Installation](#installation)
- [Features](#features)
- [Usage](#usage)
- [Customization](#customization)
- [Controls](#controls)
- [Tests and CI](#tests-and-ci)
- [Contributing](#contributing)
- [License](#license)

## Installation

1. Clone the repository
2. Open the project in Unity (2022.3 LTS)
3. Open the `MainScene` scene located in the `Assets` folder
4. Press the Play button in the Unity editor to start the simulation

## Features

- Generation of one or more stars with random size and HDRP physical lighting
- Generation of one or more planets for each star using the Titius-Bode formula to determine their position and Kepler's laws to determine their orbital speed
- Procedural terrain on planets and moons using layered (fBm) 3D noise, consistent across LOD levels
- Level-of-detail (LOD) system that swaps planet meshes based on camera distance, with hysteresis and optional benchmarking
- Generation of one or more moons for each planet with random size and orbit
- Reproducible generation through a configurable seed

## Usage

The `MainScene` scene contains a `SolarSystemGenerator` component that generates the solar system at Play time. All settings are exposed in the Inspector:

- `useRandomSeed`: When enabled, a new random seed is picked on every run
- `seed`: The seed used for generation; the same seed reproduces the same solar system
- `numberOfStars`: The number of stars to generate
- `numberOfPlanets`: The number of planets to generate orbiting the first star
- `maxNumberOfPlanets`: The maximum number of planets to generate orbiting a star

## Customization

The star, planet, and moon generators are configured directly on the `SolarSystemGenerator` component in the Inspector.

### Stars (`starGenerator`)

- `minStarRadius` / `maxStarRadius`: The radius range of the star
- `starSubdivisions`: The number of subdivisions used to generate the star's mesh
- `starSeparation`: The distance between two consecutive stars
- `lumensPerRadiusUnit`: Light intensity (in lumens) per unit of star radius
- `emissionColor` / `emissionIntensity`: Emissive appearance of the star surface

### Planets (`planetGenerator`)

- `maxNumberOfMoonsPerPlanet`: The maximum number of moons that can be generated for a planet
- `minPlanetRadius` / `maxPlanetRadius`: The radius range of the planet
- `planetSubdivisions`: The number of subdivisions of the highest LOD mesh
- `orbitalSpeedMultiplier` / `rotationSpeedMultiplier`: Multipliers applied to the Kepler-derived orbital speed and the random rotation speed
- `terrainFrequency` / `terrainRelief`: Feature size and maximum elevation (as a fraction of the radius) of the procedural terrain

### Moons (`moonGenerator`)

- `minMoonRadius` / `maxMoonRadius`: The radius range of the moon
- `moonSubdivisions`: The number of subdivisions used to generate the moon's mesh
- `moonOrbitDistance`: The spacing between consecutive moon orbits
- `rotationSpeedMultiplier` / `orbitalSpeedMultiplier`: Multipliers applied to the rotation and orbital speeds
- `terrainFrequency` / `terrainRelief`: Procedural terrain settings for the moon surface

### Level of detail (`Assets/Resources/Settings/PlanetLodConfig.asset`)

- `transitionDistances`: Camera distances at which the planet switches to a lower-detail mesh
- `transitionHysteresis`: Distance margin preventing rapid back-and-forth switching
- `updateInterval`: How often (in seconds) the LOD is re-evaluated
- `enableBenchmarking`: Logs LOD evaluation timings to the console

## Controls

- **Mouse**: Look around (press `L` to lock/unlock the camera)
- **Arrow keys**: Move forward/backward/left/right
- **Space / Left Ctrl**: Move up / down
- **A / E**: Roll the camera
- **Mouse wheel**: Zoom

## Tests and CI

EditMode tests live in `Assets/Tests/EditMode` and cover the icosphere topology, the procedural terrain determinism, and the orbital math. Run them from the Unity Test Runner window (`Window > General > Test Runner`).

The GitHub Actions workflow (`.github/workflows/unity-ci.yml`) runs the tests with [GameCI](https://game.ci/) on every push and pull request targeting `main`. It requires the `UNITY_LICENSE`, `UNITY_EMAIL`, and `UNITY_PASSWORD` repository secrets ([activation guide](https://game.ci/docs/github/activation)).

## Contributing

Contributions to Procedural Solar System Generation are welcome and encouraged! To contribute, please follow these steps:

1. Fork the repository
2. Create a new branch for your contribution
3. Make your changes
4. Test your changes
5. Submit a pull request

## License

This project is licensed under the Creative Commons Zero v1.0 Universal license (CC0). You are free to use the material for any purpose, without any conditions, including commercial use. You can copy, modify, distribute and perform the work, even for commercial purposes, without asking permission.
