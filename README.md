# Procedural Solar System Generation

Procedural Solar System Generation is a Unity project that generates a solar system with stars, planets, and moons using procedural generation techniques.

## Table of Contents

- [Installation](#installation)
- [Features](#features)
- [Usage](#usage)
- [Customization](#customization)
- [Contributing](#contributing)
- [License](#license)

## Installation

1. Clone the repository
2. Open the project in Unity
3. Open the `MainScene` scene located in the `Assets/Scenes` folder
4. Press the Play button in the Unity editor to start the simulation

## Features

- Generation of one or more stars with random size, position, and emission
- Generation of one or more planets for each star using the Titius-Bode formula to determine their position and Kepler's laws to determine their speed and orbit
- Generation of one or more moons for each planet with random size and orbit
- Physics-based gravity calculations between celestial objects
- Procedural generation allows for the creation of unique solar systems with just a few clicks

## Usage

The `MainScene` scene contains a `SolarSystemGenerator` script that generates the solar system. The script generates a specified number of stars and a specified number of planets orbiting the first star. 

To customize the solar system, you can modify the following properties in the `SolarSystemGenerator` script:

- `numberOfStars`: The number of stars to generate
- `numberOfPlanets`: The number of planets to generate orbiting the first star
- `maxNumberOfPlanets`: The maximum number of planets to generate orbiting a star

The solar system simulation can be run in the Unity editor by pressing the Play button.

## Customization

### Stars

Stars are generated using the `StarGenerator` script. To customize the stars, you can modify the following properties in the `StarGenerator` script:

- `minStarRadius`: The minimum radius of the star
- `maxStarRadius`: The maximum radius of the star
- `starSubdivisions`: The number of subdivisions used to generate the star's mesh
- `starOrbitDistance`: The distance from the origin at which the star is generated

### Planets

Planets are generated using the `PlanetGenerator` script. To customize the planets, you can modify the following properties in the `PlanetGenerator` script:

- `maxNumberOfMoonsPerPlanet`: The maximum number of moons that can be generated for a planet
- `minPlanetRadius`: The minimum radius of the planet
- `maxPlanetRadius`: The maximum radius of the planet
- `planetSubdivisions`: The number of subdivisions used to generate the planet's mesh
- `orbitalSpeedMultiplier`: The multiplier used to calculate the planet's orbital speed
- `rotationSpeedMultiplier`: The multiplier used to calculate the planet's rotation speed

### Moons

Moons are generated using the `MoonGenerator` script. To customize the moons, you can modify the following properties in the `MoonGenerator` script:

- `minMoonRadius`: The minimum radius of the moon
- `maxMoonRadius`: The maximum radius of the moon
- `moonSubdivisions`: The number of subdivisions used to generate the moon's mesh
- `moonOrbitDistance`: The distance from the planet at which the moon is generated
- `rotationSpeedMultiplier`: The multiplier used to calculate the moon's rotation speed
- `orbitalSpeedMultiplier`: The multiplier used to calculate the moon's orbital speed

## Contributing

Contributions to Procedural Solar System Generation are welcome and encouraged! To contribute, please follow these steps:

1. Fork the repository
2. Create a new branch for your contribution
3. Make your changes
4. Test your changes
5. Submit a pull request

## License

This project is licensed under the Creative Commons Zero v1.0 Universal license (CC0). You are free to use the material for any purpose, without any conditions, including commercial use. You can copy, modify, distribute and perform the work, even for commercial purposes, without asking permission.
