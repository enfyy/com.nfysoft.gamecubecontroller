# Gamecube controller support V3
This package adds support for the Nintendo Gamecube controller Adapter to Unitys new input system.
Works with the original Nintendo adapter and the Mayflash Adapter in Wii U mode.
It uses a C++ Library (source located in /Plugin-source) which uses libusb-1.0 
The binaries for the plugin are in Assets/Plugin but you can also compile them yourself if you wish.

## Prerequisites:
### Windows
- Use Zadig to install the required custom driver as described [here](https://dolphin-emu.org/docs/guides/how-use-official-gc-controller-adapter-wii-u/#Windows)

### Linux
Coming soon (maybe).

## Install Package: 
Add to your manifest.json:
```json
"dependencies": {
  "com.nfysoft.gamecubecontroller": "git@github.com:enfyy/com.nfysoft.gamecubecontroller.git"  
}
```


## Usage:
- Put the GamecubeControllerAdapter script on any game object in your scene.
- Use the controller as you would use any other controller with Unitys input system.
