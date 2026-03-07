#!/usr/bin/env bash

GI_PATH="/home/steam/cs2-dedicated/game/csgo/gameinfo.gi"


while read -r line; do printf '\e[32m%s\e[0m\n' "$line"; done <<'BANNER'
.____     ________   ____ __________________  __.
|    |    \______ \ |    |   \_   _____/    |/ _|
|    |     |    |  \|    |   /|    __) |      <  
|    |___  |    `   \    |  / |     \  |    |  \ 
|_______ \/_______  /______/  \___  /  |____|__ \
        \/        \/              \/           \/
BANNER

if ! grep -q "csgo/addons/metamod" "$GI_PATH"; then
sed -E -i '
/^[[:space:]]*SearchPaths[[:space:]]*$/,/^[[:space:]]*}/{
    /^[[:space:]]*Game[[:space:]]+csgo[[:space:]]*$/i\
                        Game    csgo/addons/metamod
}
' "$GI_PATH"
fi

rm -rf /home/steam/cs2-dedicated/game/csgo/addons/counterstrikesharp
rm -rf /home/steam/cs2-dedicated/game/csgo/addons/matchup
rm -rf /home/steam/cs2-dedicated/game/csgo/addons/cfg/MatchZy

cp -rf /overlay/* /home/steam/cs2-dedicated/game/csgo/
