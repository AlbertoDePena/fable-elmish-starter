# Fable Elmish Starter


### Setting up your development environment

* Run `dotnet --list-sdks` to capture latest version 6 sdk
* Create `global.json` file in fable-elmish-starter folder

```json
{
    "sdk": {
        "version": "enter latest version 6 sdk here",
        "rollForward": "latestFeature"
    }
}
```

* Install `fable` dotnet tool

```bash
C:\> dotnet tool install --global fable
```

* Install `npm` dependencies

```bash
C:\fable-elmish-starter> npm install
```

### Run project

* Run fable to compile FSharp to JavaScript
* Run parcel to trans-pile JavaScript and open web server

```bash
# run debug mode
C:\fable-elmish-starter> npm run start
```

```bash
# build for production
C:\fable-elmish-starter> npm run build
```
