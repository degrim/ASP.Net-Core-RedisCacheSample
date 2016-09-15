# ASP.Net Core Redis Distributed Cache Sample Overview

The purpose of this sample is to demonstrate how to implement Redis Cloud service in Bluemix as an IDistributedCache provider for session caching in ASP.Net Core.  This allows each instance of the application to retrieve session data from the Redis Cloud database so that the user's data is persisted accross multiple instances.

## Running the app on Bluemix
The easiest way to run this sample on Bluemix is to click the Deploy to Bluemix button below.  This will deploy 2 instances of the application in order to demonstrate how session data is persisted accross instances.

[![Deploy to Bluemix](https://bluemix.net/deploy/button.png)](https://bluemix.net/deploy)

The app can also be pushed to Bluemix from the command-line by following these steps:
1. Create a Bluemix Account

  [Sign up][sign_up] in Bluemix, or use an existing account.

2. Download and install the [Cloud-foundry CLI][cloud_foundry] tool

3. Edit the `manifest.yml` file and change the `<application-name>` to something unique.  You can optionally adjust the number of instances here, but 2 is the minimum to demonstrate the functionality of the sample.
  ```none
applications:
- name: <application-name>
  memory: 512
  instances: 2
  services:
  - rediscloud-service
  ```
  The name you use will determine your application url initially, e.g. `<application-name>.mybluemix.net`.

4. Connect to Bluemix in the command line tool.  Note that https://api.ng.bluemix.net is the api endpoint for the US South region, if you are using a different region you should specify that region's endpoint here.
  ```sh
  $ cf api https://api.ng.bluemix.net
  $ cf login -u <your user ID>
  ```

5. Create the Redis Cloud service in Bluemix using the 30mb data plan (currently free).
  ```sh
  $ cf create-service rediscloud 30mb rediscloud-service
  ```

6. Push it live!
  ```sh
  $ cf push
  ```


## Run the app locally
This sample app uses .Net Core version 1.1.0 and ASP.NET Core 1.1.1.
The app can be run locally from Visual Studio or using the command-line tools, but this will only run one instance of the app and will not demonstrate the session data being persisted accross instances.

1. Copy the credentials from your `rediscloud-service` service in Bluemix to `src/RedisSample/config.json`, you can see the credentials using:

    ```sh
    $ cf env <application-name>
    ```
    Example output:
    ```sh
    System-Provided:
    {
    "VCAP_SERVICES": {
      "rediscloud": [{
          "credentials": {
            "hostname": "<hostname>",
            "password": "<password>",
            "port": "<port>"
          },
        "label": "rediscloud",
        "name": "rediscloud-service",
        "plan": "30mb"
     }]
    }
    }
    ```

    You need to copy the value of `VCAP_SERVICES` to `src/RedisSample/config.json`.

2. Install [.Net Core SDK](https://www.microsoft.com/net/download/core)
  Be sure to install and use the 1.1 version of the CoreCLR runtime and .NET SDK version 1.0.0-preview2-1-003177:

3.1 Run the project (Linux/Mac). Go to the project folder in a terminal and run:
  ```sh
  $ dotnet restore
  $ dotnet run
  ```

3.2 Run the project (Windows).
  Open the solution in Visual Studio 2015 and wait for NuGet to restore packages, then press F5 to start debugging.  Alternatively, you can run the project from a command-line as described in section 3.1.

4. Go to `http://localhost:5000`

## Decomposition Instructions
* See src/RedisSample/Startup.cs to see how to obtain the Redis Cloud credentials and configure Redis for both session data and data protection keys.

## Troubleshooting

The primary source of debugging information for your Bluemix app is the logs. To see them, run the following command using the Cloud Foundry CLI:

  ```
  $ cf logs <application-name> --recent
  ```
For more detailed information on troubleshooting your application, see the [Troubleshooting section](https://www.ng.bluemix.net/docs/troubleshoot/tr.html) in the Bluemix documentation.

## License

  This sample code is licensed under Apache 2.0. Full license text is available in [LICENSE](LICENSE).

## Open Source @ IBM
  Find more open source projects on the [IBM Github Page](http://ibm.github.io/)

### Useful links
[IBM Bluemix](https://bluemix.net/)  
[IBM  Bluemix Documentation](https://www.ng.bluemix.net/docs/)  
[IBM Bluemix Developers Community](http://developer.ibm.com/bluemix)  

[cloud_foundry]: https://github.com/cloudfoundry/cli
[sign_up]: http://bluemix.net/
