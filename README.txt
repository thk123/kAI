====== Contents ==============================================================
 0. Introduction
 1. File Structure
 2. Compiling kAI
 3. Compiling the documentaion
 4. Compiling the example games
 5. Running the tool
    5.1 Creating a new project
    5.2 Loading an example project
 6. Browsing the documentation
 7. Running the example games


0. Introduction
------------------------------------------------------------------------------
kAI is Thomas Kiley's third year project, for a degree is Discrete Mathematics
from the University of Warwick. 

The purpose of the project was to develop a tool in C# which takes the idea of
a component based objects (as opposed to inheritance hierarchies) that can be 
used to create AI behaviours for a wide variety of game AI's.

The source can be obtained from github: https://github.com/thk123/kAI/ though
if you are reading this, you presumably have this already. 

The tool is Windows only. The example games can be run on Linux. There are 
precompiled versions of them in the Builds\ directory in the root of the 
source. See 4-5 for instructions on how to run the examples. 2 - 3 outlines 
how to compile the projects individually. 

The ProjectReport.pdf is probably a little large for casual reading, though
section 4 contains detailed instructions on using kAI. I will be blogging
about this shortly.. 

1. File Structure
-----------------------------------------------------------------------------
 + root
    +Builds
        - zips of all the executables, ready compiled for you!
    +FileMap
        –Library for FileMap code
    +GroundExample
        –Source code for the strategy game example
    +kAICore
        –Source code for the core of kAI
    +Debug
        –Source code for the debug API
    +kAIDocs
        –Project for generating source code   

        +hmtl
            –The generated documentation
    +kAI-Editor
         –Source code for the editor
    +kAI-Example
         –Source code for the sword duel example
    +SpaceExample
        –Source code for the abandoned example with the inertia flight 
        model
    +SpriteTextRenderer
        –Source code for the SDX library used in the editor
    +ThreadMessaging
        –Source code for the semaphores library that was used
    +Tools
        –Installers for compiling kAI


2. Compiling kAI
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

3. Compiling the Docs
------------------------------------------------------------------------------
kAI comes generated documentation which can be found in appendix F (see 6 to 
find out how to read it). To generate this, Sandcastle needs to be installed.
This is a plugin for visual studio. An installer can be found in the tools 
folder in src. Alternatively, it can be optained from:
https://sandcastle.codeplex.com/  

Once this is installed and kAI has been compiled, ensure in the project 
properties for kAI that the XML Documentation checkbox is ticked (in the build
tab of the project properties). Then proceed to build the kAI sandcastle 
project to build the documentation. This takes some time. 

4. Compiling the example games
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


5. Running the tool 
------------------------------------------------------------------------------
If you have compiled the tool from the kAI solution, it can be run from there.
Otherwise, there is a precompiled tool in the Builds/ directory. 

You will need SlimDX installed which can be obtained from SlimDX.org

Double click on the executable will launch the application. Full instructions 
on using thetool can be found in the final report in chapter 4 (avaiable in the root of 
this disk), below is a breif guide on how to set up a basic project or load one
of the existing ones. 

     5.1. Creating a new project
     -------------------------------------------------------------------------
     Once the tool has loaded, click File>New Project to create a new project.
     
     You will need to give it a name and load any DLLs you want to use in it.


     5.2 Loading an example project
     -------------------------------------------------------------------------
     Once the tool has loaded, click File>Open Project and browse to the 
     .kAIProj file in the example directory. The behaviours can be seen on the
     right hand side of the editor. Double click on one to load that 
     kAI-behaviour. 

6. Browsing the documentation
------------------------------------------------------------------------------
To browse the documentation, open index.html in a web browser. N.b. Chrome 
does not support frame navigation, Firefox is the recommended browser for 
viewing the documentation. 

7. Running the example games
------------------------------------------------------------------------------
The two example games can be found in the Builds/Games/ directory. Double click
either executable to launch the games to see the AI. Use Alt-F4 to exit at any
point. 
