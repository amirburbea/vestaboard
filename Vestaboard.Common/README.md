# Vestaboard Common

This library comprises a set of shared utilities used to interact with the Vestaboard.

Requirements:

- The software runs on Microsoft's .NET 8.0+. Download and install the [SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) for your operating system (Linux/Windows/MacOS). If you're using Windows, an install of Visual Studio 2022/2023 with recent patches will have already included this.
- A Vestaboard obviously. For now this library has a hard-coded assumption that your Vestaboard has the network name `vestaboard.local`. You can set this in your hosts file or router - but that's beyond the scope of this document.
- Your Vestaboard will need to have "local API" enabled. Follow the instructions [here](https://docs.vestaboard.com/docs/local-api/authentication) to get "local API" enabled on your Vestaboard.
- This libary expects the API key to be stored in user secrets associated directly with this project. I'm not sure how to do that without VS2022, but in Visual Studio you can right click on the project and click "Configure User Secrets...".  Configure a setting with a key of `LOCAL_API_KEY` and a value of the key you got for your Vestaboard.
