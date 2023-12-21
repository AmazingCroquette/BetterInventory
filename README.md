# Better Inventory
BetterInventory adds some inventory related quality of life features while keeping the game design as close as possible to the original.

## Updates
### 0.2.2
- Fixed an issue where Swap mode could make items disappear by overriding an item by picking up a new one.
- Fixed an issue where inventory slot frames were changing color to black.
- Fixed an issue where the inventory was not properly fading out after using easy inventory and swap mode.

### 0.2.1
- Swap Mode: Allows the player to reorganize their inventory without throwing everything on the ground. (uses X by default, change bindings in-game)

### 0.1.0
- Bindings to easily swap between inventory slots. (uses 1 2 3 4 by default, change bindings in-game)
- Binding for a global flashlight button for easy access. (uses T by default, change bindings in-game)

# Features
## Easy Inventory:
Instead of using inventory previous and next (default to mouse wheel), easy inventory allows you to set your own bindings (default to 1, 2, 3, 4).
Use the inventory bindings for easier inventory navigation and set your own bindings

\**Currently only supports 4 slots, I plan to make this dynamic at some point*

## Global Flashlight:
Toggle your flashlight on and off easily with a global keybind even while holding heavy objects. (default to T)

## Swap Mode:

Swap Mode aims at reducing the unnecessary need to throw your inventory to the ground for sorting.
Press the Swap Mode binding (default to X) to enable or disable Swap Mode.
When enabled, swap modes highlights the current slot and waits for the next input to swap the 2 positions together by using:
- The easy inventory feature to select an end slot directly. Swap mode deactivates itself afterwards.
- The inventory previous and next bindings to move the current item wherever you want it to go. Swap mode needs to be disabled manually.

\**Swap Mode doesn't work while transporting heavy objects or while using the terminal*
