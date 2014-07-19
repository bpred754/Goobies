Goobies Environment Setup
==============================================
1. Install Visual c# 2010 Express from http://www.visualstudio.com/en-us/downloads#d-2010-express
2. In the Git Shell/Bash navigate to Documents/Visual Studio 2010/Projects and create a new directory with 'mkdir Goobies'
3. Step inside the Goobies directory with 'cd Goobies'
4. Initialize git with 'git init'
5. Set up a remote repository with 'git remote add goobies https://github.com/bpred754/goobies.git'
6. Pull code from master repository with 'git pull https://github.com/bpred754/goobies.git'
7. In Windows Explorer navigate to Documents/Visual Studio 2010/Projects/Goobies and double click Goobies.sln
8. If a 'debug target missing' error occurs when trying to run the project go to Build -> Configuration Manager and make 
sure there is a check mark for Goobies in the Build column. Then go to Build -> Build Solution, and once this is complete 
the project should be able to run.

Enabling Keyboard Input
==============================================
1. Open Game1.cs in Goobies root directory
2. Comment out line 108 'screenStack.Peek().listen(GamePad.GetState(PlayerIndex.One));'
3. Uncomment lines 111 and 112 where it says 'DEBUG --Keyboard Listener'

Using Keyboard
==============================================
1. Start Screen
  - Nagivate with up/down keys
  - Select with enter key
2. Select Map Size Screen
  - Same as Start Screen
3. Select Map Screen
  - Navigate with left/right keys
  - Select with enter key
4. Select Goobies Screen
  - Change Goobies with left and right keys
  - Select Goobie with space bar
  - Move cursor with number pad (refer to movement section)
  - When done press enter key to go to Game Screen
5. Game Screen
  - Rotate camera with left and right arrows
  - Rotate camera into birds eye view up arrow
  - Select with enter key
  - Switch unit left with comma
  - Switch unit right with period
  - Change turns with backspace
  - Move cursor with number pad (refer to movement section)
6. Movement
  - Up left - 7
  - Up - 8
  - Up right - 9
  - Left - 4
  - Right - 6
  - Down left - 1
  - Down - 2
  - Down right - 3
7. Game Screen keyboard keys can be changed in Goobies/GameObjects/Controllers/PlayerController.cs in the bottom section 
labeled 'DEBUG -- Keyboard listeners'

Committing Changes
==============================================
1. First update local repository with 'git pull https://github.com/bpred754/goobies.git'
2. Add all modified files to staging area with 'git add -u'
3. Commit changes to local git repository with 'git commit -m "place commit comment here"'
4. Push changes to master repository with 'git push goobies master'

Requirements
==============================================
1. Git installed
2. Goobies Contributor
