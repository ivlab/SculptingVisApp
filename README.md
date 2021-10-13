# SculptingVisApp

This is the Unity component of the main app for SculptingVis. It includes:

- The Artifact-Based Rendering engine
- Interactive camera control / screenshots
- Interactive lighting control
- VR support via Unity XR



## Developer Setup

SculptingVisApp handles the "Graphics Engine" potion of the ABR architecture,
and adds the additional features listed above. See below for a complete diagram
of the ABR architecture, including the Design User Interfaces, Server, Graphics
Engines, and Data Hosts.

![The four-component ABR Architecture, including Design User Interfaces, Server,
Graphics Engines, and Data
Hosts.](https://www.sculpting-vis.org/wp-content/uploads/2021/05/abr_components.png)


### Required components

- A [GitHub SSH key](https://docs.github.com/en/github-ae@latest/github/authenticating-to-github/connecting-to-github-with-ssh/generating-a-new-ssh-key-and-adding-it-to-the-ssh-agent) associated with your UMN GitHub account on your development machine
- The [abr_server](https://github.umn.edu/ivlab-cs/abr_server)
    - Optionally, this can be run inside Docker - see instructions inside that
    repo for running inside Docker.
- Read-write access on the package(s) you want to work on. All of the Unity
packages in this project are included via the Unity Package Manager (UPM), which means
they are *read-only*. To enable read-write developer access on a package (say,
the [ABREngine-UnityPackage](git@github.umn.edu:ivlab-cs/ABREngine-UnityPackage.git)):
    1. Navigate to the `Packages` folder inside of this project
    2. Clone your requisite repository (e.g. `git clone git@github.umn.edu:ivlab-cs/ABREngine-UnityPackage.git`)
    3. You can now edit the package as you would any other
    4. In the SculptingVisApp parent repo, make sure **not to commit** anything
    from the Packages folder now - that will add a submodule to this Unity
    project, which we don't want. All end-user dependencies should continue to
    be managed by the UPM.
    5. When you're ready to publish the next version of the Unity package, make sure all your changes are committed/pushed, then delete that repo in the Packages folder of the parent SculptingVisApp repo as well as the `packages-lock.json` file. Go back to the Unity editor and all your packages should be refreshed, including getting the lastest from your development version. You may also need to delete the `Library/PackageCache` folder. (`packages-lock.json` saves the commit hash for your git-dependency packages so this needs to be updated whenever you have new content in that package, `Library/PackageCache` saves a local copy of each git package)
