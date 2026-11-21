# Building Nautilus from Source

This repository contains **source code only**.  
Required runtime binaries are distributed separately via GitHub Releases.

## Prerequisites
- Windows 10/11
- Visual Studio 2022
- .NET Desktop Development workload installed

## Step 1: Clone the Source
git clone https://github.com/NemosNautilus/Nautilus.git
or download ZIP from GitHub.

## Step 2: Download Runtime Binaries
1. Go to **Releases**
2. Download the latest release zip
3. Extract it

You should have:
bin\ and res\ folders

## Step 3: Place Runtime Files
Copy the extracted folders into:
Nautilus\Nautilus\bin\Debug\

(or `Release` if building Release)

## Step 4: Open Solution
Open `Nautilus.sln` in Visual Studio.

If references are missing:
- Right-click project → Add Reference → Browse
- Point to the DLLs in `bin\Debug`

The main ones to look for are Naudio.dll, Naudio.Midi.dll, NautilusFREE.dll, Bass.Net.dll and SharpMP4Parser.dll.
I might be forgetting some but VS will scream at you when trying to compile. Just pay attention.

## Step 5: Build
Build the solution.

The application should now compile and run.

## Notes
- Runtime binaries are kept up to date with releases, not with source
- If you update source code and something breaks, make sure you have grabbed the latest runtime binaries

I hope this helps more of you be able to compile and edit Nautilus
