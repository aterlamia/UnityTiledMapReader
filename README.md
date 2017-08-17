# Tiled map loader

This project contains the files needed to read tiled maps.


It will take the tile layer at position 1 (first layer) and the first tile set to build your map in unity.

Also it will take information from object layers and convert them to collision objects.


## Tile layer
Important for now the scrip will only read the first layer and first tileset in the tiled map it will store the extra layers but does not use them yet, keep this in mind when exporting and when you get errors.

## Object layer
Every object layer will be read but it will only check for 2 types of objects for now

 - A rectangle object will be converted to a box collider
 - A "hill" object, which is basicly any of the objects with the type "hill" added to the object. This will use the polygon2d collider. Only tested with an object with 4 verteces

Names given to the objects will be added to the game object in unity for easy recognision

## Example
The project here is an as is working example including some home baked tiles and a map. This map should give you an idea how to use this script.

## Resources
For your resources to be found they have to be in the resources directory (as is normal for unity)

### Map resource
Tiled saves the map as an TMX file this is however a normal xml file, for unity to use it you have to save it as an XML. Tiled will warn you it might not be able to open it but in my experience it works without any problems but in the example both the TMX as the XML are included.

