# NLog Hipchat extension for dot net core 

## installation
 

* You need to add the following line in your `nlog.config` inside the `extension` section

    ```XML
    <extensions>
    ...

        <add assembly="MHipchat.NLogCore.Extension"/>

    ...
    </extensions>
    ```

* add following line in your `nlog.config`'s `targets` section

    ```XML
    <targets>
       ...

       <target xsi:type="MHipChat" name="hipChat" 
          layout="/code ${message}${when:when=length('${exception}')>0:Inner=${NewLine}}${exception}" authToken="{auth token here}" roomId="{room Id here}"
          from="JobDelivaryMan.Pbapp - ${level}" NotifyMinLevel="Info">
          <!--you can setup the color of different level here, uncomment bellow-->
          <!--<level-color Level="debug" Color="purple" />-->
        </target>

        ...
    </targets>
    ```

    > note that you need to update the **authToken** and **roomId** in above.
    > for detail about configuration read the [Configuration](#config) section in this page
    
* add following line in your rule

    ```XML
    <rule>
       ...

       <logger name="*" minlevel="Error" writeTo="hipChat" />

       ...
    </rule>
    ```

* include `MMHipchat.NLogCore.Extension.dll` to your build folder


> A working sample of nlog.config can be find in the console sample project


<section id="config"></section>

## Configuration

| Property | isRequired| Description |
|----------|-----------|-------------|
| roomId   | yes | the room id that you want to send logs to|
| authKey  | yes | the token, you need to get it from hipchat website |
| from     | no  | it's good practice to set a name for the logger. so if you have different log in one room, then you can know source of each log. you can use any layout variable that is available in nlog|
| notifyMinLevel | no | you decide what level onward send with notify equal to true, notify as true will show a pop up in hipchat desktop or mobile push notification, if notify as false there will be no notification for the user in the room, valid levels are `Debug` `Error` `Fatal` `Info` `Off` `Trace` `Warn`|

you can also decide color for each level. logs with that color will be send to hipchat.

by default these colors is set for each level 


| level | color |
|-------|------|
|Debug| Purple |
| Error|Red|
|Fatal|Red|
|Info|Green|
|Off|Gray|
|Trace|Yellow|
|Warn|Yellow|

to change the color you can add following 

```XML
<level-color Level="debug" Color="red" />
```

>you can set multiple of these colors together 


