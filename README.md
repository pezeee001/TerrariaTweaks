# Terraria Tweaks

Basic modloader for Terraria. A loader is injected into Terraria.exe which loads .dll's from Tweaks folder.  
Tweaks are based on Harmony.

## How to use
1. Download [latest release](https://github.com/pezeee001/TerrariaTweaks/releases/latest).
1. Put contents of the archive in your Terraria folder.
1. Start TweakPatcher.exe. 
    * This will inject a .dll loader into Terraria.exe.
    * It will print a success message or an error if anything goes wrong.
    *  **You need to do this everytime the game updates!**
1. Put tweaks into Tweaks folder. 
1. Launch game normally.

## How do I know it works?
See if any tweaks you use actually work.

## Is this related to TerrariaTweaks (TT2) by TiberiumFusion?
No. I just couldn't pick a better name. It was an inspiration however. I tried using it on 1.4.5 and it obviously didn't work
so I tried writing my own patcher.

## How do I unpatch my game?
Patcher creates a backup at Terraria.exe.bk, simply delete Terraria.exe and restore the backup.  
Another method is validating your files through Steam.
