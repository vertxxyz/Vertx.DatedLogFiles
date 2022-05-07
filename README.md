# Vertx.DatedLogFiles
Renames old Unity Player logs (`Player-prev.log`) with the date they were created.  
Only performed on standalone platforms.

## Usage
**Either:**  
Add the **Date Log Files** component to an object in your startup scene.  
**or**  
Call `DateLogFiles.Run` in any startup context.

## Installation

<details>
<summary>Add from GitHub</summary>

- Open Package Manager
- Click <kbd>+</kbd>
- Select <kbd>Add from Git URL</kbd>
- Paste `https://github.com/vertxxyz/Vertx.DatedLogFiles.git`
- Click <kbd>Add</kbd>  
  **or**
- Edit your `manifest.json` file to contain `"com.vertx.dated-log-files": "https://github.com/vertxxyz/Vertx.DatedLogFiles.git"`

To update the package with new changes, remove the lock from the `packages-lock.json` file.
</details>
