# Convert Markdown to Html in the build process
This project contains a gulpfile that converts Markdown files to Html.

## Step 0 : Prerequisites
I have the following softwares installed on my machine.

* Visual Studio 2015 Update 1
* Visual Studio Code - Insiders
* Node.js
* Gulp

## Step 1 : Create your project
* Create a new project in Visual Studio called *MarkdownDemo*
* Add the files from this GitHub repo to a new *Documentation* folder in the solution folder  
![](Media/005_FilesOnDisk.PNG)  
* You can add solution folder an add the files in Visual Studio  
![](Media/004_VisualStudioSolution.PNG)

## Step 3 : Build the Documentation with Gulp

### Install all node packages
Open a command promt and navigate to the documentation folder.  
`..\Documentation>npm install`  
*Install all npm packages*

### Build the documentation in Dev mode
`..\Documentation>node_modules\.bin\gulp BuildDocumentation`  
*Run the BuildDocumention task with Gulp*  

Open the Test.html file in the Markdown folder in a browser.

![](Media/001_DevBuild.PNG)  
*In the top right corner the html file is marked as a DevBuid*

![](Media/003_CodeHighlight.PNG)  
*Code are highlighted*

### Build the documentation in Release mode
`..\Documentation>node_modules\.bin\gulp BuildDocumentation  --BuildNumber "Version 1.0"`

![](Media/002_ReleaseBuild.PNG)  
*In the top right corner the html file is marked with the buildnumber Version 1.0*

### Generate the html file when the Markdown file are saved
`..\Documentation>node_modules\.bin\gulp Watch`  
*If you run the **Watch** task and the html file will be generated in Dev Mode every time you save your markdown file* 

## Step 4 : Use VSCode to edit the documentation files
![](Media/006_OpenInVSCode.PNG)  
*Open the Documentation folder with VSCode*

* Right click on the documentation folder and **Open with Code - Insider**
* Ctrl + Shift + B

![](Media/007_VSCodeStartWatcher.PNG)  
*The watch task starts*

If you open the html file in Chrome, edit and save the markdown file. Chrome will automatically reload the page.

## Step 5 : Generate the documentation as a part of your build in TFS

### Build Definition
![](Media/008_BuildTemplate.PNG)  
*Add the npm, Gulp and Copy and Publish Build Artifacts tasks to your build definition*

### npm
![](Media/009_Npm.PNG)  
*Run the install command and set the Working Directory to the Documentation folder*

### Gulp
![](Media/010_Gulp.PNG)  
*Run the BuildDocumentation task, add the --BuildNumber argument and set the working directory to the Documentation folder*

### Copy and Publish Build Artifacts
![](Media/011_CreateArtifact.PNG)  
*Add all html files to the Documentation artifact*

### Build Result
![](Media/012_BuildResult.PNG)  
*The Documentation artifact is created for Build 20160303.1*

### Artifact
![](Media/013_ExploreArtifact.PNG)  
*Download and open the Test.html file*

![](Media/014_BuildNumerAdded.PNG)  
*In the top right corner the html file is marked with the buildnumber 20160303.1*


