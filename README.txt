====== Contents ==============================================================
 0. Introduction
 1. Compiling kAI
 2. Compiling the example games
 3. Running the tool
    3.1 Creating a new project
    3.2 Loading an example project
 4. Running the example games


0. Introduction
------------------------------------------------------------------------------
kAI is Thomas Kiley's third year project, for a degree is Discrete Mathematics
from the University of Warwick. 

The purpose of the project was to Develop a tool in C# which takes the idea of
a component based objects (as opposed to inheritance hierarchies) that can be 
used to create AI behaviours for a wide variety of game AI's.

The source can be obtained from github: https://github.com/thk123/kAI/ though
if you are reading this, you presumably have this already. 

The tool and examples work on Windows only. There are precompiled versions of
them in the Builds\ directory in the root of the source. See 3-5 for 
instructions on how to run the examples. 1- 2 outlines how to compile the 
projects individually. 


1. Compiling kAI
------------------------------------------------------------------------------
In the root of the source, there is a Visual Studio 2010 solution file. This
can be opened using Visual Studio 2010 (express version downloadable from:
http://www.visualstudio.com/downloads/download-visual-studio-vs#d-2010-express
ensuring to get the C# flavour).

Optionally, to build the documentation you will need to install the Sandcastle
plugin: https://sandcastle.codeplex.com/releases/view/47665.

Once the solution is loaded, the entire solution can be built by running "Build
solution". 

This will generate the kAI Core DLL (used to run the behaviours) and the editor
for creating and editing behaviours. 


2. Compiling the example games
------------------------------------------------------------------------------
The example games are created in Unity3D, which is freely downloadable from 
Unity's homepage: http://unity3d.com/unity

To open the simple example, load Unity, click open project and select open 
other to browse for the project. Select the kAI-Example directory (contains 
folders called Assets, Library etc.). This will load the Unity project. Once
it is loaded, select the Main.scene file from the assets browser in the lower
pane.  Finally, you can click the play button to run the game.

To build the game, using File>Build and Run and configure and select the 
desired platform. Finally, the kAI behaviours must be copied into the build 
directory: located the folder called kAIBehaviours in the Assets folder of the
project. Copy this folder into the build data of the built game. 


3. Running the tool 
------------------------------------------------------------------------------
If you have compiled the tool from the kAI solution, it can be run from there.
Otherwise, there is a precompiled tool in the Builds/ directory. Double click
on the executable will launch the application. Full instructions on using the
tool can be found in the final report in chapter 4 (avaiable from where you 
obtained the source), below is a breif guide on how to set up a basic project
or load one of the existing ones. 

     3.1. Creating a new project
     -------------------------------------------------------------------------
     Once the tool has loaded, click File>New Project to create a new project.
     
     You will need to give it a name and load any DLLs you want to use in it.


     3.2 Loading an example project
     -------------------------------------------------------------------------
     Once the tool has loaded, click File>Open Project and browse to the 
     .kAIProj file in the example directory. The behaviours can be seen on the
     right hand side of the editor. Double click on one to load that 
     kAI-behaviour. 


4. Running the example games
------------------------------------------------------------------------------
The two example games can be found in the Builds/Games/ directory. Double click
either executable to launch the games to see the AI. Use Alt-F4 to exit at any
point. 



