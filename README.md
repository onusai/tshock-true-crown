# tshock-true-crown
 Forces players to consume a teleportation potion every X seconds
 
***

### Setup
* Install the plugin and start tshock, this will generate the config file in `tshock/TrueCrown.json`
* Close tshock, and edit the config file
* The `Interval` property allows you to define the frequency (in seconds) at which players will be teleported
    - By default it is set to 60 seconds
* The `GiveItems` propety is a list of items that will be given to players on each teleport
    - By default, this list contains 10 Scarab Bombs
    - To add your own items, follow this format `"item id": amount`
        - example: `"4423": 10`
        - If the list has more than one entry, you will need to add a comma at the end of each entry (except the last one) to seperate them
* Save the config file. Note that you will always need to restart tshock after editing config file

### Commands
`/ttc`
* Toggle True Crown (pause/resume random teleportation)
* Requires `tshock.admin` permission to use (can also be used in console)

***

[Download TrueCrown.dll](https://github.com/onusai/tshock-true-crown/raw/main/bin/Debug/net6.0/TrueCrown.dll)
