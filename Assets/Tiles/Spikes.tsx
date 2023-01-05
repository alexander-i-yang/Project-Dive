<?xml version="1.0" encoding="UTF-8"?>
<tileset version="1.9" tiledversion="1.9.2" name="Spikes" tilewidth="8" tileheight="8" tilecount="16" columns="4">
 <image source="Spikes.png" width="32" height="32"/>
 <tile id="0" class="SpikeUp">
  <objectgroup draworder="index" id="3">
   <object id="5" x="1" y="6" width="6" height="2"/>
  </objectgroup>
 </tile>
 <tile id="1" class="SpikeLeft">
  <objectgroup draworder="index" id="2">
   <object id="1" x="4" y="1" width="4" height="6"/>
  </objectgroup>
 </tile>
 <tile id="2" class="SpikeDown">
  <objectgroup draworder="index" id="2">
   <object id="1" x="1" y="0" width="6" height="2"/>
  </objectgroup>
 </tile>
 <tile id="3" class="SpikeRight">
  <objectgroup draworder="index" id="2">
   <object id="1" x="0" y="1" width="2" height="6"/>
  </objectgroup>
 </tile>
 <tile id="9" class="DirectionalSpikeUp">
  <properties>
   <property name="SetDirectionFromCode" type="int" value="1"/>
  </properties>
 </tile>
 <tile id="10" class="ConnectedSpikeUp">
  <properties>
   <property name="SetDirectionFromCode" type="int" value="1"/>
  </properties>
  <objectgroup draworder="index" id="2">
   <object id="1" class="ConnectedSpike" x="0" y="7" width="8" height="1"/>
  </objectgroup>
 </tile>
</tileset>
