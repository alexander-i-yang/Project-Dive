<?xml version="1.0" encoding="UTF-8"?>
<tileset version="1.9" tiledversion="1.9.2" name="Mechanics" tilewidth="24" tileheight="24" tilecount="7" columns="0">
 <grid orientation="orthogonal" width="1" height="1"/>
 <tile id="0" class="Blue Crystal">
  <image width="8" height="8" source="../Sprites/Mechanics/Crystal Blue.png"/>
 </tile>
 <tile id="1" class="Moving Box">
  <image width="24" height="24" source="../Sprites/Mechanics/Moving Box.png"/>
 </tile>
 <tile id="3" class="Spawn">
  <image width="8" height="12" source="../Sprites/Mechanics/Spawn.png"/>
 </tile>
 <tile id="5" class="Gate">
  <image width="24" height="8" source="../Sprites/Temp/Temp_Gate.png"/>
 </tile>
 <tile id="6" class="Firefly">
  <properties>
   <property name="Next" type="object" value="0"/>
  </properties>
  <image width="8" height="8" source="../Sprites/Mechanics/Crystal Yellow.png"/>
 </tile>
 <tile id="7" class="FireflyPoint">
  <properties>
   <property name="Next" type="object" value="0"/>
  </properties>
  <image width="8" height="8" source="../Sprites/Mechanics/Firefly End.png"/>
 </tile>
 <tile id="8" class="Waterfall">
  <image width="8" height="8" source="Waterfall.png"/>
 </tile>
</tileset>
