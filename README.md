# StreamDeckX7Plugin
A Stream Deck plugin to control a Sound Blaster X7 without the need for the Sound Blaster X7 Control Panel software.

# Current capabilities
* Connects to Sound Blaster X7 units (and X7 units only; may work with X5 units, but I don't have one to test) via Bluetooth.
  * I recommend disabling the recording and playback devices that appear in Windows when you connect via Bluetooth, as these will have extra latency compared to the wired connection.
* Toggles between speakers and headphones.

# Possible future capabilities
* Set speaker mode to headphones, stereo, and multichannel audio.
* If other features of the protocol are discovered, it shouldn't be too hard to add those.

# Unlikely features
* USB protocol. Unless someone else figures this out. I don't have enough time or interest to do this at this stage.

# Sources
Based heavily on the protocol discovery work from [x7-bluetooth-control](https://github.com/Sayrus/x7-bluetooth-control) [(blog post)](https://sayr.us/reverse/soundblaster-reverse/).

Uses [StreamDeckToolkit](https://github.com/FritzAndFriends/StreamDeckToolkit) to implement a Stream Deck plugin in C#.
