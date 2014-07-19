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

testing commit
test Commit from mac
