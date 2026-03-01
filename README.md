# LDUFK-cs2-modpack

## Mods
* [Metamod](https://github.com/roflmuffin/CounterStrikeSharp)
* [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)
  [release 1.0.363](https://github.com/roflmuffin/CounterStrikeSharp/releases/tag/v1.0.363)
* [MatchZy](https://github.com/shobhit-pathak/MatchZy) \
  [MatchZy docs](https://shobhit-pathak.github.io/MatchZy/commands/)
  [release v0.8.15](https://github.com/shobhit-pathak/MatchZy/releases/tag/0.8.15)

## How to install
1. Build modpack
```sh
make
```
2. Add metamod to seach path
> game/csgo/addons/metamod
```diff
    FileSystem
    {
        SearchPaths
        {
            Game_LowViolence    csgo_lv // Perfect World content override

+           Game    csgo/addons/metamod
            Game    csgo
            Game    csgo_imported
            Game    csgo_core
            Game    core
```
3. Install modpack
```sh
cp -rf ./overlay/* /your/server/files/game/csgo/
```
