rm %LOCALAPPDATA%\Mojang\Scrolls\game\Scrolls_Data\Managed\ModLoader\mods.ini
rm %LOCALAPPDATA%\Mojang\Scrolls\game\Scrolls_Data\Managed\ModLoader\mods\AnimationControl\AnimationControl.mod.dll
rm %LOCALAPPDATA%\Mojang\Scrolls\game\Scrolls_Data\Managed\ModLoader\mods\AnimationControl\config.json
mkdir %LOCALAPPDATA%\Mojang\Scrolls\game\Scrolls_Data\Managed\ModLoader\mods\AnimationControl
cp AnimationControl.mod.dll %LOCALAPPDATA%\Mojang\Scrolls\game\Scrolls_Data\Managed\ModLoader\mods\AnimationControl\

PAUSE

%LOCALAPPDATA%\Mojang\Scrolls\game\Scrolls.exe