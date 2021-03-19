# VU Mod Manager Registry
Registry for the Venice Unleashed Mod Manager

Venice Unleashed Mod Manager is a tool server owners can use to automatically install mods and their dependencies.
It also allows developers to easily distribute new versions and don't have to worry about shipping the proper dependencies.\
Furthermore the tool automatically checks compatibility between your installed mods and will warn you when something is off.

This project serves as the registry of the Venice Unleashed Mod Manager. It takes care of validating and storing published mod versions.
It's also possible to restrict mods/mod versions to specific people.\
This for example will all us to properly distribute Reality Mod to verified server hosters in the future once we release.

## Running the registry
### Docker
Every release of the registry has a Docker image available at Docker Hub (insert link)
(insert docker instructions)

### Building and running from source
If you wish to build the registry from source, first install [.NET 5.0](https://dotnet.microsoft.com/download/dotnet). \
Once that's installed running the registry should be as simple as running the following two commands from the `VUModManagerRegistry` folder:
```bash
dotnet build
dotnet run
```
This project can also be opened in Visual Studio or Jetbrains Rider and be run from within the IDE.

## API
We highly recommend using [our own tool](https://github.com/BF3RM/vumm-cli). In case you wonder what is possible with the registy, below is an overview of the current available api endpoints.
### Fetch mod metadata
Fetch all metadata of a mod (description, authors, tags & versions)\
`GET /api/v1/mods/<name>`

| Parameter | Description | Required |
| --------- | ----------- | -------- |
| name      | Mod Name    | True     |

Example response
```json
{
  "name": "realitymod",
  "description": "A 'Project Reality'-Style mod for Battlefield 3",
  "author": "BF3: Reality Mod Team",
  "tags": {
    "latest": "0.1.0",
    "qa": "0.2.0-rc.2"
  },
  "versions": {
    "0.1.0": {
      "name": "realitymod",
      "description": "A 'Project Reality'-Style mod for Battlefield 3",
      "author": "BF3: Reality Mod Team",
      "version": "0.1.0",
      "dependencies": {
        "vemanager": "^1.0.0",
        "blueprintmanager": "^1.0.0"
      }
    },
    "0.2.0-rc.2": {
      "name": "realitymod",
      "description": "A 'Project Reality'-Style mod for Battlefield 3",
      "author": "BF3: Reality Mod Team",
      "version": "0.2.0-rc.2",
      "dependencies": {
        "vemanager": "^1.0.0",
        "blueprintmanager": "^1.0.0"
      }
    }
  }
}
```

### Fetch mod version metadata
Fetch a specific version and get it's dependencies\
`GET /api/v1/mods/<name>/<version>`

| Parameter | Description | Required |
| --------- | ----------- | -------- |
| name      | Mod Name    | True     |
| version   | Mod Version | True     |

Example response
```json
{
  "name": "realitymod",
  "description": "A 'Project Reality'-Style mod for Battlefield 3",
  "author": "BF3: Reality Mod Team",
  "version": "0.1.0",
  "dependencies": {
    "vemanager": "^1.0.0",
    "blueprintmanager": "^1.0.0"
  }
}
```

### Download mod version archive
Download mod version archive in a oclet-stream as gzipped tarbal (tar.gz)\
`GET /api/v1/mods/<name>/<version>/archive`

| Parameter | Description | Required |
| --------- | ----------- | -------- |
| name      | Mod Name    | True     |
| version   | Mod Version | True     |


### Publish mod version
Publishing a mod version\
`PUT /api/v1/mods/<name>/<version>`

| Parameter | Description | Required |
| --------- | ----------- | -------- |
| name      | Mod Name    | True     |
| version   | Mod Version | True     |
...

### Unpublish mod version
Unpublish a mod version and remove it's archive from the registry.
If no versions are left of a mod, the mod itself is fully removed from the registry.
This action is not reversible!\
`DELETE /api/v1/mods/<name>/<version>`

| Parameter | Description | Required |
| --------- | ----------- | -------- |
| name      | Mod Name    | True     |
| version   | Mod Version | True     |

## License
The Venice Unleashed Mod Manager Registry is available under the MIT license. See the [LICENSE](./LICENSE) file for more info.